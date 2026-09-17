using System.Collections.Generic;

namespace ADHDGoneWild.Trail
{
    /// <summary>
    /// Where the player has been, this session.
    ///
    /// The whole rule lives here, with no game types in it, because this is the part that has to
    /// be *right* and the part nobody can check by playing: whether a fly-past leaves a mark,
    /// whether coming back to a roundabout makes a second entry, whether panning along a highway
    /// turns the trail into one enormous place. Those are questions for a test, not for a city.
    ///
    /// The idea it is built on is dwell, not travel. A sample either lands inside an existing
    /// place or it starts a new one, and a place only becomes visible after it has held the
    /// player's attention for <see cref="PromoteAfterSeconds"/>. Crossing the map therefore leaves
    /// a trail of one-second candidates that never promote and are dropped - no "is the camera
    /// settled" test is needed, because the radius already asks that question.
    ///
    /// What it deliberately does not do:
    ///
    ///   - it never looks at what was built, only at where the camera was and which tool was held;
    ///   - it never decides anything is unfinished, or a project, or worth returning to;
    ///   - it keeps no count the player can see, and nothing here is ever shown as a total;
    ///   - it forgets everything when the city closes.
    ///
    /// That last one is the point of the feature rather than a limitation of it. Welcome Back is
    /// the memory between sessions. This is the memory inside one, and a thing that only lasts as
    /// long as the session cannot turn into a backlog waiting for you tomorrow.
    /// </summary>
    public sealed class AttentionTrail
    {
        /// <summary>
        /// How far from a place's centre still counts as being at it, in metres.
        ///
        /// About a neighbourhood. Small enough that the two ends of a long avenue are different
        /// places, large enough that nudging the camera to see round a building is not.
        /// </summary>
        public const float MergeRadius = 250f;

        /// <summary>
        /// Attention a place must hold before the player is told it exists.
        ///
        /// Twenty-five seconds is roughly "I stopped here and did something", and comfortably more
        /// than a camera passing through. It is the single number that decides whether this
        /// feature is a memory or a logfile.
        /// </summary>
        public const int PromoteAfterSeconds = 25;

        /// <summary>
        /// How many places the player is ever shown. A trail you can read at a glance; past this,
        /// the least recently visited drops off the back.
        /// </summary>
        public const int MaxPlaces = 8;

        /// <summary>
        /// Candidates kept alongside them. Flying across the city mints one per second, so this is
        /// only here to bound the churn - none of them is visible.
        /// </summary>
        public const int MaxTracked = MaxPlaces + 16;

        /// <summary>
        /// Attention stops accruing after fifteen minutes at one place.
        ///
        /// Which is the whole answer to being away from the keyboard: the mod does not have to
        /// detect it, because a camera left pointing at a park for an hour reaches the cap and
        /// stops there, and the park never outranks anywhere else.
        /// </summary>
        public const int AttentionCapSeconds = 900;

        /// <summary>
        /// The largest gap between two samples that is credited as attention.
        ///
        /// A longer one means a loading screen, an alt-tab or a sleeping machine - not somebody
        /// sitting there. The same guard, for the same reason, as the five-second one in
        /// HyperfocusSystem.
        /// </summary>
        public const int MaxCreditedGapSeconds = 4;

        /// <summary>Most recently attended last. Kept in LastSeenUnixUtc order, always.</summary>
        private readonly List<TrailPlace> _places = new List<TrailPlace>();

        private long _lastSampleUnixUtc;
        private bool _hasCurrentView;
        private float _currentX;
        private float _currentZ;
        private int _nextId;

        /// <summary>
        /// Changes when a visible place is observed or the camera crosses a remembered place's
        /// edge. The UI can redraw when a way back appears or disappears without repainting for
        /// every one-second candidate made while crossing the city.
        /// </summary>
        public int VisibleRevision { get; private set; }

        /// <summary>The places worth showing, oldest visit first.</summary>
        public IEnumerable<TrailPlace> Places
        {
            get
            {
                for (var i = 0; i < _places.Count; i++)
                {
                    if (_places[i].Promoted)
                    {
                        yield return _places[i];
                    }
                }
            }
        }

        /// <summary>
        /// Places worth returning to. A remembered place stays in the trail while the camera is
        /// there, but offering to take the player to the view they already have is no help.
        /// The same radius used to merge visits decides whether a place is still current.
        /// </summary>
        public IEnumerable<TrailPlace> PlacesAwayFromCurrentView
        {
            get
            {
                const float limit = MergeRadius * MergeRadius;

                foreach (var place in Places)
                {
                    if (!_hasCurrentView || place.DistanceSquaredTo(_currentX, _currentZ) > limit)
                    {
                        yield return place;
                    }
                }
            }
        }

        public int PlaceCount
        {
            get
            {
                var count = 0;

                for (var i = 0; i < _places.Count; i++)
                {
                    if (_places[i].Promoted)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Take one second of attention into account.
        ///
        /// Returns the place that became visible because of this sample, or null. The revision
        /// also changes when an already visible place is visited again, so its row can stay current.
        /// </summary>
        public TrailPlace Observe(TrailSample sample)
        {
            // A movement across the edge of a remembered place changes the list even when the
            // camera lands in a one-second candidate. The UI needs a redraw to reveal the place
            // just left; otherwise it would stay hidden until another place was promoted.
            var visibleRowsChanged = CurrentViewChangesVisibleRows(sample.X, sample.Z);
            _hasCurrentView = true;
            _currentX = sample.X;
            _currentZ = sample.Z;

            var credited = Credit(sample.UnixUtc);
            _lastSampleUnixUtc = sample.UnixUtc;

            var place = Nearest(sample.X, sample.Z);

            if (place == null)
            {
                // Somewhere new. It starts at zero rather than inheriting the seconds since the
                // last sample: those were spent getting here, not here.
                _places.Add(new TrailPlace(NextId(), sample));
                Trim();
                if (visibleRowsChanged)
                {
                    VisibleRevision++;
                }
                return null;
            }

            var wasPromoted = place.Promoted;

            place.Remember(sample, credited);

            // One invariant, relied on by both the display order and the eviction rule: the list
            // is always in order of last attention.
            _places.Remove(place);
            _places.Add(place);

            Trim();

            if (place.Promoted || visibleRowsChanged)
            {
                VisibleRevision++;
            }

            return !wasPromoted && place.Promoted ? place : null;
        }

        /// <summary>A new city, or the same one reopened. Nothing carries over.</summary>
        public void Clear()
        {
            _places.Clear();
            _lastSampleUnixUtc = 0;
            _hasCurrentView = false;
            VisibleRevision++;
        }

        /// <summary>The place the player asked to go back to, or null if it has since dropped off.</summary>
        public TrailPlace Find(string id)
        {
            for (var i = 0; i < _places.Count; i++)
            {
                if (_places[i].Id == id && _places[i].Promoted)
                {
                    return _places[i];
                }
            }

            return null;
        }

        /// <summary>"Not that one." Takes a place off the trail for good.</summary>
        public bool Forget(string id)
        {
            for (var i = 0; i < _places.Count; i++)
            {
                if (_places[i].Id == id)
                {
                    _places.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private int Credit(long unixUtc)
        {
            if (_lastSampleUnixUtc == 0)
            {
                return 0;
            }

            var gap = unixUtc - _lastSampleUnixUtc;

            if (gap <= 0)
            {
                return 0;
            }

            return gap > MaxCreditedGapSeconds ? 0 : (int)gap;
        }

        private bool CurrentViewChangesVisibleRows(float x, float z)
        {
            if (!_hasCurrentView)
            {
                return false;
            }

            const float limit = MergeRadius * MergeRadius;

            foreach (var place in Places)
            {
                if ((place.DistanceSquaredTo(_currentX, _currentZ) <= limit) !=
                    (place.DistanceSquaredTo(x, z) <= limit))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The nearest place within the radius, not merely the first one found. Where two places
        /// overlap, the sample belongs to the one it is actually closer to - otherwise the older
        /// of the two quietly swallows every visit to its neighbour.
        /// </summary>
        private TrailPlace Nearest(float x, float z)
        {
            const float limit = MergeRadius * MergeRadius;

            TrailPlace best = null;
            var bestDistance = float.MaxValue;

            for (var i = 0; i < _places.Count; i++)
            {
                var distance = _places[i].DistanceSquaredTo(x, z);

                if (distance <= limit && distance < bestDistance)
                {
                    best = _places[i];
                    bestDistance = distance;
                }
            }

            return best;
        }

        /// <summary>
        /// Forgetting, in two stages: candidates first, then the visible places the player has been
        /// away from longest. Never the most recent - the question this feature answers is "where
        /// was I just now".
        /// </summary>
        private void Trim()
        {
            while (PlaceCount > MaxPlaces)
            {
                for (var i = 0; i < _places.Count; i++)
                {
                    if (_places[i].Promoted)
                    {
                        _places.RemoveAt(i);
                        break;
                    }
                }
            }

            while (_places.Count > MaxTracked)
            {
                var removed = false;

                for (var i = 0; i < _places.Count; i++)
                {
                    if (!_places[i].Promoted)
                    {
                        _places.RemoveAt(i);
                        removed = true;
                        break;
                    }
                }

                if (!removed)
                {
                    _places.RemoveAt(0);
                }
            }
        }

        private string NextId()
        {
            _nextId++;
            return "p" + _nextId.ToString();
        }
    }
}

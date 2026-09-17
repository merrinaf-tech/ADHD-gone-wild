using System;
using System.Collections.Generic;
using ADHDGoneWild.BrainParking;
using ADHDGoneWild.Core;
using ADHDGoneWild.Persistence;

namespace ADHDGoneWild.Memory
{
    /// <summary>
    /// Everything the mod remembers about the city currently open, and the single owner of its
    /// file on disk.
    ///
    /// One owner on purpose. Parked ideas and silenced alerts live in the same file, and two
    /// objects writing it would eventually write over each other. Features ask this for what they
    /// need; nobody else touches the store.
    ///
    /// Deliberately free of ECS and of Unity, so ordering, muting and persistence can be
    /// exercised without launching Cities: Skylines II. Nothing in here counts anything towards
    /// anything, ages it out, or marks it overdue.
    /// </summary>
    public class CityMemory
    {
        private const string LogPrefix = "[Memory] ";

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly ICityStore _store;
        private readonly List<Idea> _ideas = new List<Idea>();
        private readonly HashSet<string> _mutedAlerts = new HashSet<string>();

        private string _cityKey = string.Empty;
        private CityData _data = new CityData();

        public CityMemory(ICityStore store)
        {
            _store = store;
        }

        /// <summary>True once a city has been opened and its file read.</summary>
        public bool IsLoaded { get; private set; }

        public string CityKey
        {
            get { return _cityKey; }
        }

        /// <summary>Newest first: the thought you just had is the one you are most likely to want.</summary>
        public IList<Idea> Ideas
        {
            get { return _ideas; }
        }

        /// <summary>Alert ids the player asked to stop hearing about, in this city.</summary>
        public ICollection<string> MutedAlerts
        {
            get { return _mutedAlerts; }
        }

        /// <summary>When this city was last open. Zero when it has never been recorded.</summary>
        public long LastSeenUnixUtc
        {
            get { return _data.LastSeenUnixUtc; }
        }

        public void Open(string cityKey, string cityName)
        {
            Close();

            _cityKey = cityKey;
            _data = _store.Load(cityKey);
            _data.CityName = cityName ?? string.Empty;

            foreach (var stored in _data.Ideas)
            {
                _ideas.Add(new Idea(
                    string.IsNullOrEmpty(stored.Id) ? Guid.NewGuid().ToString("N") : stored.Id,
                    (IdeaCategory)stored.Category,
                    stored.Note,
                    stored.Description,
                    new WorldPoint(stored.X, stored.Y, stored.Z),
                    FromUnix(stored.CreatedUnixUtc)));
            }

            foreach (var id in _data.MutedAlerts)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    _mutedAlerts.Add(id);
                }
            }

            _ideas.Sort(NewestFirst);
            IsLoaded = true;

            Mod.Log.Info(LogPrefix + "Opened '" + cityKey + "': " + _ideas.Count + " parked ideas, " +
                         _mutedAlerts.Count + " muted alerts.");
        }

        public void Close()
        {
            _ideas.Clear();
            _mutedAlerts.Clear();
            _cityKey = string.Empty;
            _data = new CityData();
            IsLoaded = false;
        }

        // ---- Ideas --------------------------------------------------------------------------

        public Idea Park(WorldPoint position, IdeaCategory category)
        {
            var idea = Idea.Create(position, category);
            _ideas.Insert(0, idea);
            Flush();

            Mod.Log.Info(LogPrefix + "Marker created at " + position + " (" + category + ", id " + idea.Id + ").");
            return idea;
        }

        public bool Forget(string id)
        {
            for (var i = 0; i < _ideas.Count; i++)
            {
                if (_ideas[i].Id != id)
                {
                    continue;
                }

                _ideas.RemoveAt(i);
                Flush();
                Mod.Log.Info(LogPrefix + "Forgot idea " + id + ".");
                return true;
            }

            return false;
        }

        public Idea FindIdea(string id)
        {
            foreach (var idea in _ideas)
            {
                if (idea.Id == id)
                {
                    return idea;
                }
            }

            return null;
        }

        public bool SetNote(string id, string note)
        {
            var idea = FindIdea(id);
            if (idea == null)
            {
                return false;
            }

            idea.Note = note ?? string.Empty;
            Flush();
            return true;
        }

        /// <summary>
        /// The long version. Empty is a perfectly good value and clears it - a player who deletes
        /// what they wrote has decided the idea says enough on its own, which is not an error.
        /// </summary>
        public bool SetDescription(string id, string description)
        {
            var idea = FindIdea(id);
            if (idea == null)
            {
                return false;
            }

            idea.Description = description ?? string.Empty;
            Flush();
            return true;
        }

        public bool SetCategory(string id, IdeaCategory category)
        {
            var idea = FindIdea(id);
            if (idea == null)
            {
                return false;
            }

            idea.Category = category;
            Flush();
            return true;
        }

        // ---- Alerts -------------------------------------------------------------------------

        /// <summary>
        /// Silences one alert in this city, or brings it back.
        ///
        /// Muting is the player saying "I know, and I am choosing not to look at it" - which is a
        /// legitimate answer that the mod respects and remembers. It is not a promise to do
        /// anything about it later, and nothing ever asks them about it again.
        /// </summary>
        public bool SetAlertMuted(string alertId, bool muted)
        {
            if (string.IsNullOrEmpty(alertId))
            {
                return false;
            }

            var changed = muted ? _mutedAlerts.Add(alertId) : _mutedAlerts.Remove(alertId);
            if (changed)
            {
                Flush();
                Mod.Log.Info(LogPrefix + (muted ? "Muted " : "Unmuted ") + alertId + ".");
            }

            return changed;
        }

        public bool IsAlertMuted(string alertId)
        {
            return _mutedAlerts.Contains(alertId);
        }

        // ---- Safety net ---------------------------------------------------------------------

        /// <summary>The save this city's safety net points at, or empty if there is none.</summary>
        public string CheckpointName
        {
            get { return _data.CheckpointName ?? string.Empty; }
        }

        public long CheckpointUnixUtc
        {
            get { return _data.CheckpointUnixUtc; }
        }

        public bool HasCheckpoint
        {
            get { return !string.IsNullOrEmpty(CheckpointName); }
        }

        public void SetCheckpoint(string saveName, DateTime takenUtc)
        {
            _data.CheckpointName = saveName ?? string.Empty;
            _data.CheckpointUnixUtc = ToUnix(takenUtc);
            Flush();
        }

        public void ClearCheckpoint()
        {
            _data.CheckpointName = string.Empty;
            _data.CheckpointUnixUtc = 0;
            Flush();
        }

        // ---- Session ------------------------------------------------------------------------

        /// <summary>The city's display name at the time it was opened.</summary>
        public string CityName
        {
            get { return _data.CityName; }
        }

        /// <summary>Where the player was standing when this city was last flushed, if anywhere.</summary>
        public bool TryGetLastCamera(out WorldPoint point)
        {
            if (!_data.HasLastCamera)
            {
                point = default(WorldPoint);
                return false;
            }

            point = new WorldPoint(_data.LastCameraX, _data.LastCameraY, _data.LastCameraZ);
            return true;
        }

        /// <summary>Records where the player was, for Welcome Back. Written with the next flush.</summary>
        public void RememberCamera(WorldPoint position)
        {
            _data.LastCameraX = position.X;
            _data.LastCameraY = position.Y;
            _data.LastCameraZ = position.Z;
            _data.HasLastCamera = true;
        }

        /// <summary>Writes the file. Called after every change; the file is a few kilobytes.</summary>
        public void Flush()
        {
            if (string.IsNullOrEmpty(_cityKey))
            {
                return;
            }

            _data.LastSeenUnixUtc = ToUnix(DateTime.UtcNow);

            _data.Ideas = new List<StoredIdea>(_ideas.Count);
            foreach (var idea in _ideas)
            {
                _data.Ideas.Add(new StoredIdea
                {
                    Id = idea.Id,
                    Category = (int)idea.Category,
                    Note = idea.Note,
                    Description = idea.Description,
                    X = idea.Position.X,
                    Y = idea.Position.Y,
                    Z = idea.Position.Z,
                    CreatedUnixUtc = ToUnix(idea.CreatedUtc)
                });
            }

            _data.MutedAlerts = new List<string>(_mutedAlerts);

            // CheckpointName / CheckpointUnixUtc are written straight onto _data by SetCheckpoint,
            // so they need no rebuilding here - they are already what should go to disk.

            _store.Save(_cityKey, _data);
        }

        private static int NewestFirst(Idea a, Idea b)
        {
            return b.CreatedUtc.CompareTo(a.CreatedUtc);
        }

        public static long ToUnix(DateTime utc)
        {
            return (long)(utc - Epoch).TotalSeconds;
        }

        public static DateTime FromUnix(long seconds)
        {
            return Epoch.AddSeconds(seconds);
        }
    }
}

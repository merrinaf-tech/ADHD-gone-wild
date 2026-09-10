using System.Collections.Generic;
using ADHDGoneWild.Core;

namespace ADHDGoneWild.Alerts
{
    /// <summary>
    /// Counts the game's own in-world notification icons and turns each kind into one alert.
    ///
    /// This is the noise reduction the mod exists for, in its plainest form. The game will put a
    /// bin icon over forty buildings; forty rows is not information. One row saying "40 buildings"
    /// carries the same fact and can be read without scrolling.
    ///
    /// Pure and reusable: the caller feeds it a name, a priority and a place. Getting those out
    /// of ECS is somebody else's job, which is what makes the counting and grouping testable.
    /// </summary>
    public class NotificationTally
    {
        public const string IdPrefix = "notify.";

        private readonly Dictionary<string, Entry> _byName = new Dictionary<string, Entry>();

        private class Entry
        {
            public string Name;
            public int Count;
            public int Priority;
            public readonly List<WorldPoint> Locations = new List<WorldPoint>();
        }

        public int DistinctKinds
        {
            get { return _byName.Count; }
        }

        public void Reset()
        {
            _byName.Clear();
        }

        /// <summary>
        /// One icon seen in the world. <paramref name="priority"/> is the game's own
        /// Game.Notifications.IconPriority value.
        /// </summary>
        public void Add(string prefabName, int priority, WorldPoint location)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                // An icon whose prefab could not be resolved is still an icon the player can see,
                // so it is counted rather than dropped - just without a name of its own.
                prefabName = "Unknown";
            }

            Entry entry;
            if (!_byName.TryGetValue(prefabName, out entry))
            {
                entry = new Entry { Name = prefabName, Priority = priority };
                _byName.Add(prefabName, entry);
            }

            entry.Count++;

            // The worst instance sets the tone for the group. One building actually on fire is
            // not made less urgent by nine that are merely at risk.
            if (priority > entry.Priority)
            {
                entry.Priority = priority;
            }

            if (entry.Locations.Count < AlertAggregator.MaxLocationsPerAlert)
            {
                entry.Locations.Add(location);
            }
        }

        public void Fill(List<Alert> into)
        {
            foreach (var entry in _byName.Values)
            {
                var status = AlertClassifier.ClassifyNotification(entry.Priority);
                var subject = NotificationSubjects.For(entry.Name);

                var alert = new Alert(
                    IdPrefix + entry.Name,
                    subject,
                    status,
                    AlertTitles.KeyForNotification(entry.Name))
                {
                    RawName = Readable(entry.Name),
                    // The game's own marker for this notification, so the panel row and the icons
                    // over the buildings read as the same thing.
                    Icon = AlertIcons.ForNotification(entry.Name),
                    AffectedCount = entry.Count
                };

                alert.Locations.AddRange(entry.Locations);
                into.Add(alert);
            }
        }

        /// <summary>
        /// Prefab names arrive as the game stores them. Most already read as English; this only
        /// tidies the ones written without spaces, so the fallback label is never a wall of
        /// capitals.
        /// </summary>
        private static string Readable(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName) || prefabName.IndexOf(' ') >= 0)
            {
                return prefabName;
            }

            var text = new System.Text.StringBuilder(prefabName.Length + 8);
            for (var i = 0; i < prefabName.Length; i++)
            {
                var c = prefabName[i];
                if (i > 0 && char.IsUpper(c) && !char.IsUpper(prefabName[i - 1]))
                {
                    text.Append(' ');
                }

                text.Append(c);
            }

            return text.ToString();
        }
    }
}

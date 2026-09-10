using System.Collections.Generic;
using ADHDGoneWild.Core;

namespace ADHDGoneWild.Alerts
{
    /// <summary>
    /// Everything that happens between "here is what the collectors found" and "here is what the
    /// player sees": merging duplicates, honouring what they muted, dropping what they asked not
    /// to see, and ordering the rest.
    ///
    /// This is where the mod earns its keep. Cities: Skylines II will happily report the same
    /// problem forty times; a list of forty rows is not information, it is noise wearing
    /// information's clothes. One row saying "40 buildings" is the same fact, and the player can
    /// read it in a glance instead of a scroll.
    ///
    /// Pure. No game, no UI, no I/O.
    /// </summary>
    public static class AlertAggregator
    {
        /// <summary>
        /// How many places are kept per alert. The "view" button visits them one at a time, so
        /// the rest are memory spent on nothing - and a city in real trouble can have thousands.
        /// </summary>
        public const int MaxLocationsPerAlert = 24;

        /// <summary>
        /// Builds what the panel shows.
        ///
        /// <paramref name="muted"/> are ids the player silenced. They are not discarded: a muted
        /// alert drops to the muted status and is counted, so "you asked me to be quiet about
        /// three things" stays answerable. <paramref name="floor"/> is the least severe status
        /// the player wants on screen at all.
        /// </summary>
        public static AlertSet Build(IEnumerable<Alert> collected, ICollection<string> muted, Status floor)
        {
            var merged = new Dictionary<string, Alert>();
            var order = new List<string>();

            foreach (var alert in collected)
            {
                if (alert == null)
                {
                    continue;
                }

                // Nothing to say about this network or building type right now.
                if (alert.Status == Status.Resolved)
                {
                    continue;
                }

                Alert existing;
                if (merged.TryGetValue(alert.Id, out existing))
                {
                    Merge(existing, alert);
                    continue;
                }

                Trim(alert);
                merged.Add(alert.Id, alert);
                order.Add(alert.Id);
            }

            var visible = new List<Alert>(order.Count);

            // Kept, not counted and thrown away. The panel offers to undo a mute, and it cannot
            // offer that for something it no longer holds.
            var silenced = new List<Alert>();

            foreach (var id in order)
            {
                var alert = merged[id];

                if (muted != null && muted.Contains(alert.Id))
                {
                    alert.Status = Status.Muted;
                    silenced.Add(alert);
                    continue;
                }

                // Status is an ordered vocabulary: Immediate 0 .. Muted 3. "At least as serious
                // as the floor" is therefore a numeric comparison, not a table.
                if ((int)alert.Status > (int)floor)
                {
                    continue;
                }

                visible.Add(alert);
            }

            visible.Sort(Ranking);
            silenced.Sort(Ranking);

            return new AlertSet(visible, silenced);
        }

        private static void Merge(Alert into, Alert other)
        {
            // Two collectors reporting the same id means the same situation seen twice. The more
            // serious reading wins, because the less serious one is not evidence that the other
            // is wrong.
            if ((int)other.Status < (int)into.Status)
            {
                into.Status = other.Status;
            }

            into.AffectedCount += other.AffectedCount;

            foreach (var location in other.Locations)
            {
                if (into.Locations.Count >= MaxLocationsPerAlert)
                {
                    break;
                }

                into.Locations.Add(location);
            }
        }

        private static void Trim(Alert alert)
        {
            if (alert.Locations.Count > MaxLocationsPerAlert)
            {
                alert.Locations.RemoveRange(MaxLocationsPerAlert, alert.Locations.Count - MaxLocationsPerAlert);
            }
        }

        /// <summary>
        /// Most serious first; within a status, whatever touches more of the city first; then by
        /// id so the list does not reshuffle itself under the player's cursor between refreshes.
        /// </summary>
        private static int Ranking(Alert a, Alert b)
        {
            var byStatus = ((int)a.Status).CompareTo((int)b.Status);
            if (byStatus != 0)
            {
                return byStatus;
            }

            var byCount = b.AffectedCount.CompareTo(a.AffectedCount);
            return byCount != 0 ? byCount : string.CompareOrdinal(a.Id, b.Id);
        }
    }
}

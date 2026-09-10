using System.Collections.Generic;
using ADHDGoneWild.Core;

namespace ADHDGoneWild.Alerts
{
    /// <summary>
    /// What the player is shown: the alerts that survived filtering, and how many of each kind
    /// there are.
    ///
    /// The counts exist so the compact view can say "1 immediate, 2 important" and stop there,
    /// with the rest one click away. They are counts of what is true about the city, which is why
    /// they are allowed to be numbers at all - a count of things the player has not dealt with
    /// would not be.
    /// </summary>
    public class AlertSet
    {
        public static readonly AlertSet Empty = new AlertSet(new List<Alert>(), new List<Alert>());

        public AlertSet(List<Alert> visible, List<Alert> muted)
        {
            Visible = visible;
            Muted = muted ?? new List<Alert>();

            foreach (var alert in visible)
            {
                switch (alert.Status)
                {
                    case Status.Immediate:
                        ImmediateCount++;
                        break;
                    case Status.Important:
                        ImportantCount++;
                        break;
                    case Status.Monitor:
                        MonitorCount++;
                        break;
                }
            }
        }

        /// <summary>Ordered most immediate first. See <see cref="AlertAggregator"/>.</summary>
        public List<Alert> Visible { get; private set; }

        public int ImmediateCount { get; private set; }
        public int ImportantCount { get; private set; }
        public int MonitorCount { get; private set; }

        /// <summary>
        /// Silenced by the player, and kept rather than discarded. Muting hides noise, not facts -
        /// and a mute the player cannot find again is not a mute, it is a leak.
        /// </summary>
        public List<Alert> Muted { get; private set; }

        public int MutedCount
        {
            get { return Muted.Count; }
        }

        public int TotalVisible
        {
            get { return Visible.Count; }
        }

        /// <summary>The worst thing currently true, for the one-glance indicator.</summary>
        public Status Peak
        {
            get
            {
                if (ImmediateCount > 0)
                {
                    return Status.Immediate;
                }

                if (ImportantCount > 0)
                {
                    return Status.Important;
                }

                return MonitorCount > 0 ? Status.Monitor : Status.Resolved;
            }
        }
    }
}

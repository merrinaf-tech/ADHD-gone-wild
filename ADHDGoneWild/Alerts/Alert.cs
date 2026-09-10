using System.Collections.Generic;
using ADHDGoneWild.Core;

namespace ADHDGoneWild.Alerts
{
    /// <summary>
    /// One thing that is true about the city right now.
    ///
    /// An alert is a statement of state, never an instruction. "Electricity demand is higher than
    /// production" belongs here; "build a power plant" does not, and there is deliberately
    /// nowhere in this class to put it. The player keeps the decision.
    ///
    /// Nothing here is a task either: an alert has no owner, no age, no progress and no way to be
    /// completed. It exists while the city is in that state and stops existing when it is not.
    /// </summary>
    public class Alert
    {
        /// <summary>
        /// Stable across refreshes for the same situation, so muting one thing does not silence
        /// something else next tick, and so the UI can keep its rows still while numbers change.
        /// </summary>
        public string Id { get; private set; }

        public AlertSubject Subject { get; private set; }

        public Status Status { get; set; }

        /// <summary>Localisation key for the sentence shown to the player.</summary>
        public string TitleKey { get; private set; }

        /// <summary>
        /// The game's own name for this, used when the mod has no localised sentence of its own -
        /// which is how an unrecognised notification type still says something true.
        /// </summary>
        public string RawName { get; set; }

        /// <summary>The game's own icon for this. See <see cref="AlertIcons"/>.</summary>
        public string Icon { get; set; }

        /// <summary>
        /// How many buildings are in this state. Zero means the alert is about the city as a
        /// whole and there is nothing to count - not that nothing is affected.
        /// </summary>
        public int AffectedCount { get; set; }

        /// <summary>
        /// Places the player can be taken to. Bounded by the collector: a list of six thousand
        /// positions would cost memory to build a "view" button that only ever uses one at a time.
        /// </summary>
        public List<WorldPoint> Locations { get; private set; }

        public Alert(string id, AlertSubject subject, Status status, string titleKey)
        {
            Id = id;
            Subject = subject;
            Status = status;
            TitleKey = titleKey;
            RawName = string.Empty;
            Icon = AlertIcons.ForSubject(subject);
            Locations = new List<WorldPoint>();
        }

        public override string ToString()
        {
            return Status + " " + Subject + " (" + Id + ", " + AffectedCount + " affected)";
        }
    }
}

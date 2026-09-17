using System;

namespace ADHDGoneWild.Trail
{
    /// <summary>
    /// What the player was doing somewhere, in the plainest words available.
    ///
    /// These are verbs about observable actions - which tool was in their hand - and nothing else.
    /// They are deliberately not "project", "task", "unfinished" or "objective": the mod can see
    /// that a road tool was active at a place, and it cannot see why, so it says only the first.
    ///
    /// Numbered, and crossing the bridge to the UI, so entries are appended and never renumbered.
    /// </summary>
    public enum TrailActivity
    {
        /// <summary>No tool in hand. Looking, selecting, reading a panel. Still attention.</summary>
        Looking = 0,

        /// <summary>Roads, tracks, pipes, power lines - anything drawn as a network.</summary>
        Drawing = 1,

        /// <summary>Placing a building, a prop, a tree.</summary>
        Placing = 2,

        Zoning = 3,
        Demolishing = 4,
        Landscaping = 5,

        /// <summary>Districts, map tiles, lots.</summary>
        Areas = 6,

        /// <summary>Transport lines.</summary>
        Lines = 7,

        Upgrading = 8,
    }

    /// <summary>
    /// Turning the game's tool into one of the words above. Pure, and total: every input has an
    /// answer, so an unrecognised tool costs the label its precision and never the entry.
    /// </summary>
    public static class TrailActivities
    {
        /// <summary>Every value, in declaration order. The UI list is built from this.</summary>
        public static readonly TrailActivity[] All =
        {
            TrailActivity.Looking,
            TrailActivity.Drawing,
            TrailActivity.Placing,
            TrailActivity.Zoning,
            TrailActivity.Demolishing,
            TrailActivity.Landscaping,
            TrailActivity.Areas,
            TrailActivity.Lines,
            TrailActivity.Upgrading,
        };

        /// <summary>
        /// Matched on the tool system's type name rather than on its <c>toolID</c>.
        ///
        /// Both are strings the game owns, but the type names are readable straight out of
        /// Game.dll and were checked there; the toolID values live inside property bodies, which
        /// reflection cannot show - so using them would have meant guessing, which this project
        /// does not do with API names.
        ///
        /// Matched on the leading word rather than anywhere in the name, and that is not fussiness:
        /// written as a substring search, "ZoneToolSystem" matched "Net" - the "neT" in the middle
        /// of it - and every minute of zoning was filed as road work. A test caught it; a player
        /// would have had to notice a label was quietly wrong.
        ///
        /// Still a word match rather than a table of exact names, for the same reason
        /// <see cref="Alerts.NotificationSubjects"/> uses one: a renamed or newly added tool then
        /// lands on <see cref="TrailActivity.Looking"/> instead of throwing the place away.
        /// </summary>
        public static TrailActivity From(string toolTypeName)
        {
            var name = Simple(toolTypeName);

            if (name.Length == 0)
            {
                return TrailActivity.Looking;
            }

            if (Has(name, "Net")) return TrailActivity.Drawing;
            if (Has(name, "Object")) return TrailActivity.Placing;
            if (Has(name, "Zone")) return TrailActivity.Zoning;
            if (Has(name, "Bulldoze")) return TrailActivity.Demolishing;
            if (Has(name, "Terrain")) return TrailActivity.Landscaping;
            if (Has(name, "Water")) return TrailActivity.Landscaping;
            if (Has(name, "Area")) return TrailActivity.Areas;
            if (Has(name, "Route")) return TrailActivity.Lines;
            if (Has(name, "Upgrade")) return TrailActivity.Upgrading;

            // DefaultToolSystem and SelectionToolSystem both land here, which is right: neither
            // changes the city.
            return TrailActivity.Looking;
        }

        /// <summary>One activity as a bit, so a place can carry every kind it saw.</summary>
        public static int Bit(TrailActivity activity)
        {
            return 1 << (int)activity;
        }

        public static bool IsIn(int mask, TrailActivity activity)
        {
            return (mask & Bit(activity)) != 0;
        }

        private static bool Has(string name, string word)
        {
            return name.StartsWith(word, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>The type's own name, whether it arrives qualified or not.</summary>
        private static string Simple(string toolTypeName)
        {
            if (string.IsNullOrEmpty(toolTypeName))
            {
                return string.Empty;
            }

            var dot = toolTypeName.LastIndexOf('.');
            return dot < 0 ? toolTypeName : toolTypeName.Substring(dot + 1);
        }
    }
}

namespace ADHDGoneWild.BrainParking
{
    /// <summary>
    /// A colour per kind of idea, for the rings drawn on the map.
    ///
    /// This is a deliberate, scoped exception to the rule that colour means status and the icon
    /// means subject. Every parked idea has the same status - it is the player's own - so within
    /// that one family there is no severity for hue to carry, and using it to tell a transport
    /// thought from a decoration one at a glance costs nothing. The rule still holds where it
    /// matters: nothing in the alert list is ever coloured by its subject.
    ///
    /// Kept in step by hand with UI/src/theme/tokens.ts, since the rings are drawn by the engine
    /// and the panel by the browser. The values are the one thing that has to exist in both.
    /// </summary>
    public static class IdeaColours
    {
        /// <summary>Red, green and blue in 0..1, matching UnityEngine.Color.</summary>
        public static void Get(IdeaCategory category, out float r, out float g, out float b)
        {
            switch (category)
            {
                case IdeaCategory.Build:
                    // Amber
                    r = 1f; g = 0.710f; b = 0.278f;
                    return;
                case IdeaCategory.Decoration:
                    // Green
                    r = 0.435f; g = 0.831f; b = 0.494f;
                    return;
                case IdeaCategory.Transport:
                    // Blue
                    r = 0.345f; g = 0.714f; b = 1f;
                    return;
                case IdeaCategory.Fix:
                    // Coral
                    r = 1f; g = 0.541f; b = 0.361f;
                    return;
                case IdeaCategory.Other:
                    // Grey
                    r = 0.725f; g = 0.761f; b = 0.800f;
                    return;
                default:
                    // Idea - the personal purple the rest of the mod uses for anything the player made.
                    r = 0.725f; g = 0.549f; b = 1f;
                    return;
            }
        }
    }
}

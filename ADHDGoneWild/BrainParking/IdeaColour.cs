namespace ADHDGoneWild.BrainParking
{
    /// <summary>
    /// A colour for one bullet in an idea's description. Numbered, written to disk inside the
    /// description text, and crossing the bridge to the UI: appended to, never renumbered.
    /// </summary>
    public enum IdeaColour
    {
        /// <summary>No colour of its own - the plain bullet every line starts with.</summary>
        Plain = 0,

        Purple = 1,
        Blue = 2,
        Green = 3,
        Yellow = 4,
        Orange = 5,
        Red = 6,
        Pink = 7,
        Grey = 8,
    }

    /// <summary>
    /// The colours a bullet can be given, and the reason they exist.
    ///
    /// <para>
    /// This started as a way to recolour a whole idea and that was the wrong shape. Colouring the
    /// idea itself means the words go coloured too, and a list where every line is a different
    /// colour of prose is harder to read than one that is all white - it spends the eye's whole
    /// budget before a single word has been understood.
    /// </para>
    ///
    /// <para>
    /// So the colour moved off the text and onto a dot beside it. The words stay in the panel's
    /// ordinary ink, the idea keeps the colour of its category, and the coloured dots become
    /// bullets in the description - a way to group the lines of one idea by whatever the player
    /// decides the colours mean. Sorting without shouting.
    /// </para>
    ///
    /// <para>
    /// A fixed set rather than a colour wheel. Eight are enough to tell groups apart at a glance,
    /// they are chosen to sit on the panel's own dark blue, and picking from a row of swatches is
    /// one click where a wheel is a small task in itself.
    /// </para>
    /// </summary>
    public static class IdeaPalette
    {
        /// <summary>In the order the swatches appear. Plain is offered first, as the way back.</summary>
        public static readonly IdeaColour[] Order =
        {
            IdeaColour.Plain,
            IdeaColour.Purple,
            IdeaColour.Blue,
            IdeaColour.Green,
            IdeaColour.Yellow,
            IdeaColour.Orange,
            IdeaColour.Red,
            IdeaColour.Pink,
            IdeaColour.Grey,
        };

        /// <summary>
        /// The colour in 0..1 RGB, or false for a plain bullet with no colour of its own.
        ///
        /// Kept in step by hand with UI/src/theme/tokens.ts. Nothing on the C# side draws these
        /// today - the bullets live in the panel - but the values belong with the enum that names
        /// them, and the day a coloured dot is wanted on the map they will already be right.
        /// </summary>
        public static bool TryGet(IdeaColour colour, out float r, out float g, out float b)
        {
            switch (colour)
            {
                case IdeaColour.Purple:
                    // #b98cff - the personal purple the rest of the mod uses for what the player made.
                    r = 0.725f; g = 0.549f; b = 1f;
                    return true;

                case IdeaColour.Blue:
                    // #58b6ff
                    r = 0.345f; g = 0.714f; b = 1f;
                    return true;

                case IdeaColour.Green:
                    // #6fd47e
                    r = 0.435f; g = 0.831f; b = 0.494f;
                    return true;

                case IdeaColour.Yellow:
                    // #ffd93d
                    r = 1f; g = 0.851f; b = 0.239f;
                    return true;

                case IdeaColour.Orange:
                    // #ff9d2e
                    r = 1f; g = 0.616f; b = 0.180f;
                    return true;

                case IdeaColour.Red:
                    // #ff6b6b - lighter and pinker than the #ff4d4d an immediate alert uses, so
                    // that a red idea does not read as a red warning at a glance.
                    r = 1f; g = 0.420f; b = 0.420f;
                    return true;

                case IdeaColour.Pink:
                    // #ff8ad4
                    r = 1f; g = 0.541f; b = 0.831f;
                    return true;

                case IdeaColour.Grey:
                    // #b9c2cc
                    r = 0.725f; g = 0.761f; b = 0.800f;
                    return true;

                case IdeaColour.Plain:
                default:
                    r = 0f; g = 0f; b = 0f;
                    return false;
            }
        }

        /// <summary>
        /// Anything unrecognised becomes <see cref="IdeaColour.Plain"/>.
        ///
        /// The value comes out of a description written by some other build - an older one, or a
        /// newer one after a downgrade. A plain bullet means the line still reads exactly as it
        /// was written, which is the only part that actually matters.
        /// </summary>
        public static IdeaColour Parse(int stored)
        {
            switch ((IdeaColour)stored)
            {
                case IdeaColour.Purple:
                case IdeaColour.Blue:
                case IdeaColour.Green:
                case IdeaColour.Yellow:
                case IdeaColour.Orange:
                case IdeaColour.Red:
                case IdeaColour.Pink:
                case IdeaColour.Grey:
                    return (IdeaColour)stored;

                default:
                    return IdeaColour.Plain;
            }
        }
    }
}

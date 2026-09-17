namespace ADHDGoneWild.Wellbeing
{
    /// <summary>
    /// Where the clock card appears. Numbered, and crossing the bridge to the UI, so entries are
    /// appended and never renumbered.
    /// </summary>
    public enum HyperfocusCorner
    {
        BottomLeft = 0,
        TopRight = 1,
        BottomRight = 2,
    }

    /// <summary>
    /// Which corner the next card goes in.
    ///
    /// This exists because a player was right and this project's own rule was wrong. The rule was
    /// "a thing that moves is a thing you have to find again", and it is a good rule - for a
    /// button you go looking for. The clock card is the opposite job: nobody goes looking for it,
    /// its entire purpose is to be noticed, and anything that appears in the same place every
    /// time stops being seen after the fourth or fifth appearance. As AnnieXfloop put it: variety
    /// keeps attention.
    ///
    /// <para>
    /// It rotates rather than picking at random, and that is the point rather than laziness.
    /// Random repeats: roll three corners twice and roughly a third of the time the card lands
    /// where it just was, which is precisely the case the feature exists to avoid. Rotating
    /// guarantees every card is somewhere the last one was not, and it stays predictable enough
    /// that somebody who dismissed one by accident can reason about where the next will be.
    /// </para>
    ///
    /// <para>
    /// Three corners, not four. Top-left is where this mod's own panel opens, and a card landing
    /// on top of the player's own open panel would be the fourth time a floating thing in this
    /// mod has been put somewhere it collides with something - see the fold control's history in
    /// Calm/toolbar-fold. The corner is left empty on purpose.
    /// </para>
    /// </summary>
    public static class HyperfocusCorners
    {
        /// <summary>In rotation order. The first card of a session is bottom left, where the card has always been.</summary>
        public static readonly HyperfocusCorner[] Order =
        {
            HyperfocusCorner.BottomLeft,
            HyperfocusCorner.TopRight,
            HyperfocusCorner.BottomRight,
        };

        /// <summary>
        /// The corner for the nth card of this run, counting from zero.
        ///
        /// Negative counts are folded back rather than throwing: the caller is a counter that has
        /// no business being negative, and a card in the wrong corner is a better failure than no
        /// card at all.
        /// </summary>
        public static HyperfocusCorner For(int shownCount)
        {
            var index = shownCount % Order.Length;

            if (index < 0)
            {
                index += Order.Length;
            }

            return Order[index];
        }
    }
}

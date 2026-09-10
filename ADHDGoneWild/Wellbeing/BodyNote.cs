namespace ADHDGoneWild.Wellbeing
{
    /// <summary>
    /// The small second line on the hyperfocus card: a nod towards the body, which is the other
    /// thing hyperfocus quietly switches off.
    ///
    /// These are written as direct, warm, slightly daft asks. They began as understatement -
    /// "Standing up is allowed" - on the theory that an imperative would provoke resistance.
    /// In game that register read as passive-aggressive: saying a thing sideways sounds like a
    /// comment on the person, and dry understatement lands as a raised eyebrow. Being plainly
    /// asked by something obviously on your side is kinder than being hinted at.
    ///
    /// The line that still holds is not "no imperatives" - it is "no jabs". Nothing here may be
    /// at the player's expense, imply they have let themselves go, or carry a number.
    ///
    /// Nothing here is ever followed up on. The mod does not ask whether the player drank
    /// anything, does not count how many notes were ignored, and cannot tell the difference
    /// between a note that worked and one that did not. That ignorance is deliberate: the moment
    /// it could tell, it would be a habit tracker.
    /// </summary>
    public enum BodyNote
    {
        Water = 0,
        Food = 1,
        Stand = 2,
        Eyes = 3,
        Shoulders = 4,

        /// <summary>
        /// Only offered in the small hours, and the only one that is about the clock rather than
        /// the body. It is reassurance, not a suggestion to stop - the reason people keep going
        /// at 3am is rarely that they forgot they could stop.
        /// </summary>
        LateHour = 5
    }
}

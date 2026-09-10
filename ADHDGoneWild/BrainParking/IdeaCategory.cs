namespace ADHDGoneWild.BrainParking
{
    /// <summary>
    /// What kind of thought this was, roughly.
    ///
    /// Chosen after the fact, never before: parking an idea takes a shortcut and a click, and
    /// lands here as <see cref="Idea"/> with no decision asked for. The list exists so a player
    /// who wants to sort their own markers later can, not so the mod can ask them a question
    /// mid-thought.
    ///
    /// The numbers are written to disk. Append, never renumber.
    /// </summary>
    public enum IdeaCategory
    {
        Idea = 0,
        Build = 1,
        Decoration = 2,
        Transport = 3,
        Fix = 4,
        Other = 5
    }
}

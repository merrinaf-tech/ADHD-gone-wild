namespace ADHDGoneWild.Alerts
{
    /// <summary>
    /// What an alert is about. Never how bad it is - that is <see cref="Core.Status"/>, and the
    /// two are decided separately on purpose.
    ///
    /// The icon in the UI comes from here. The colour comes from the status. So electricity is
    /// not "yellow": an electricity problem worth watching is.
    ///
    /// <see cref="Other"/> is not a failure. The game has many notification types and this list
    /// only names the ones the mod can speak about with confidence; anything else still appears,
    /// still carries its real name from the game, and simply borrows a generic icon. A subject
    /// the mod has not heard of must never vanish.
    /// </summary>
    public enum AlertSubject
    {
        Other = 0,
        Electricity = 1,
        Water = 2,
        Sewage = 3,
        Garbage = 4,
        Health = 5,
        Deathcare = 6,
        Fire = 7,
        Police = 8,
        Education = 9,
        Transport = 10,
        Mail = 11,
        Traffic = 12,
        Housing = 13,
        Workplace = 14
    }
}

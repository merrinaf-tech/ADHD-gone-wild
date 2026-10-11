namespace ADHDGoneWild.Alerts
{
    /// <summary>
    /// What a notification icon is standing on. The game puts the same kind of warning over a
    /// building, a car or a stretch of road, and "3 buildings" is only true when they are.
    ///
    /// The values are indexes into <see cref="Alert.CasesByKind"/> and are written to the UI in
    /// this order, so the two sides must change together.
    /// </summary>
    public enum CaseKind
    {
        Building = 0,
        Vehicle = 1,
        Road = 2,

        /// <summary>Anything else, or an icon whose owner could not be resolved.</summary>
        Other = 3
    }

    public static class CaseKinds
    {
        public const int Count = 4;
    }
}

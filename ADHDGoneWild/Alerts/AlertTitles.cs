namespace ADHDGoneWild.Alerts
{
    /// <summary>
    /// The localisation key for an alert, built from what it is about and how serious it is.
    ///
    /// Generated rather than tabulated, so adding a subject does not mean editing a lookup in
    /// three places. A key with no translation behind it is not an error: the UI falls back to
    /// the game's own name for the thing, which is always true even when the mod has nothing
    /// better to say about it.
    ///
    /// Every sentence these resolve to is a statement about the city. None of them is an
    /// instruction - see docs/PRODUCT_PRINCIPLES.md.
    /// </summary>
    public static class AlertTitles
    {
        public const string Prefix = "ADHDGoneWild.Alert.";
        public const string NotificationPrefix = "ADHDGoneWild.Notification.";

        public static string KeyFor(AlertSubject subject, Core.Status status)
        {
            return Prefix + subject + "." + status;
        }

        /// <summary>
        /// The key for one of the game's own notification types.
        ///
        /// Deliberately its own namespace rather than the subject/status key above. A notification
        /// says something specific about specific buildings - "garbage not collected here" - and
        /// must never inherit the city-wide sentence for the same subject, which would be a
        /// different and possibly untrue claim.
        ///
        /// Nothing translates these yet, so they fall through to the game's own name, which is
        /// always true. Translating a common one later needs no code change.
        /// </summary>
        public static string KeyForNotification(string prefabName)
        {
            return NotificationPrefix + (prefabName ?? string.Empty).Replace(" ", string.Empty);
        }
    }
}

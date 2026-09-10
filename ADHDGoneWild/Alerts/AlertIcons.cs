using System.Collections.Generic;

namespace ADHDGoneWild.Alerts
{
    /// <summary>
    /// The game's own icon for an alert.
    ///
    /// Drawing our own turned out to be a mistake: hand-made glyphs at 15px read as smudges next
    /// to the vanilla interface, and a player scanning the screen has to learn a second visual
    /// vocabulary for no benefit. The game already ships an icon for every one of these things,
    /// the player already knows them, and they are drawn for exactly this size and background.
    ///
    /// Paths are relative, with no protocol prefix - that is how the game resolves its own UI
    /// media, confirmed by another working mod in this workspace.
    ///
    /// Nothing here is guaranteed to exist: the UI falls back to <see cref="Fallback"/> when an
    /// image fails to load, so a wrong or removed filename costs an icon, never a row.
    /// </summary>
    public static class AlertIcons
    {
        /// <summary>The generic "something is happening" marker, used when nothing better loads.</summary>
        public const string Fallback = "Media/Game/Icons/Notifications.svg";

        private const string IconRoot = "Media/Game/Icons/";
        private const string NotificationRoot = "Media/Game/Notifications/";

        /// <summary>
        /// Notification prefabs whose icon file is not simply the prefab name without spaces.
        /// Established by inspecting the shipped UI media - see the note in docs/TECHNICAL_FINDINGS.md.
        /// </summary>
        private static readonly Dictionary<string, string> FileAliases = new Dictionary<string, string>
        {
            { "ElectricityNotification", "NotEnoughElectricity" },
            { "WaterNotification", "NoRunningWater" },
            { "SewageNotification", "Sewage" },
            { "GarbageNotification", "TooMuchGarbage" },
            { "TelecomNotification", "NoTelecomCoverage" },
            { "HearseNotification", "HearseServiceNeeded" },
            { "TrafficBottleneckNotification", "TrafficBottleneck" },
            { "MissingEducatedWorkers", "NoEducatedWorkers" },
            { "MissingUneducatedWorkers", "NoWorkers" },
            { "NoRoadAccess", "RoadNotConnected" }
        };

        /// <summary>
        /// The icon the game itself puts over the affected buildings, so the row in the panel and
        /// the marker on the map are recognisably the same thing.
        /// </summary>
        public static string ForNotification(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                return Fallback;
            }

            var file = prefabName.Replace(" ", string.Empty);

            string alias;
            if (FileAliases.TryGetValue(file, out alias))
            {
                file = alias;
            }

            return NotificationRoot + file + ".svg";
        }

        /// <summary>
        /// The icon for a whole-city statement, where there is no single notification behind it.
        /// </summary>
        public static string ForSubject(AlertSubject subject)
        {
            switch (subject)
            {
                case AlertSubject.Electricity:
                    return IconRoot + "Electricity.svg";
                case AlertSubject.Water:
                    return IconRoot + "Water.svg";
                case AlertSubject.Sewage:
                    return IconRoot + "Sewage.svg";
                case AlertSubject.Garbage:
                    return IconRoot + "WasteRecycling.svg";
                case AlertSubject.Health:
                    return IconRoot + "Healthcare.svg";
                case AlertSubject.Deathcare:
                    return IconRoot + "Deathcare.svg";
                case AlertSubject.Fire:
                    return IconRoot + "FireSafety.svg";
                case AlertSubject.Police:
                    return IconRoot + "Police.svg";
                case AlertSubject.Education:
                    return IconRoot + "Education.svg";
                case AlertSubject.Transport:
                    return IconRoot + "Transportation.svg";
                case AlertSubject.Mail:
                    return IconRoot + "PostService.svg";
                case AlertSubject.Traffic:
                    return IconRoot + "Traffic.svg";
                case AlertSubject.Housing:
                    return IconRoot + "Zones.svg";
                case AlertSubject.Workplace:
                    return IconRoot + "Workers.svg";
                default:
                    return Fallback;
            }
        }
    }
}

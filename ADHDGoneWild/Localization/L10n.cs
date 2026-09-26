namespace ADHDGoneWild.Localization
{
    /// <summary>
    /// Localisation keys for everything the player can read in the mod's own UI.
    ///
    /// The React side looks these up through the game's localisation manager, so no user-facing
    /// sentence is written into a component. English is the source language; French and Italian
    /// are expected to follow, and will be added as further <see cref="System.Collections.Generic.IDictionary{TKey,TValue}"/>
    /// sources rather than as more code.
    ///
    /// Icons carry no language and are never translated.
    ///
    /// The same list exists in UI/src/theme/l10n.ts. Keep the two in step.
    /// </summary>
    public static class L10n
    {
        private const string Prefix = "ADHDGoneWild.UI.";

        public const string PanelTitle = Prefix + "PanelTitle";
        public const string ToolbarTooltip = Prefix + "ToolbarTooltip";

        public const string ParkIdea = Prefix + "ParkIdea";
        public const string ParkingHint = Prefix + "ParkingHint";
        public const string ParkingCancel = Prefix + "ParkingCancel";

        public const string Empty = Prefix + "Empty";
        public const string EmptyHint = Prefix + "EmptyHint";

        public const string View = Prefix + "View";
        public const string Forget = Prefix + "Forget";
        public const string NotePlaceholder = Prefix + "NotePlaceholder";

        public const string Disabled = Prefix + "Disabled";
        public const string Close = Prefix + "Close";
        public const string JustParkedSaved = Prefix + "JustParkedSaved";
        public const string ToolbarFold = Prefix + "ToolbarFold";
        public const string ToolbarUnfold = Prefix + "ToolbarUnfold";

        public const string SafetyNetTitle = Prefix + "SafetyNetTitle";
        public const string SafetyNetCreate = Prefix + "SafetyNetCreate";
        public const string SafetyNetHint = Prefix + "SafetyNetHint";
        public const string SafetyNetHave = Prefix + "SafetyNetHave";
        public const string SafetyNetRestore = Prefix + "SafetyNetRestore";
        public const string SafetyNetKeep = Prefix + "SafetyNetKeep";
        public const string SafetyNetConfirm = Prefix + "SafetyNetConfirm";
        public const string SafetyNetConfirmYes = Prefix + "SafetyNetConfirmYes";
        public const string SafetyNetConfirmNo = Prefix + "SafetyNetConfirmNo";
        public const string SafetyNetWorking = Prefix + "SafetyNetWorking";

        /// <summary>
        /// Two words inside the checkpoint's save name, so the row the player meets in their own
        /// list of saves is in the same language as the panel that offered it.
        /// </summary>
        public const string SafetyNetSaveMarker = Prefix + "SafetyNetSaveMarker";

        public const string HyperfocusHereFor = Prefix + "HyperfocusHereFor";
        public const string HyperfocusHourOne = Prefix + "HyperfocusHourOne";
        public const string HyperfocusHoursMany = Prefix + "HyperfocusHoursMany";
        public const string HyperfocusMinutesMany = Prefix + "HyperfocusMinutesMany";
        public const string HyperfocusThanks = Prefix + "HyperfocusThanks";
        public const string HyperfocusLater = Prefix + "HyperfocusLater";

        public const string WelcomeBackTitle = Prefix + "WelcomeBackTitle";
        public const string WelcomeBackViewPlace = Prefix + "WelcomeBackViewPlace";
        public const string WelcomeBackIdeasOne = Prefix + "WelcomeBackIdeasOne";
        public const string WelcomeBackIdeasMany = Prefix + "WelcomeBackIdeasMany";

        public const string AlertsTitle = Prefix + "AlertsTitle";
        public const string AlertsQuiet = Prefix + "AlertsQuiet";
        public const string AlertsQuietHint = Prefix + "AlertsQuietHint";
        public const string AlertsDisabled = Prefix + "AlertsDisabled";
        public const string AlertsMute = Prefix + "AlertsMute";
        public const string AlertsUnmute = Prefix + "AlertsUnmute";
        public const string AlertsMutedCount = Prefix + "AlertsMutedCount";
        public const string AffectedOne = Prefix + "AffectedOne";
        public const string AffectedMany = Prefix + "AffectedMany";

        /// <summary>
        /// The body notes, in the order of <see cref="Wellbeing.BodyNote"/>. Direct, warm and a
        /// little daft. Translators: an imperative is fine here - a jab is not. Nothing in these
        /// may be at the player's expense or imply they have let themselves go.
        /// </summary>
        public static readonly string[] BodyNote =
        {
            Prefix + "BodyNote.Water",
            Prefix + "BodyNote.Food",
            Prefix + "BodyNote.Stand",
            Prefix + "BodyNote.Eyes",
            Prefix + "BodyNote.Shoulders",
            Prefix + "BodyNote.LateHour"
        };

        public const string IdeaDescriptionPlaceholder = Prefix + "IdeaDescriptionPlaceholder";
        public const string HasDescription = Prefix + "HasDescription";
        public const string IdeaAddNote = Prefix + "IdeaAddNote";

        public const string TrailTitle = Prefix + "TrailTitle";
        public const string TrailEmpty = Prefix + "TrailEmpty";
        public const string TrailEmptyHint = Prefix + "TrailEmptyHint";
        public const string TrailBack = Prefix + "TrailBack";

        /// <summary>
        /// The activity words, in the order of <see cref="Trail.TrailActivity"/>.
        ///
        /// Translators: these describe what the player was doing, and they must stay descriptions.
        /// Not "unfinished roadworks", not "road project" - just the doing. The mod can see that a
        /// road tool was in their hand and it cannot see why, so it only says the first part.
        /// </summary>
        public static readonly string[] TrailActivity =
        {
            Prefix + "Trail.Looking",
            Prefix + "Trail.Drawing",
            Prefix + "Trail.Placing",
            Prefix + "Trail.Zoning",
            Prefix + "Trail.Demolishing",
            Prefix + "Trail.Landscaping",
            Prefix + "Trail.Areas",
            Prefix + "Trail.Lines",
            Prefix + "Trail.Upgrading"
        };

        public const string CategoryIdea = Prefix + "Category.Idea";
        public const string CategoryBuild = Prefix + "Category.Build";
        public const string CategoryDecoration = Prefix + "Category.Decoration";
        public const string CategoryTransport = Prefix + "Category.Transport";
        public const string CategoryFix = Prefix + "Category.Fix";
        public const string CategoryOther = Prefix + "Category.Other";
    }
}

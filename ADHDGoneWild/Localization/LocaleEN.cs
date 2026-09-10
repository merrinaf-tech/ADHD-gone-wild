using System.Collections.Generic;
using ADHDGoneWild.Alerts;
using ADHDGoneWild.BrainParking;
using ADHDGoneWild.Settings;
using Colossal;

namespace ADHDGoneWild.Localization
{
    /// <summary>
    /// The en-US source. Further languages arrive as data, not as more classes.
    ///
    /// House style for everything in here: short, calm, and never about the player. The mod
    /// reports what is there and what they saved. It does not tell them what to do about it, and
    /// it never implies that something is owed.
    /// </summary>
    public class LocaleEN : IDictionarySource
    {
        private readonly AdhdSettings _settings;

        public LocaleEN(AdhdSettings settings)
        {
            _settings = settings;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                // ---- Options page -----------------------------------------------------------
                { _settings.GetSettingsLocaleID(), Mod.Name },
                { _settings.GetOptionTabLocaleID(AdhdSettings.MainSection), "Main" },

                { _settings.GetOptionGroupLocaleID(AdhdSettings.InformationGroup), "Information" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.MemoryGroup), "Memory" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.CreativityGroup), "Creativity" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.WellbeingGroup), "Wellbeing" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.AccessibilityGroup), "Accessibility" },
                { _settings.GetOptionGroupLocaleID(AdhdSettings.AboutGroup), "About" },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.CalmNotificationIcons)),
                    "Hold map icons still"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.CalmNotificationIcons)),
                    "The game's warning icons pulse, which pulls the eye whether or not you meant to look. This stops the movement. Nothing is hidden - they stay where they are, the same size, saying the same thing."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.ShowToolbarCollapseButton)),
                    "Toolbar fold control"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.ShowToolbarCollapseButton)),
                    "A small control beside the build toolbar that folds it away and brings it back. The buttons are always one click from returning."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.SmartAlertsEnabled)),
                    "Smart alerts"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.SmartAlertsEnabled)),
                    "Gather the city's warnings into one short list, grouped instead of repeated."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.AlertVerbosity)),
                    "Tell me about"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.AlertVerbosity)),
                    "How much reaches the list. What is left out is still happening - it is just not on screen."
                },

                { _settings.GetEnumValueLocaleID(AdhdSettings.Verbosity.OnlyImmediate), "Only what is immediate" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.Verbosity.ImportantAndUp), "Immediate and important" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.Verbosity.Everything), "Everything" },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.BrainParkingEnabled)),
                    "Brain parking"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.BrainParkingEnabled)),
                    "Leave a thought on the map with a shortcut and a click, and carry on with what you were doing."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.ParkIdeaKey)),
                    "Park an idea"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.ParkIdeaKey)),
                    "Press this, then click where the thought belongs. Press it again to change your mind."
                },
                { _settings.GetBindingKeyLocaleID(AdhdSettings.ParkIdeaActionName), "Park an idea" },
                { _settings.GetBindingMapLocaleID(), Mod.Name },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.DefaultIdeaCategory)),
                    "New ideas are filed as"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.DefaultIdeaCategory)),
                    "What a freshly parked idea starts out as. You can change any of them later, or never."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.AskAfterParking)),
                    "Offer to add a note"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.AskAfterParking)),
                    "After parking a thought, offer to pick a kind and add a title. The thought is already saved either way - switch this off for shortcut, click, done."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.ShowIdeaMarkers)),
                    "Show ideas on the map"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.ShowIdeaMarkers)),
                    "Leave a small ring where each thought was parked, so the place itself reminds you."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.WelcomeBackEnabled)),
                    "Welcome back"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.WelcomeBackEnabled)),
                    "A small hello when you return to a city after a real break, with enough context that you don't have to reconstruct it yourself."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.WelcomeBackAfter)),
                    "Only after being away for"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.WelcomeBackAfter)),
                    "A short alt-tab does not deserve a greeting. This is how long a break has to be first."
                },

                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.TenMinutes), "10 minutes" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.ThirtyMinutes), "30 minutes" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.OneHour), "1 hour" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.ThreeHours), "3 hours" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.AwayLength.OneDay), "A day or more" },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.CreativeSafetyNetEnabled)),
                    "Safety net"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.CreativeSafetyNetEnabled)),
                    "A one-click way back from an experiment. It saves the game for you - separately from your own saves - so you can try something and change your mind without naming or finding anything."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.HyperfocusRemindersEnabled)),
                    "Mention the time now and then"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.HyperfocusRemindersEnabled)),
                    "A small card says what time it is and how long you have been here, then goes away. It never suggests stopping, and looking at it and carrying on is a perfectly good outcome."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.HyperfocusAfter)),
                    "How often"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.HyperfocusAfter)),
                    "How much play goes by between mentions. Time in the main menu does not count."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.HyperfocusBodyNotes)),
                    "Add a word about the body"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.HyperfocusBodyNotes)),
                    "A short second line under the clock - water, shoulders, standing up. Nothing is ever followed up on and nothing is counted. Off leaves you the time on its own."
                },

                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.TwoMinutes), "Every 2 minutes (to see what it looks like)" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.ThirtyMinutes), "Every 30 minutes" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.OneHour), "Every hour" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.NinetyMinutes), "Every 90 minutes" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.TwoHours), "Every 2 hours" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.FocusLength.ThreeHours), "Every 3 hours" },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.ColourPalette)),
                    "Colour palette"
                },
                {
                    _settings.GetOptionDescLocaleID(nameof(AdhdSettings.ColourPalette)),
                    "Colour never carries meaning on its own here - every status also has an icon and a shape."
                },

                {
                    _settings.GetOptionLabelLocaleID(nameof(AdhdSettings.Version)),
                    "Version"
                },

                { _settings.GetEnumValueLocaleID(AdhdSettings.Palette.Standard), "Standard" },
                { _settings.GetEnumValueLocaleID(AdhdSettings.Palette.ColourBlindFriendly), "Colour-blind friendly" },

                { _settings.GetEnumValueLocaleID(IdeaCategory.Idea), "Idea" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Build), "Build something" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Decoration), "Decoration" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Transport), "Transport" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Fix), "Fix or change" },
                { _settings.GetEnumValueLocaleID(IdeaCategory.Other), "Other" },

                // ---- In-game panel ----------------------------------------------------------
                { L10n.PanelTitle, "Parked ideas" },
                { L10n.ToolbarTooltip, Mod.Name },

                { L10n.ParkIdea, "Park an idea" },
                { L10n.ParkingHint, "Click anywhere to leave a thought there." },
                { L10n.ParkingCancel, "Never mind" },

                { L10n.Empty, "Nothing parked here yet." },
                { L10n.EmptyHint, "Press the shortcut, click the map. That is the whole thing." },

                { L10n.View, "View" },
                { L10n.Forget, "Forget" },
                { L10n.NotePlaceholder, "A word about it, if you want" },

                { L10n.Disabled, "Brain parking is switched off in the options." },
                { L10n.Close, "Close" },
                { L10n.JustParkedSaved, "Saved. Anything to add?" },
                { L10n.ToolbarFold, "Fold the toolbar away" },
                { L10n.ToolbarUnfold, "Bring the toolbar back" },

                // ---- Safety net ---------------------------------------------------------------
                // Reassurance, never an errand. Nothing here counts anything or asks to be closed.
                { L10n.SafetyNetTitle, "Safety net" },
                { L10n.SafetyNetCreate, "Save a way back" },
                { L10n.SafetyNetHint, "So you can try something and change your mind." },
                { L10n.SafetyNetHave, "You can come back to this" },
                { L10n.SafetyNetRestore, "Go back" },
                { L10n.SafetyNetKeep, "Keep what I did" },
                { L10n.SafetyNetWorking, "Working..." },
                { L10n.SafetyNetSaveMarker, "safety net" },

                // The wall clock, and nothing else. No advice, no suggestion to stop, no opinion
                // about the length of a session - hyperfocus takes away the sense of time, so the
                // mod hands the time back and lets the player decide what to do with it.
                { L10n.HyperfocusHereFor, "You have been here for" },
                { L10n.HyperfocusHourOne, "hour" },
                { L10n.HyperfocusHoursMany, "hours" },
                { L10n.HyperfocusMinutesMany, "minutes" },
                { L10n.HyperfocusThanks, "Thanks" },
                { L10n.HyperfocusLater, "Later" },

                // The shape that works, arrived at by testing the ones that did not: a direct
                // ask, plus either a warm reason or an offer to cover for them. Never a bare
                // command, and never a wry observation.
                //
                // These began as understatement - "Standing up is allowed", "Shoulders: probably
                // somewhere near your ears" - on the theory that an imperative would provoke
                // resistance. In game that register read as passive-aggressive: saying a thing
                // sideways sounds like a comment on the person, and dry understatement lands as
                // a raised eyebrow. Being plainly asked by something obviously on your side is
                // kinder than being hinted at.
                //
                // So imperatives are fine. What is not fine is a jab: nothing here may be at the
                // player's expense, imply they have let themselves go, or carry a number. Where
                // there is a joke it is aimed at a spine or a city, never at the reader.
                { L10n.BodyNote[0], "Water. Go on, I'll watch the city." },
                { L10n.BodyNote[1], "Eat something. Cities are hungry work." },
                { L10n.BodyNote[2], "Stand up and move for a minute. That's the whole ask." },
                { L10n.BodyNote[3], "Look at something far away. Your eyes will thank you." },
                { L10n.BodyNote[4], "Unclench your jaw. Lower your shoulders. Undo your fists." },
                { L10n.BodyNote[5], "Go to bed. That road will still be there tomorrow." },

                // ---- Welcome back -------------------------------------------------------------
                { L10n.WelcomeBackTitle, "Welcome back to" },
                { L10n.WelcomeBackViewPlace, "You were around here" },
                { L10n.WelcomeBackIdeasOne, "1 saved idea here" },
                { L10n.WelcomeBackIdeasMany, "saved ideas here" },

                // ---- Alerts -----------------------------------------------------------------
                // Every line below states something about the city. None of them tells the player
                // what to do about it; that decision stays theirs. See docs/PRODUCT_PRINCIPLES.md.
                { L10n.AlertsTitle, "Right now" },
                { L10n.AlertsQuiet, "Nothing to report." },
                { L10n.AlertsQuietHint, "The city is running as it was." },
                { L10n.AlertsDisabled, "Smart alerts are switched off in the options." },
                { L10n.AlertsMute, "Mute" },
                { L10n.AlertsUnmute, "Unmute" },
                { L10n.AlertsMutedCount, "muted" },
                { L10n.AffectedOne, "1 building" },
                { L10n.AffectedMany, "buildings" },

                {
                    AlertTitles.KeyFor(AlertSubject.Electricity, Core.Status.Immediate),
                    "Part of the city is without electricity."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Electricity, Core.Status.Important),
                    "Electricity demand has reached production."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Electricity, Core.Status.Monitor),
                    "Electricity demand is close to production."
                },

                {
                    AlertTitles.KeyFor(AlertSubject.Water, Core.Status.Immediate),
                    "Part of the city is without water."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Water, Core.Status.Important),
                    "Water demand has reached capacity."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Water, Core.Status.Monitor),
                    "Water demand is close to capacity."
                },

                {
                    AlertTitles.KeyFor(AlertSubject.Sewage, Core.Status.Immediate),
                    "Some sewage is going untreated."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Sewage, Core.Status.Important),
                    "Sewage treatment has reached capacity."
                },
                {
                    AlertTitles.KeyFor(AlertSubject.Sewage, Core.Status.Monitor),
                    "Sewage treatment is close to capacity."
                },

                { L10n.CategoryIdea, "Idea" },
                { L10n.CategoryBuild, "Build something" },
                { L10n.CategoryDecoration, "Decoration" },
                { L10n.CategoryTransport, "Transport" },
                { L10n.CategoryFix, "Fix or change" },
                { L10n.CategoryOther, "Other" }
            };
        }

        public void Unload()
        {
        }
    }
}

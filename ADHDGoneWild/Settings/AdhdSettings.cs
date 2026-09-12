using ADHDGoneWild.BrainParking;
using Colossal.IO.AssetDatabase;
using Game.Input;
using Game.Modding;
using Game.Settings;

namespace ADHDGoneWild.Settings
{
    /// <summary>
    /// Every feature the mod has, and whether the player wants it.
    ///
    /// There is no single correct ADHD experience, so nothing here is load-bearing: switching a
    /// feature off leaves the game exactly as the game shipped it. Options are added only once
    /// the feature behind them exists - an option that promises something unimplemented is worse
    /// than no option.
    ///
    /// Groups mirror the product's own categories (Information, Memory, Creativity, Wellbeing,
    /// Accessibility) so later features have an obvious home. Only the groups with something in
    /// them are declared today.
    /// </summary>
    [FileLocation("ModsSettings/ADHDGoneWild/ADHDGoneWild")]
    [SettingsUIGroupOrder(InformationGroup, MemoryGroup, CreativityGroup, WellbeingGroup, AppearanceGroup, AboutGroup)]
    [SettingsUIShowGroupName(InformationGroup, MemoryGroup, CreativityGroup, WellbeingGroup, AppearanceGroup, AboutGroup)]
    [SettingsUIKeyboardAction(ParkIdeaActionName, ActionType.Button, Usages.kDefaultUsage)]
    public class AdhdSettings : ModSetting
    {
        public const string MainSection = "Main";

        public const string InformationGroup = "InformationGroup";
        public const string MemoryGroup = "MemoryGroup";
        public const string CreativityGroup = "CreativityGroup";
        public const string WellbeingGroup = "WellbeingGroup";
        public const string AppearanceGroup = "AppearanceGroup";
        public const string AboutGroup = "AboutGroup";

        /// <summary>The input action behind the parking shortcut. Referenced by the binding below.</summary>
        public const string ParkIdeaActionName = "ADHDGoneWild.ParkIdea";

        private bool _calmNotificationIcons = true;
        private bool _showToolbarCollapseButton = true;
        private bool _toolbarCollapsed;
        private bool _smartAlertsEnabled = true;
        private Verbosity _alertVerbosity = Verbosity.ImportantAndUp;
        private bool _brainParkingEnabled = true;
        private IdeaCategory _defaultIdeaCategory = IdeaCategory.Idea;
        private bool _askAfterParking = true;
        private bool _showIdeaMarkers = true;
        private bool _creativeSafetyNetEnabled = true;
        private bool _welcomeBackEnabled = true;
        private AwayLength _welcomeBackAfter = AwayLength.ThirtyMinutes;
        private bool _hyperfocusRemindersEnabled = true;
        private FocusLength _hyperfocusAfter = FocusLength.TwoHours;
        private bool _hyperfocusBodyNotes = true;
        private int _interfaceHue = UI.InterfaceColour.DefaultHue;

        public AdhdSettings(IMod mod) : base(mod)
        {
        }

        /// <summary>
        /// How much the player wants to be told. This is a floor, not a filter on truth: what is
        /// hidden is still happening, and the mod never claims a quiet list means a quiet city.
        /// </summary>
        public enum Verbosity
        {
            OnlyImmediate = 0,
            ImportantAndUp = 1,
            Everything = 2
        }

        /// <summary>
        /// How much play goes by between mentions of the time. Like AwayLength, the values are
        /// the minutes themselves, so nothing has to translate them.
        ///
        /// The two-minute entry is real, not a debug leftover. Without it the only way to find
        /// out what the card looks like is to play for two hours, which is a poor way to decide
        /// whether you want it at all - and anyone who genuinely wants the time that often is
        /// entitled to it. The mod does not overrule a preference stated out loud in the options.
        /// </summary>
        public enum FocusLength
        {
            TwoMinutes = 2,
            ThirtyMinutes = 30,
            OneHour = 60,
            NinetyMinutes = 90,
            TwoHours = 120,
            ThreeHours = 180
        }

        /// <summary>
        /// How long a break has to be before Welcome Back says anything. An enum rather than a
        /// free number: every value here is a real minute count, so the settings page never shows
        /// a slider that can be dragged to zero and turned into a nag.
        /// </summary>
        public enum AwayLength
        {
            TenMinutes = 10,
            ThirtyMinutes = 30,
            OneHour = 60,
            ThreeHours = 180,
            OneDay = 1440
        }

        // ---- Information --------------------------------------------------------------------

        /// <summary>
        /// Holds the game's in-world notification icons still instead of letting them pulse.
        ///
        /// On by default: holding the icons still is the cheapest calm this mod can offer, with
        /// no information cost at all. Nothing is hidden by it - the icons stay where they are,
        /// the same size, saying the same thing. Only the movement goes.
        /// </summary>
        [SettingsUISection(MainSection, InformationGroup)]
        public bool CalmNotificationIcons
        {
            get { return _calmNotificationIcons; }
            set
            {
                if (_calmNotificationIcons == value)
                {
                    return;
                }

                _calmNotificationIcons = value;
                Mod.OnCalmSettingsChanged();
            }
        }

        /// <summary>
        /// Whether the small control that folds the game's build toolbar away is on screen.
        ///
        /// The control is what makes folding the toolbar honest rather than destructive: the
        /// buttons are one click away at all times, so nothing has been taken, only put down.
        /// </summary>
        [SettingsUISection(MainSection, InformationGroup)]
        public bool ShowToolbarCollapseButton
        {
            get { return _showToolbarCollapseButton; }
            set
            {
                if (_showToolbarCollapseButton == value)
                {
                    return;
                }

                _showToolbarCollapseButton = value;
                Mod.OnCalmSettingsChanged();
            }
        }

        /// <summary>
        /// Whether the toolbar is currently folded. Hidden from the options page because it is not
        /// really a preference - it is the state of a control the player flips in game.
        ///
        /// It used to be kept so the fold survived a restart. It no longer does: every city now
        /// opens with the toolbar out. A player folded it, used the safety net to go back, and
        /// could not get the build row out again - a remembered preference had become a state they
        /// could not leave. See CalmToolbarUISystem.OnCityChanged.
        /// </summary>
        [SettingsUIHidden]
        public bool ToolbarCollapsed
        {
            get { return _toolbarCollapsed; }
            set { _toolbarCollapsed = value; }
        }

        [SettingsUISection(MainSection, InformationGroup)]
        public bool SmartAlertsEnabled
        {
            get { return _smartAlertsEnabled; }
            set
            {
                if (_smartAlertsEnabled == value)
                {
                    return;
                }

                _smartAlertsEnabled = value;
                Mod.OnAlertSettingsChanged();
            }
        }

        [SettingsUISection(MainSection, InformationGroup)]
        [SettingsUIHideByCondition(typeof(AdhdSettings), nameof(SmartAlertsOff))]
        public Verbosity AlertVerbosity
        {
            get { return _alertVerbosity; }
            set
            {
                if (_alertVerbosity == value)
                {
                    return;
                }

                _alertVerbosity = value;
                Mod.OnAlertSettingsChanged();
            }
        }

        /// <summary>The least serious status that still reaches the screen.</summary>
        public Core.Status AlertFloor()
        {
            switch (_alertVerbosity)
            {
                case Verbosity.OnlyImmediate:
                    return Core.Status.Immediate;
                case Verbosity.Everything:
                    return Core.Status.Muted;
                default:
                    return Core.Status.Important;
            }
        }

        // ---- Memory -------------------------------------------------------------------------

        [SettingsUISection(MainSection, MemoryGroup)]
        public bool BrainParkingEnabled
        {
            get { return _brainParkingEnabled; }
            set
            {
                if (_brainParkingEnabled == value)
                {
                    return;
                }

                _brainParkingEnabled = value;
                Mod.OnBrainParkingSettingsChanged();
            }
        }

        /// <summary>
        /// Press, click, done. The shortcut is the whole interaction; anything that asks a
        /// question first has already cost the player the thought they were trying to put down.
        /// </summary>
        [SettingsUISection(MainSection, MemoryGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.I, ParkIdeaActionName, ctrl: true)]
        public ProxyBinding ParkIdeaKey { get; set; }

        /// <summary>
        /// What a freshly parked idea is filed as. It can be changed afterwards, and most players
        /// will never touch it - the point of a default is that nobody has to choose in the moment.
        /// </summary>
        [SettingsUISection(MainSection, MemoryGroup)]
        [SettingsUIHideByCondition(typeof(AdhdSettings), nameof(BrainParkingOff))]
        public IdeaCategory DefaultIdeaCategory
        {
            get { return _defaultIdeaCategory; }
            set { _defaultIdeaCategory = value; }
        }

        /// <summary>
        /// Whether parking a thought offers a card to add a kind and a title afterwards.
        ///
        /// The idea is saved before the card appears either way, so this only decides whether the
        /// offer is made. Switch it off and parking goes back to its shortest form: shortcut,
        /// click, straight back to what you were doing.
        /// </summary>
        [SettingsUISection(MainSection, MemoryGroup)]
        [SettingsUIHideByCondition(typeof(AdhdSettings), nameof(BrainParkingOff))]
        public bool AskAfterParking
        {
            get { return _askAfterParking; }
            set { _askAfterParking = value; }
        }

        /// <summary>
        /// Whether a parked idea leaves a visible ring on the map. On by default: the place is
        /// half of what the player saved, and a list alone throws that half away.
        /// </summary>
        [SettingsUISection(MainSection, MemoryGroup)]
        [SettingsUIHideByCondition(typeof(AdhdSettings), nameof(BrainParkingOff))]
        public bool ShowIdeaMarkers
        {
            get { return _showIdeaMarkers; }
            set { _showIdeaMarkers = value; }
        }

        [SettingsUISection(MainSection, MemoryGroup)]
        public bool WelcomeBackEnabled
        {
            get { return _welcomeBackEnabled; }
            set
            {
                if (_welcomeBackEnabled == value)
                {
                    return;
                }

                _welcomeBackEnabled = value;
                Mod.OnWelcomeBackSettingsChanged();
            }
        }

        /// <summary>
        /// How long the player has to have been away before the mod says anything about it. A
        /// five-minute alt-tab is not a homecoming; this exists so it is never treated like one.
        /// </summary>
        [SettingsUISection(MainSection, MemoryGroup)]
        [SettingsUIHideByCondition(typeof(AdhdSettings), nameof(WelcomeBackOff))]
        public AwayLength WelcomeBackAfter
        {
            get { return _welcomeBackAfter; }
            set { _welcomeBackAfter = value; }
        }

        public int WelcomeBackAfterMinutes()
        {
            return (int)_welcomeBackAfter;
        }

        // ---- Creativity ---------------------------------------------------------------------

        /// <summary>
        /// Whether the safety net is offered: a one-click way back from an experiment.
        ///
        /// The checkpoint is a real save made through the game's own save path, kept separate
        /// from the player's own saves and overwritten only by itself.
        /// </summary>
        [SettingsUISection(MainSection, CreativityGroup)]
        public bool CreativeSafetyNetEnabled
        {
            get { return _creativeSafetyNetEnabled; }
            set
            {
                if (_creativeSafetyNetEnabled == value)
                {
                    return;
                }

                _creativeSafetyNetEnabled = value;
                Mod.OnSafetyNetSettingsChanged();
            }
        }

        // ---- Wellbeing ----------------------------------------------------------------------

        /// <summary>
        /// Whether the mod mentions the time now and then.
        ///
        /// On by default. It is the one thing here that a player deep in hyperfocus cannot ask
        /// for at the moment they need it, because not noticing is the whole condition - so it
        /// has to already be on. It costs nothing to ignore, and one click to switch off.
        /// </summary>
        [SettingsUISection(MainSection, WellbeingGroup)]
        public bool HyperfocusRemindersEnabled
        {
            get { return _hyperfocusRemindersEnabled; }
            set
            {
                if (_hyperfocusRemindersEnabled == value)
                {
                    return;
                }

                _hyperfocusRemindersEnabled = value;
                Mod.OnHyperfocusSettingsChanged();
            }
        }

        /// <summary>
        /// How much play goes by between mentions. Spacing, not a limit: nothing in the mod knows
        /// or cares how long a session ought to be.
        /// </summary>
        [SettingsUISection(MainSection, WellbeingGroup)]
        [SettingsUIHideByCondition(typeof(AdhdSettings), nameof(HyperfocusOff))]
        public FocusLength HyperfocusAfter
        {
            get { return _hyperfocusAfter; }
            set
            {
                if (_hyperfocusAfter == value)
                {
                    return;
                }

                _hyperfocusAfter = value;
                Mod.OnHyperfocusSettingsChanged();
            }
        }

        /// <summary>
        /// Whether the card adds a quiet word about the body under the clock.
        ///
        /// Separate from the reminder itself on purpose. Some people want to know the time and
        /// nothing else, and being told about water by a city builder is exactly the kind of
        /// thing that gets a whole feature switched off. Turning this off keeps the clock.
        ///
        /// Nothing is ever followed up on: the mod cannot tell whether the player drank
        /// anything, and does not count what was ignored. The moment it could, it would be a
        /// habit tracker.
        /// </summary>
        [SettingsUISection(MainSection, WellbeingGroup)]
        [SettingsUIHideByCondition(typeof(AdhdSettings), nameof(HyperfocusOff))]
        public bool HyperfocusBodyNotes
        {
            get { return _hyperfocusBodyNotes; }
            set
            {
                if (_hyperfocusBodyNotes == value)
                {
                    return;
                }

                _hyperfocusBodyNotes = value;
                Mod.OnHyperfocusSettingsChanged();
            }
        }

        /// <summary>Hides the rest of the group when there is nothing to configure.</summary>
        public bool HyperfocusOff
        {
            get { return !_hyperfocusRemindersEnabled; }
        }

        // ---- Appearance ---------------------------------------------------------------------

        /// <summary>
        /// The hue of the mod's own panels. Only the hue: saturation and lightness are fixed, so
        /// every position on this slider is a dark, muted surface the text still reads against.
        ///
        /// A free colour picker would let the player make their own panels unreadable and then
        /// have no way of seeing what they had done. This gives the choice without the trap.
        /// </summary>
        [SettingsUISection(MainSection, AppearanceGroup)]
        [SettingsUISlider(min = 0f, max = 360f, step = 5f, unit = "integer")]
        public int InterfaceHue
        {
            get { return _interfaceHue; }
            set
            {
                if (_interfaceHue == value)
                {
                    return;
                }

                _interfaceHue = value;
                Mod.OnInterfaceHueChanged();
            }
        }

        // ---- About --------------------------------------------------------------------------

        [SettingsUISection(MainSection, AboutGroup)]
        public string Version
        {
            get { return Mod.Version; }
        }

        /// <summary>Used by SettingsUIHideByCondition; must be public and parameterless.</summary>
        public bool BrainParkingOff()
        {
            return !_brainParkingEnabled;
        }

        /// <summary>Used by SettingsUIHideByCondition; must be public and parameterless.</summary>
        public bool SmartAlertsOff()
        {
            return !_smartAlertsEnabled;
        }

        /// <summary>Used by SettingsUIHideByCondition; must be public and parameterless.</summary>
        public bool WelcomeBackOff()
        {
            return !_welcomeBackEnabled;
        }

        public override void SetDefaults()
        {
            _calmNotificationIcons = true;
            _showToolbarCollapseButton = true;
            _toolbarCollapsed = false;
            _smartAlertsEnabled = true;
            _alertVerbosity = Verbosity.ImportantAndUp;
            _brainParkingEnabled = true;
            _defaultIdeaCategory = IdeaCategory.Idea;
            _askAfterParking = true;
            _showIdeaMarkers = true;
            _creativeSafetyNetEnabled = true;
            _welcomeBackEnabled = true;
            _welcomeBackAfter = AwayLength.ThirtyMinutes;
            _interfaceHue = UI.InterfaceColour.DefaultHue;
        }
    }
}

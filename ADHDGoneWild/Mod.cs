using System;
using ADHDGoneWild.Alerts;
using ADHDGoneWild.BrainParking;
using ADHDGoneWild.Calm;
using ADHDGoneWild.Creativity;
using ADHDGoneWild.Localization;
using ADHDGoneWild.Memory;
using ADHDGoneWild.Settings;
using ADHDGoneWild.UI;
using ADHDGoneWild.Wellbeing;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace ADHDGoneWild
{
    /// <summary>
    /// ADHD gone wild - a mod that lets your brain play however it wants, and takes care of what
    /// it does not want to keep in mind.
    ///
    /// It reduces what the player has to hold in their head. It does not try to make them more
    /// organised, more focused or more productive, and nothing in it is allowed to imply that an
    /// unfinished thing is owed. See docs/PRODUCT_PRINCIPLES.md before adding a feature.
    ///
    /// Towards the game it is read-only: it reads positions and city state, and it writes nothing
    /// into a save. Its own notes live in a file beside the game's user data, so a save made with
    /// this mod opens without it.
    /// </summary>
    public class Mod : IMod
    {
        public const string Id = "ADHDGoneWild";
        public const string Name = "ADHD gone wild";
        public const string Version = "0.1.0";

        /// <summary>
        /// English is the source language: every key exists here first, and the others are checked
        /// against it at load. A missing translation falls back to this rather than showing a key.
        /// </summary>
        private const string LocaleId = "en-US";

        private const string LocaleIdIT = "it-IT";
        private const string LocaleIdFR = "fr-FR";

        public static readonly ILog Log = LogManager.GetLogger(Id).SetShowsErrorsInUI(false);

        public static AdhdSettings Settings { get; private set; }

        private static BrainParkingUISystem _brainParking;
        private static AlertsUISystem _alerts;
        private static WelcomeBackUISystem _welcomeBack;
        private static CalmIconsSystem _calm;
        private static CalmToolbarUISystem _calmToolbar;
        private static CheckpointSystem _checkpoints;
        private static HyperfocusSystem _hyperfocus;
        private static ThemeUISystem _theme;
        private static LocaleEN _locale;
        private static LocaleIT _localeIT;
        private static LocaleFR _localeFR;

        public void OnLoad(UpdateSystem updateSystem)
        {
            Log.Info(Name + " " + Version + " loading.");

            Settings = new AdhdSettings(this);
            Settings.RegisterInOptionsUI();

            _locale = new LocaleEN(Settings);
            GameManager.instance.localizationManager.AddSource(LocaleId, _locale);

            _localeIT = new LocaleIT(Settings);
            GameManager.instance.localizationManager.AddSource(LocaleIdIT, _localeIT);

            _localeFR = new LocaleFR(Settings);
            GameManager.instance.localizationManager.AddSource(LocaleIdFR, _localeFR);

            // A translation missing a line falls back to English and says nothing about it, so
            // the only way a gap ever surfaces is if something goes looking. See LocaleCoverage.
            LocaleCoverage.Report("it-IT", _locale, _localeIT);
            LocaleCoverage.Report("fr-FR", _locale, _localeFR);

            // After the locale source, so the binding has a name to show, and before LoadSettings,
            // because the stored file may carry a key the player rebound and the action has to
            // exist for that to be applied to anything.
            Settings.RegisterKeyBindings();

            AssetDatabase.global.LoadSettings(Id, Settings, new AdhdSettings(this));

            updateSystem.UpdateAt<BrainParkingUISystem>(SystemUpdatePhase.UIUpdate);

            // Markers are painted into the overlay buffer, which is rebuilt every rendered frame.
            updateSystem.UpdateAt<IdeaMarkerSystem>(SystemUpdatePhase.Rendering);
            updateSystem.UpdateAt<AlertsUISystem>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateAt<WelcomeBackUISystem>(SystemUpdatePhase.UIUpdate);

            // Registered so it gets created; it does no per-frame work and wakes on
            // the game's load callbacks and on the option changing.
            updateSystem.UpdateAt<CalmIconsSystem>(SystemUpdatePhase.PrefabUpdate);
            updateSystem.UpdateAt<CalmToolbarUISystem>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateAt<CheckpointSystem>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateAt<HyperfocusSystem>(SystemUpdatePhase.UIUpdate);
            updateSystem.UpdateAt<ThemeUISystem>(SystemUpdatePhase.UIUpdate);

            // CityMemorySystem is deliberately not registered in a phase: it has no per-frame
            // work. The systems above create it, and it wakes on the game's own load callbacks.

            Log.Info(Name + " loaded.");
        }

        public void OnDispose()
        {
            Log.Info(Name + " disposing.");

            // Whatever is unsaved is written by CityMemorySystem.OnDestroy, which the game calls
            // as the world comes down. Nothing to flush from here.
            _brainParking = null;
            _alerts = null;
            _welcomeBack = null;
            _calm = null;
            _calmToolbar = null;
            _checkpoints = null;
            _theme = null;

            if (_locale != null)
            {
                try
                {
                    if (GameManager.instance != null && GameManager.instance.localizationManager != null)
                    {
                        GameManager.instance.localizationManager.RemoveSource(LocaleId, _locale);

                        if (_localeIT != null)
                        {
                            GameManager.instance.localizationManager.RemoveSource(LocaleIdIT, _localeIT);
                        }

                        if (_localeFR != null)
                        {
                            GameManager.instance.localizationManager.RemoveSource(LocaleIdFR, _localeFR);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Warn("Could not remove the localisation source: " + e.Message);
                }

                _locale = null;
                _localeIT = null;
                _localeFR = null;
            }

            if (Settings != null)
            {
                try
                {
                    Settings.UnregisterInOptionsUI();
                }
                catch (Exception e)
                {
                    Log.Warn("Could not unregister the options page: " + e.Message);
                }

                Settings = null;
            }

            Log.Info(Name + " disposed.");
        }

        internal static void RegisterBrainParking(BrainParkingUISystem system)
        {
            _brainParking = system;
        }

        internal static void RegisterAlerts(AlertsUISystem system)
        {
            _alerts = system;
        }

        internal static void RegisterWelcomeBack(WelcomeBackUISystem system)
        {
            _welcomeBack = system;
        }

        internal static void RegisterCalm(CalmIconsSystem system)
        {
            _calm = system;
        }

        internal static void RegisterCalmToolbar(CalmToolbarUISystem system)
        {
            _calmToolbar = system;
        }

        internal static void RegisterCheckpoints(CheckpointSystem system)
        {
            _checkpoints = system;
        }

        internal static void RegisterTheme(ThemeUISystem system)
        {
            _theme = system;
        }

        internal static void RegisterHyperfocus(HyperfocusSystem system)
        {
            _hyperfocus = system;
        }

        internal static void OnBrainParkingSettingsChanged()
        {
            if (_brainParking != null)
            {
                _brainParking.ApplySettings();
            }
        }

        internal static void OnAlertSettingsChanged()
        {
            if (_alerts != null)
            {
                _alerts.ApplySettings();
            }
        }

        internal static void OnWelcomeBackSettingsChanged()
        {
            if (_welcomeBack != null)
            {
                _welcomeBack.ApplySettings();
            }
        }

        internal static void OnCalmSettingsChanged()
        {
            if (_calm != null)
            {
                _calm.ApplySettings();
            }

            if (_calmToolbar != null)
            {
                _calmToolbar.ApplySettings();
            }
        }

        internal static void OnSafetyNetSettingsChanged()
        {
            if (_checkpoints != null)
            {
                _checkpoints.ApplySettings();
            }
        }

        internal static void OnInterfaceHueChanged()
        {
            if (_theme != null)
            {
                _theme.ApplySettings();
            }
        }

        internal static void OnHyperfocusSettingsChanged()
        {
            if (_hyperfocus != null)
            {
                _hyperfocus.ApplySettings();
            }
        }

    }
}

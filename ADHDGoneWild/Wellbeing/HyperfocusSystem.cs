using System;
using ADHDGoneWild.Localization;
using ADHDGoneWild.Memory;
using Colossal.UI.Binding;
using Game.UI;

namespace ADHDGoneWild.Wellbeing
{
    /// <summary>
    /// The Hyperfocus Guardian: the mod's one wellbeing feature, and the only place it is allowed
    /// to notice how long the player has been here.
    ///
    /// It does not ask them to stop, suggest a break, or have any opinion about the length of a
    /// session. Hyperfocus is not a discipline problem to be corrected - and a mod that treats it
    /// as one would be exactly the productivity tool this project refuses to be. What hyperfocus
    /// actually takes away is the sense of time passing. So the card hands that back and nothing
    /// else: the wall clock, and how long they have been in the city.
    ///
    /// A player who looks at "23:40" and keeps building has lost nothing. That is a success, not
    /// a failure of the feature.
    ///
    /// It is ambient by construction: one small card, never modal, never pausing the game, never
    /// making a sound, dismissable in one click and switchable off entirely. Only one can be on
    /// screen, and there is no queue behind it.
    ///
    /// Time is counted while a city is open, from a monotonic clock, so it does not include the
    /// main menu and cannot be confused by the simulation being paused or run at triple speed.
    /// </summary>
    public partial class HyperfocusSystem : UISystemBase
    {
        private const string Group = "adhd";
        private const string LogPrefix = "[Wellbeing] ";

        private CityMemorySystem _memory;

        private RawValueBinding _binding;
        private ValueBinding<bool> _enabledBinding;

        /// <summary>Minutes spent in a city since the game was started. Never persisted.</summary>
        private double _playedMinutes;

        /// <summary>The reading of <see cref="_playedMinutes"/> at which to say something next.</summary>
        private double _nextDueMinutes;

        private float _lastRealtime;
        private bool _showing;

        /// <summary>The wall clock at the moment the card appeared, not now. It states when it arrived.</summary>
        private string _clock = string.Empty;

        private int _shownAtMinutes;

        /// <summary>
        /// How many cards this run. Only ever used to vary the card - which line it carries and
        /// which corner it appears in - and never reported, never persisted, never used to make
        /// the card more insistent.
        /// </summary>
        private int _shownCount;

        /// <summary>Where the current card is. Rotates, so no two in a row land in one place.</summary>
        private HyperfocusCorner _corner;

        /// <summary>The body note on the current card, as a localisation key. Empty when off.</summary>
        private string _noteKey = string.Empty;

        protected override void OnCreate()
        {
            base.OnCreate();

            _memory = World.GetOrCreateSystemManaged<CityMemorySystem>();

            _binding = new RawValueBinding(Group, "hyperfocus", Write);
            AddBinding(_binding);

            _enabledBinding = new ValueBinding<bool>(Group, "hyperfocusEnabled", FeatureEnabled());
            AddBinding(_enabledBinding);

            AddBinding(new TriggerBinding(Group, "dismissHyperfocus", Dismiss));
            AddBinding(new TriggerBinding(Group, "snoozeHyperfocus", Snooze));

            _lastRealtime = UnityEngine.Time.realtimeSinceStartup;
            _nextDueMinutes = HyperfocusRule.FromNow(0.0, IntervalMinutes());

            Mod.RegisterHyperfocus(this);
            Mod.Log.Info("[UI] Hyperfocus bridge initialised.");
        }

        /// <summary>Called when the player changes the option.</summary>
        public void ApplySettings()
        {
            _enabledBinding.Update(FeatureEnabled());

            // Changing the spacing restarts it from now, in both directions - see
            // HyperfocusRule.FromNow. Switching the feature off also takes the card away, rather
            // than leaving one on screen that the option says should not exist.
            _nextDueMinutes = HyperfocusRule.FromNow(_playedMinutes, IntervalMinutes());

            if (!FeatureEnabled() && _showing)
            {
                _showing = false;
            }

            _binding.Update();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            // realtimeSinceStartup rather than deltaTime: this has to be minutes of the player's
            // life, which the simulation speed and the pause button have no bearing on.
            var now = UnityEngine.Time.realtimeSinceStartup;
            var elapsed = now - _lastRealtime;
            _lastRealtime = now;

            if (!_memory.InGame)
            {
                return;
            }

            // A frame that took more than a few seconds means the game was loading or the machine
            // was asleep, not that the player sat there. Counting it would inflate the figure the
            // whole feature depends on being true.
            if (elapsed > 0f && elapsed < 5f)
            {
                _playedMinutes += elapsed / 60.0;
            }

            if (_showing)
            {
                return;
            }

            if (!HyperfocusRule.ShouldShow(FeatureEnabled(), _playedMinutes, _nextDueMinutes))
            {
                return;
            }

            Show();
        }

        private void Show()
        {
            _showing = true;
            _shownAtMinutes = (int)_playedMinutes;

            // Local time, because it is being read by someone in a room, and formatted here so
            // the panel never has to guess at a locale.
            var now = DateTime.Now;
            _clock = now.ToString("HH:mm");

            // The note rotates so that no single line becomes the one the player learns to stop
            // reading. _shownCount exists for that and nothing else: it is never sent to the UI,
            // never saved, and never used to make the card more insistent.
            _noteKey = BodyNotesOn()
                ? L10n.BodyNote[(int)BodyNotes.Pick(_shownCount, now.Hour)]
                : string.Empty;

            // Chosen before the counter moves on, so the first card of a session is bottom left -
            // where this card has always been - and each one after it is somewhere the last was
            // not. See HyperfocusCorners for why it rotates rather than rolling a die.
            _corner = HyperfocusCorners.For(_shownCount);

            _shownCount++;

            _binding.Update();
            Mod.Log.Info(LogPrefix + "Mentioned the time at " + _clock + " (" + _shownAtMinutes + " minutes in).");
        }

        /// <summary>"Thanks." Nothing is recorded about it beyond when to mention the time again.</summary>
        private void Dismiss()
        {
            _showing = false;
            _nextDueMinutes = HyperfocusRule.AfterDismissed(_playedMinutes, IntervalMinutes());
            _binding.Update();
        }

        /// <summary>"Later."</summary>
        private void Snooze()
        {
            _showing = false;
            _nextDueMinutes = HyperfocusRule.AfterSnoozed(_playedMinutes, IntervalMinutes());
            _binding.Update();
        }

        private static bool FeatureEnabled()
        {
            var settings = Mod.Settings;
            return settings == null || settings.HyperfocusRemindersEnabled;
        }

        private static bool BodyNotesOn()
        {
            var settings = Mod.Settings;
            return settings == null || settings.HyperfocusBodyNotes;
        }

        private static int IntervalMinutes()
        {
            var settings = Mod.Settings;
            return settings == null ? 120 : (int)settings.HyperfocusAfter;
        }

        private void Write(IJsonWriter writer)
        {
            writer.TypeBegin("adhd.Hyperfocus");

            writer.PropertyName("visible");
            writer.Write(_showing);

            writer.PropertyName("clock");
            writer.Write(_clock ?? string.Empty);

            writer.PropertyName("minutes");
            writer.Write(_shownAtMinutes);

            writer.PropertyName("noteKey");
            writer.Write(_noteKey ?? string.Empty);

            writer.PropertyName("corner");
            writer.Write((int)_corner);

            writer.TypeEnd();
        }
    }
}

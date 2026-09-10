using System;
using ADHDGoneWild.Core;
using Colossal.UI.Binding;
using Game.Rendering;
using Game.UI;

namespace ADHDGoneWild.Memory
{
    /// <summary>
    /// Says hello, once, when the player returns to a city after a real break.
    ///
    /// The judgement itself - is this actually a homecoming - lives in the pure, tested
    /// <see cref="WelcomeBackRule"/>. This class only does what needs the game: reading the
    /// clock, reading what <see cref="CityMemory"/> already remembers, and publishing it.
    ///
    /// It never reappears once dismissed for a given load, and it never appears at all for a city
    /// the mod has not seen before - there is nothing to welcome the player back from.
    /// </summary>
    public partial class WelcomeBackUISystem : UISystemBase
    {
        private const string Group = "adhd";
        private const string LogPrefix = "[WelcomeBack] ";

        private CityMemorySystem _memory;
        private CameraUpdateSystem _camera;

        private ValueBinding<bool> _visibleBinding;
        private RawValueBinding _infoBinding;

        protected override void OnCreate()
        {
            base.OnCreate();

            _memory = World.GetOrCreateSystemManaged<CityMemorySystem>();
            _memory.CityChanged += OnCityChanged;

            _camera = World.GetOrCreateSystemManaged<CameraUpdateSystem>();

            _visibleBinding = new ValueBinding<bool>(Group, "welcomeBackVisible", false);
            AddBinding(_visibleBinding);

            _infoBinding = new RawValueBinding(Group, "welcomeBackInfo", WriteInfo);
            AddBinding(_infoBinding);

            AddBinding(new TriggerBinding(Group, "dismissWelcomeBack", Dismiss));
            AddBinding(new TriggerBinding(Group, "viewLastPlace", ViewLastPlace));

            Mod.RegisterWelcomeBack(this);
            Mod.Log.Info("[UI] Welcome Back bridge initialised.");
        }

        private void OnCityChanged()
        {
            if (!_memory.InGame || !_memory.Memory.IsLoaded)
            {
                _visibleBinding.Update(false);
                return;
            }

            Evaluate();
        }

        /// <summary>Called by the settings when the player switches the feature or its threshold.</summary>
        public void ApplySettings()
        {
            var settings = Mod.Settings;
            if (settings != null && !settings.WelcomeBackEnabled)
            {
                _visibleBinding.Update(false);
                return;
            }

            if (_memory.InGame && _memory.Memory.IsLoaded)
            {
                Evaluate();
            }
        }

        private void Evaluate()
        {
            try
            {
                var settings = Mod.Settings;
                if (settings != null && !settings.WelcomeBackEnabled)
                {
                    _visibleBinding.Update(false);
                    return;
                }

                var lastSeen = _memory.Memory.LastSeenUnixUtc;
                var now = CityMemory.ToUnix(DateTime.UtcNow);
                var afterMinutes = settings == null ? (int)Settings.AdhdSettings.AwayLength.ThirtyMinutes
                    : settings.WelcomeBackAfterMinutes();

                if (!WelcomeBackRule.ShouldShow(lastSeen, now, afterMinutes))
                {
                    _visibleBinding.Update(false);
                    return;
                }

                _infoBinding.Update();
                _visibleBinding.Update(true);

                Mod.Log.Info(LogPrefix + "Showing for '" + _memory.Memory.CityKey + "', " +
                             ((now - lastSeen) / 60) + " minutes away.");
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not decide whether to say welcome back.");
                _visibleBinding.Update(false);
            }
        }

        private void Dismiss()
        {
            _visibleBinding.Update(false);
        }

        private void ViewLastPlace()
        {
            WorldPoint point;
            if (_memory.Memory.TryGetLastCamera(out point))
            {
                CameraJump.To(_camera, point);
            }
        }

        private void WriteInfo(IJsonWriter writer)
        {
            writer.TypeBegin("adhd.WelcomeBackInfo");

            writer.PropertyName("cityName");
            writer.Write(_memory.Memory.CityName);

            writer.PropertyName("ideaCount");
            writer.Write(_memory.Memory.Ideas.Count);

            WorldPoint unused;
            writer.PropertyName("hasLastPlace");
            writer.Write(_memory.Memory.TryGetLastCamera(out unused));

            writer.TypeEnd();
        }
    }
}

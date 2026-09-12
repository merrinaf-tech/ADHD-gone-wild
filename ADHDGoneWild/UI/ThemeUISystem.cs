using Colossal.UI.Binding;
using Game.UI;

namespace ADHDGoneWild.UI
{
    /// <summary>
    /// The one thing the whole UI shares: the colour of the mod's own panels.
    ///
    /// It is sent as a finished CSS colour rather than as a hue, so the conversion lives once, in
    /// <see cref="InterfaceColour"/>, where it can be tested outside the game. The React side only
    /// has to drop the string into a style attribute.
    /// </summary>
    public partial class ThemeUISystem : UISystemBase
    {
        private const string Group = "adhd";

        private ValueBinding<string> _surfaceBinding;

        protected override void OnCreate()
        {
            base.OnCreate();

            _surfaceBinding = new ValueBinding<string>(Group, "surface", CurrentSurface());
            AddBinding(_surfaceBinding);

            Mod.RegisterTheme(this);
            Mod.Log.Info("[UI] Theme bridge initialised.");
        }

        /// <summary>Called when the player drags the hue slider.</summary>
        public void ApplySettings()
        {
            if (_surfaceBinding != null)
            {
                _surfaceBinding.Update(CurrentSurface());
            }
        }

        private static string CurrentSurface()
        {
            var settings = Mod.Settings;
            return InterfaceColour.Surface(settings == null
                ? InterfaceColour.DefaultHue
                : settings.InterfaceHue);
        }
    }
}

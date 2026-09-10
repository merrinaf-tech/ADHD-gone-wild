using ADHDGoneWild.Settings;
using Colossal.UI.Binding;
using Game.UI;

namespace ADHDGoneWild.UI
{
    /// <summary>
    /// The one thing the whole UI shares: which palette it is drawing in.
    ///
    /// The values themselves live on the UI side, in UI/src/theme/tokens.ts. This side only says
    /// which set to use, because a hex code in C# would be a second place for a colour to live
    /// and the two would drift.
    /// </summary>
    public partial class ThemeUISystem : UISystemBase
    {
        private const string Group = "adhd";

        private ValueBinding<int> _paletteBinding;

        protected override void OnCreate()
        {
            base.OnCreate();

            _paletteBinding = new ValueBinding<int>(Group, "palette", (int)CurrentPalette());
            AddBinding(_paletteBinding);

            Mod.RegisterTheme(this);
            Mod.Log.Info("[UI] Theme bridge initialised.");
        }

        public void ApplySettings()
        {
            if (_paletteBinding != null)
            {
                _paletteBinding.Update((int)CurrentPalette());
            }
        }

        private static AdhdSettings.Palette CurrentPalette()
        {
            var settings = Mod.Settings;
            return settings == null ? AdhdSettings.Palette.Standard : settings.ColourPalette;
        }
    }
}

using System;
using Colossal.UI.Binding;
using Game.UI;

namespace ADHDGoneWild.Calm
{
    /// <summary>
    /// Folds the game's own build toolbar away, and holds the state of that fold.
    ///
    /// The folding itself is not implemented here or on the UI side: the vanilla
    /// <c>Toolbar</c> component already takes an <c>onlyStatusVisible</c> prop and animates the
    /// change itself. All this does is decide when to ask for it, which is why the feature is
    /// cheap and why it should keep working across patches - we are using the game's own
    /// behaviour rather than covering it up.
    ///
    /// This is progressive disclosure, not filtering: the control that folds the toolbar stays on
    /// screen while it is folded, so the buttons are always one click from coming back. That is
    /// the line between putting something down and taking it away.
    /// </summary>
    public partial class CalmToolbarUISystem : UISystemBase
    {
        private const string Group = "adhd";
        private const string LogPrefix = "[Calm] ";

        private ValueBinding<bool> _collapsedBinding;
        private ValueBinding<bool> _buttonVisibleBinding;

        protected override void OnCreate()
        {
            base.OnCreate();

            _collapsedBinding = new ValueBinding<bool>(Group, "toolbarCollapsed", Collapsed());
            AddBinding(_collapsedBinding);

            _buttonVisibleBinding = new ValueBinding<bool>(Group, "toolbarButtonVisible", ButtonVisible());
            AddBinding(_buttonVisibleBinding);

            AddBinding(new TriggerBinding<bool>(Group, "setToolbarCollapsed", SetCollapsed));

            Mod.RegisterCalmToolbar(this);
            Mod.Log.Info("[UI] Calm toolbar bridge initialised.");
        }

        /// <summary>Called when the player changes the option, and once at load.</summary>
        public void ApplySettings()
        {
            _buttonVisibleBinding.Update(ButtonVisible());
            _collapsedBinding.Update(Collapsed());

            // With no way back on screen, a folded toolbar would be information taken away rather
            // than put down. Hiding the control therefore unfolds it.
            if (!ButtonVisible() && Collapsed())
            {
                SetCollapsed(false);
            }
        }

        private void SetCollapsed(bool collapsed)
        {
            try
            {
                var settings = Mod.Settings;
                if (settings != null)
                {
                    settings.ToolbarCollapsed = collapsed;
                    settings.ApplyAndSave();
                }

                _collapsedBinding.Update(collapsed);
                Mod.Log.Info(LogPrefix + "Toolbar " + (collapsed ? "folded." : "unfolded."));
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not change the toolbar fold.");
            }
        }

        private static bool Collapsed()
        {
            var settings = Mod.Settings;
            return settings != null && settings.ToolbarCollapsed;
        }

        private static bool ButtonVisible()
        {
            var settings = Mod.Settings;
            return settings == null || settings.ShowToolbarCollapseButton;
        }
    }
}

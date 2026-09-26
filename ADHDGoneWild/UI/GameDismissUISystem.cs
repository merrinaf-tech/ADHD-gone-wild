using System;
using System.Collections.Generic;
using Colossal.UI.Binding;
using Game.Input;
using Game.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace ADHDGoneWild.UI
{
    /// <summary>
    /// Carries the game's real Tool / Cancel action into the mod UI.
    ///
    /// Gameface's <c>Default Tool UI</c> action is contextual and is not emitted for a component
    /// appended to the global Game surface. The game also blocks mouse Tool actions while the
    /// pointer is over UI, so WasPerformedThisFrame cannot close a panel. What survives both is
    /// the binding itself: read the buttons the player has actually bound, without enabling or
    /// consuming the blocked tool action.
    ///
    /// <para>
    /// The buttons come from <see cref="ProxyAction.bindings"/>, which is public and already
    /// carries the player's overrides in <see cref="ProxyBinding.path"/> - "&lt;Mouse&gt;/middleButton"
    /// for somebody who put Cancel on the wheel. The action's underlying InputAction would give
    /// its resolved controls directly, but <c>sourceAction</c> is not public, and reaching it
    /// through reflection to save one path lookup is a debt that comes due at the next patch.
    /// </para>
    ///
    /// <para>
    /// Only mouse buttons are followed. Escape and the gamepad Back button already travel through
    /// the UI action consumer, and a second route for them would dismiss two surfaces per press.
    /// </para>
    /// </summary>
    public partial class GameDismissUISystem : UISystemBase
    {
        private const string Group = "adhd";
        private const string LogPrefix = "[UI] ";

        private ProxyAction _cancelAction;
        private ValueBinding<int> _revisionBinding;
        private int _revision;
        private bool _lookupFailureLogged;

        /// <summary>
        /// The mouse buttons Cancel is bound to right now, resolved once rather than per frame.
        /// </summary>
        private readonly List<ButtonControl> _buttons = new List<ButtonControl>();

        /// <summary>
        /// The game's own counter of "the bindings have changed". Rebinding Cancel in the options
        /// moves it, which is the signal to look the buttons up again - without it this would
        /// follow whatever was bound when the city loaded until the next restart.
        /// </summary>
        private int _boundAtActionVersion = -1;

        protected override void OnCreate()
        {
            base.OnCreate();

            _revisionBinding = new ValueBinding<int>(Group, "gameCancelRevision", 0);
            AddBinding(_revisionBinding);

            FindCancelAction();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (_cancelAction == null)
            {
                FindCancelAction();
                return;
            }

            RefreshButtonsIfBindingsChanged();

            for (var i = 0; i < _buttons.Count; i++)
            {
                if (_buttons[i].wasPressedThisFrame)
                {
                    _revisionBinding.Update(++_revision);
                    break;
                }
            }
        }

        private void FindCancelAction()
        {
            try
            {
                var input = InputManager.instance;
                if (input == null)
                {
                    return;
                }

                _cancelAction = input.FindAction("Tool", "Cancel");

                if (_cancelAction != null)
                {
                    _lookupFailureLogged = false;
                    _boundAtActionVersion = -1;
                    Mod.Log.Info(LogPrefix + "Following the vanilla Tool / Cancel binding.");
                }
            }
            catch (Exception e)
            {
                // Input can be incomplete while the world is coming up. Retry silently after the
                // first useful diagnostic instead of turning a temporary startup state into spam.
                if (!_lookupFailureLogged)
                {
                    _lookupFailureLogged = true;
                    Mod.Log.Warn(LogPrefix + "Could not find Tool / Cancel yet: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Resolve the bound mouse buttons, and only when something has actually been rebound.
        /// </summary>
        private void RefreshButtonsIfBindingsChanged()
        {
            try
            {
                var input = InputManager.instance;
                if (input == null || input.actionVersion == _boundAtActionVersion)
                {
                    return;
                }

                _boundAtActionVersion = input.actionVersion;
                _buttons.Clear();

                foreach (var binding in _cancelAction.bindings)
                {
                    if (!binding.isMouse || !binding.isSet || string.IsNullOrEmpty(binding.path))
                    {
                        continue;
                    }

                    var button = InputSystem.FindControl(binding.path) as ButtonControl;
                    if (button != null && !_buttons.Contains(button))
                    {
                        _buttons.Add(button);
                    }
                }

                Mod.Log.Info(
                    LogPrefix + "Cancel is on " + _buttons.Count + " mouse button(s).");
            }
            catch (Exception e)
            {
                // A binding that cannot be resolved costs this one route out of a panel. Escape
                // still works, so this is a smaller thing than an exception every frame.
                _buttons.Clear();
                Mod.Log.Warn(LogPrefix + "Could not read the Cancel binding: " + e.Message);
            }
        }
    }
}

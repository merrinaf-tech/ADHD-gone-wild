using System;
using ADHDGoneWild.Core;
using ADHDGoneWild.Memory;
using ADHDGoneWild.Settings;
using Colossal.UI;
using Colossal.UI.Binding;
using Game.Input;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
using Game.UI;
using UnityEngine;

namespace ADHDGoneWild.BrainParking
{
    /// <summary>
    /// The bridge between parked ideas and the panel that shows them.
    ///
    /// Watches for the shortcut, turns a screen click into a world position, keeps the UI's copy
    /// of the idea list current, and moves the camera when the player picks one. The rules live in
    /// <see cref="CityMemory"/>, which knows about none of this.
    ///
    /// Parking mode itself is pure UI state - a bool the panel shows a hint for - rather than a
    /// game tool. See <see cref="GroundPoint"/> for why.
    /// </summary>
    public partial class BrainParkingUISystem : UISystemBase
    {
        private const string Group = "adhd";
        private const string LogPrefix = "[BrainParking] ";

        private CameraUpdateSystem _camera;
        private TerrainSystem _terrain;
        private CityMemorySystem _memory;

        private ValueBinding<bool> _parkingBinding;

        /// <summary>
        /// The idea just parked, or empty. The UI offers to refine it while this is set - the
        /// idea is already saved by then, so the offer can always be ignored.
        /// </summary>
        private ValueBinding<string> _justParkedBinding;
        private ValueBinding<bool> _enabledBinding;
        private RawValueBinding _ideasBinding;

        /// <summary>
        /// An idea the player clicked the ring of, or empty. The panel opens on it and then
        /// clears this, so it is a nudge rather than a selection the mod holds on to.
        /// </summary>
        private ValueBinding<string> _focusedIdeaBinding;

        private ToolSystem _tools;

        private ProxyAction _parkAction;

        protected override void OnCreate()
        {
            base.OnCreate();

            _memory = World.GetOrCreateSystemManaged<CityMemorySystem>();
            _memory.CityChanged += OnCityChanged;

            _camera = World.GetOrCreateSystemManaged<CameraUpdateSystem>();
            _terrain = World.GetOrCreateSystemManaged<TerrainSystem>();
            _tools = World.GetOrCreateSystemManaged<ToolSystem>();

            _parkingBinding = new ValueBinding<bool>(Group, "parking", false);
            AddBinding(_parkingBinding);

            _justParkedBinding = new ValueBinding<string>(Group, "justParked", string.Empty);
            AddBinding(_justParkedBinding);

            _enabledBinding = new ValueBinding<bool>(Group, "brainParkingEnabled", true);
            AddBinding(_enabledBinding);

            _ideasBinding = new RawValueBinding(Group, "ideas", WriteIdeas);
            AddBinding(_ideasBinding);

            _focusedIdeaBinding = new ValueBinding<string>(Group, "focusedIdea", string.Empty);
            AddBinding(_focusedIdeaBinding);

            AddBinding(new TriggerBinding(Group, "startParking", StartParking));
            AddBinding(new TriggerBinding(Group, "cancelParking", CancelParking));
            AddBinding(new TriggerBinding<float, float>(Group, "parkAt", ParkAt));
            AddBinding(new TriggerBinding(Group, "dismissJustParked", DismissJustParked));
            AddBinding(new TriggerBinding<string>(Group, "forgetIdea", ForgetIdea));
            AddBinding(new TriggerBinding<string>(Group, "jumpToIdea", JumpToIdea));
            AddBinding(new TriggerBinding<string, string>(Group, "setIdeaNote", SetIdeaNote));
            AddBinding(new TriggerBinding<string, int>(Group, "setIdeaCategory", SetIdeaCategory));
            AddBinding(new TriggerBinding<float, float>(Group, "clickWorld", ClickWorld));
            AddBinding(new TriggerBinding(Group, "clearIdeaFocus", ClearIdeaFocus));

            Mod.RegisterBrainParking(this);
            Mod.Log.Info("[UI] Brain Parking bridge initialised.");
        }

        private void OnCityChanged()
        {
            _ideasBinding.Update();
            ClearIdeaFocus();
            DismissJustParked();
            ApplySettings();
        }

        /// <summary>Called by the settings when the player switches the feature or its key.</summary>
        public void ApplySettings()
        {
            var settings = Mod.Settings;
            var on = settings == null || settings.BrainParkingEnabled;

            _enabledBinding.Update(on);
            SetActionEnabled(_memory.InGame && on);

            if (!on)
            {
                CancelParking();
                DismissJustParked();
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (!_memory.InGame || _parkAction == null)
            {
                return;
            }

            // One input read per UI frame. Everything else here is event driven.
            if (_parkAction.WasPerformedThisFrame())
            {
                if (_parkingBinding.value)
                {
                    CancelParking();
                }
                else
                {
                    StartParking();
                }
            }
        }

        private void StartParking()
        {
            if (!_memory.InGame || !_memory.Memory.IsLoaded)
            {
                return;
            }

            var settings = Mod.Settings;
            if (settings != null && !settings.BrainParkingEnabled)
            {
                return;
            }

            _parkingBinding.Update(true);
        }

        private void CancelParking()
        {
            _parkingBinding.Update(false);
        }

        /// <summary>
        /// The player clicked while parking was on. <paramref name="screenX"/>/<paramref name="screenY"/>
        /// are DOM client coordinates from the React side - see <see cref="GroundPoint"/>.
        /// </summary>
        private void ParkAt(float screenX, float screenY)
        {
            // Logged before any guard: if this line is missing from a session where the player
            // clicked, the click never reached C# at all, which is a different problem from one
            // that reached it and was rejected. One line per click, not per frame.
            Mod.Log.Info(LogPrefix + "Click received at screen " + screenX + "," + screenY +
                         " (parking=" + _parkingBinding.value + ").");

            if (!_parkingBinding.value)
            {
                return;
            }

            // Ends the interaction either way: a click that misses the ground is still a click,
            // and leaving parking mode stuck on is worse than one thought not landing.
            CancelParking();

            try
            {
                var settings = Mod.Settings;
                var category = settings == null ? IdeaCategory.Idea : settings.DefaultIdeaCategory;

                WorldPoint point;
                if (!GroundPoint.TryFromScreen(ActiveCamera(), _terrain, screenX, screenY, out point))
                {
                    Mod.Log.Info(LogPrefix + "Click at " + screenX + "," + screenY + " did not hit the ground.");
                    return;
                }

                var idea = _memory.Memory.Park(point, category);
                _ideasBinding.Update();

                // Already saved. What follows is an offer, never a step the player has to
                // complete - and a player who wants parking at its shortest can switch the offer
                // off entirely.
                if (settings == null || settings.AskAfterParking)
                {
                    _justParkedBinding.Update(idea.Id);
                }
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not park an idea at screen " + screenX + "," + screenY + ".");
            }
        }

        /// <summary>
        /// Every click on the map, offered to the ideas parked in it. If one of them was drawn
        /// under the cursor, the panel is asked to open on it.
        ///
        /// This runs on ordinary clicks, so the guards are the feature. Getting any of them wrong
        /// would not break the mod - it would make the game feel haunted, which is worse, because
        /// nobody reports a click that also did something else:
        ///
        /// - <c>mouseOverUI</c> is the game's own answer to "is the cursor on the interface", and
        ///   the same one <see cref="ToolSystem"/> uses to decide whether to block its input. A
        ///   click on any panel - ours, vanilla's, another mod's - is not a click on the map.
        /// - Only with the default tool active. While a road or a zone is being drawn, a click
        ///   belongs to that tool and to nothing else.
        /// - Only when the markers are actually drawn. Clicking a ring that is switched off is
        ///   not something a player can mean to do.
        ///
        /// Nothing here consumes the click. The world still receives it; this only listens.
        /// </summary>
        private void ClickWorld(float screenX, float screenY)
        {
            if (!_memory.InGame || !_memory.Memory.IsLoaded || _parkingBinding.value)
            {
                return;
            }

            var settings = Mod.Settings;
            if (settings != null && (!settings.BrainParkingEnabled || !settings.ShowIdeaMarkers))
            {
                return;
            }

            try
            {
                if (InputManager.instance != null && InputManager.instance.mouseOverUI)
                {
                    return;
                }

                if (_tools != null && !(_tools.activeTool is DefaultToolSystem))
                {
                    return;
                }

                WorldPoint point;
                if (!GroundPoint.TryFromScreen(ActiveCamera(), _terrain, screenX, screenY, out point))
                {
                    return;
                }

                var hit = IdeaHitTest.Nearest(
                    _memory.Memory.Ideas,
                    point,
                    IdeaMarker.Radius);

                if (hit == null)
                {
                    return;
                }

                _focusedIdeaBinding.Update(hit.Id);
                Mod.Log.Info(LogPrefix + "Ring clicked: opening idea " + hit.Id + ".");
            }
            catch (Exception e)
            {
                // A click that cannot be resolved is a click that does nothing, which is what it
                // would have done anyway. It must never reach the player as an error.
                Mod.Log.Error(e, LogPrefix + "Could not test the click at " + screenX + "," + screenY + ".");
            }
        }

        /// <summary>The panel has opened on it. The mod stops pointing.</summary>
        private void ClearIdeaFocus()
        {
            _focusedIdeaBinding.Update(string.Empty);
        }

        private void DismissJustParked()
        {
            _justParkedBinding.Update(string.Empty);
        }

        private void ForgetIdea(string id)
        {
            // An idea being pointed at that no longer exists would leave the panel opening on
            // nothing for ever.
            if (_focusedIdeaBinding.value == id)
            {
                ClearIdeaFocus();
            }

            try
            {
                if (_memory.Memory.Forget(id))
                {
                    _ideasBinding.Update();
                    DismissJustParked();
                }
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not forget idea '" + id + "'.");
            }
        }

        private void SetIdeaNote(string id, string note)
        {
            try
            {
                if (_memory.Memory.SetNote(id, note))
                {
                    _ideasBinding.Update();
                }
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not write the note on idea '" + id + "'.");
            }
        }

        private void SetIdeaCategory(string id, int category)
        {
            try
            {
                if (_memory.Memory.SetCategory(id, (IdeaCategory)category))
                {
                    _ideasBinding.Update();
                }
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not recategorise idea '" + id + "'.");
            }
        }

        private void JumpToIdea(string id)
        {
            var idea = _memory.Memory.FindIdea(id);
            if (idea != null)
            {
                CameraJump.To(_camera, idea.Position);
            }
        }

        private void SetActionEnabled(bool enabled)
        {
            try
            {
                if (_parkAction == null)
                {
                    var settings = Mod.Settings;
                    if (settings == null)
                    {
                        return;
                    }

                    _parkAction = settings.GetAction(AdhdSettings.ParkIdeaActionName);
                }

                if (_parkAction != null)
                {
                    _parkAction.shouldBeEnabled = enabled;
                }
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not change the state of the parking shortcut.");
            }
        }

        /// <summary>The camera the player is actually looking through, matching what they clicked on.</summary>
        private static Camera ActiveCamera()
        {
            var view = UIManager.defaultUISystem == null ? null : UIManager.defaultUISystem.defaultUIView;
            var rendering = view == null ? null : view.RenderingCamera;
            return rendering != null ? rendering : Camera.main;
        }

        private void WriteIdeas(IJsonWriter writer)
        {
            var ideas = _memory.Memory.Ideas;
            writer.ArrayBegin((uint)ideas.Count);

            for (var i = 0; i < ideas.Count; i++)
            {
                var idea = ideas[i];

                writer.TypeBegin("adhd.Idea");

                writer.PropertyName("id");
                writer.Write(idea.Id);

                writer.PropertyName("category");
                writer.Write((int)idea.Category);

                writer.PropertyName("note");
                writer.Write(idea.Note);

                writer.PropertyName("createdUnixUtc");
                writer.Write(CityMemory.ToUnix(idea.CreatedUtc));

                writer.PropertyName("x");
                writer.Write(idea.Position.X);

                writer.PropertyName("z");
                writer.Write(idea.Position.Z);

                writer.TypeEnd();
            }

            writer.ArrayEnd();
        }
    }
}

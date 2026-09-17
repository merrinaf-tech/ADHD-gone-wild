using System;
using ADHDGoneWild.Core;
using ADHDGoneWild.Memory;
using Colossal.UI.Binding;
using Game.Rendering;
using Game.Tools;
using Game.UI;

namespace ADHDGoneWild.Trail
{
    /// <summary>
    /// "Where was I?" - the thin half.
    ///
    /// Everything this does is read four things once a second and hand them to
    /// <see cref="AttentionTrail"/>, which owns every decision. Nothing here judges, thresholds or
    /// merges: if a rule is worth arguing about it lives in the pure class, where a test can hold
    /// it to account.
    ///
    /// The four things are the camera's pivot, its zoom, its angle, and the name of the type of
    /// tool the player is holding. No entity is touched, no query is run, nothing is written
    /// anywhere - which is why this is the cheapest system in the mod and the one least able to
    /// break a save.
    ///
    /// It is also the only feature here that keeps nothing at all. The trail is cleared when a
    /// city closes, and it is never written to disk. Welcome Back is the memory between sessions;
    /// this is the memory inside one, and a memory that cannot outlive the session cannot turn
    /// into a list of things waiting for you tomorrow.
    /// </summary>
    public partial class TrailUISystem : UISystemBase
    {
        private const string Group = "adhd";
        private const string LogPrefix = "[Trail] ";

        /// <summary>
        /// One sample a second. Fine enough that dwelling is measured honestly, coarse enough that
        /// the cost is a handful of property reads a second and nothing else.
        /// </summary>
        private const float SampleIntervalSeconds = 1f;

        private readonly AttentionTrail _trail = new AttentionTrail();

        private CameraUpdateSystem _camera;
        private ToolSystem _tools;
        private CityMemorySystem _memory;

        private RawValueBinding _binding;
        private ValueBinding<bool> _enabledBinding;

        private float _nextSampleAt;

        /// <summary>
        /// Whether this system actually came up. Everything below checks it.
        ///
        /// The newest feature in the mod, and the one nobody has yet watched run, is not allowed
        /// to be the reason somebody's whole mod fails to load. An exception thrown out of
        /// OnCreate does exactly that - the world stops building the system and the player gets a
        /// mod that is simply not there, with no clue which part of it was at fault. So this one
        /// is allowed to fail on its own: the trail goes quiet, a line goes in the log, and every
        /// other feature carries on as if it had never been written.
        ///
        /// It comes out once the feature has run in a real city for a while.
        /// </summary>
        private bool _ready;

        protected override void OnCreate()
        {
            base.OnCreate();

            try
            {
                _camera = World.GetOrCreateSystemManaged<CameraUpdateSystem>();
                _tools = World.GetOrCreateSystemManaged<ToolSystem>();
                _memory = World.GetOrCreateSystemManaged<CityMemorySystem>();

                _binding = new RawValueBinding(Group, "trail", Write);
                AddBinding(_binding);

                _enabledBinding = new ValueBinding<bool>(Group, "trailEnabled", FeatureEnabled());
                AddBinding(_enabledBinding);

                AddBinding(new TriggerBinding<string>(Group, "jumpToTrailPlace", JumpTo));
                AddBinding(new TriggerBinding<string>(Group, "forgetTrailPlace", Forget));

                // A different city is a different set of places. Nothing carries over, and nothing
                // is saved, so this is the whole of the lifecycle.
                _memory.CityChanged += OnCityChanged;

                _ready = true;

                Mod.RegisterTrail(this);
                Mod.Log.Info("[UI] Trail bridge initialised.");
            }
            catch (Exception e)
            {
                _ready = false;
                Mod.Log.Error(e, LogPrefix + "Could not start. The rest of the mod is unaffected.");
            }
        }

        protected override void OnDestroy()
        {
            if (_ready && _memory != null)
            {
                _memory.CityChanged -= OnCityChanged;
            }

            base.OnDestroy();
        }

        /// <summary>Called when the player changes the option.</summary>
        public void ApplySettings()
        {
            if (!_ready)
            {
                return;
            }

            _enabledBinding.Update(FeatureEnabled());

            // Switching it off empties the trail rather than hiding it. A player who turns this
            // feature off has said they do not want their movements remembered, and keeping the
            // list in memory against the day they turn it back on would not honour that.
            if (!FeatureEnabled())
            {
                _trail.Clear();
            }

            _binding.Update();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            if (!_ready || !FeatureEnabled() || !_memory.InGame)
            {
                return;
            }

            var now = UnityEngine.Time.realtimeSinceStartup;

            if (now < _nextSampleAt)
            {
                return;
            }

            _nextSampleAt = now + SampleIntervalSeconds;

            Sample();
        }

        private void Sample()
        {
            WorldPoint pivot;
            float zoom, rotationX, rotationY, rotationZ;

            if (!CameraJump.TryGetFraming(_camera, out pivot, out zoom, out rotationX, out rotationY, out rotationZ))
            {
                return;
            }

            var sample = new TrailSample(
                DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                pivot.X,
                pivot.Y,
                pivot.Z,
                zoom,
                rotationX,
                rotationY,
                rotationZ,
                Activity());

            var previousRevision = _trail.VisibleRevision;
            _trail.Observe(sample);

            // Revisiting a visible place changes its position, activity and last visit. Moving
            // away from one also reveals it as a way back. Other one-second candidates leave the
            // visible list alone, so they do not redraw the panel.
            if (_trail.VisibleRevision != previousRevision)
            {
                _binding.Update();
            }
        }

        /// <summary>
        /// What the player is holding, as one of the mod's own plain words.
        ///
        /// The type's name rather than the tool's own <c>toolID</c>: the names are readable
        /// straight out of the game assembly and were checked there, while every toolID lives
        /// inside a property body that cannot be inspected without decompiling.
        /// </summary>
        private TrailActivity Activity()
        {
            try
            {
                var tool = _tools == null ? null : _tools.activeTool;
                return tool == null ? TrailActivity.Looking : TrailActivities.From(tool.GetType().Name);
            }
            catch (Exception e)
            {
                Mod.Log.Warn(LogPrefix + "Could not read the active tool: " + e.Message);
                return TrailActivity.Looking;
            }
        }

        private void OnCityChanged()
        {
            _trail.Clear();
            _binding.Update();
        }

        /// <summary>Back to a view the player had earlier, framing and all.</summary>
        private void JumpTo(string id)
        {
            var place = _trail.Find(id);

            if (place == null)
            {
                return;
            }

            CameraJump.ToFraming(
                _camera,
                new WorldPoint(place.PivotX, place.PivotY, place.PivotZ),
                place.Zoom,
                place.RotationX,
                place.RotationY,
                place.RotationZ);
        }

        /// <summary>"Not that one." The trail is the player's, so they get to edit it.</summary>
        private void Forget(string id)
        {
            if (_trail.Forget(id))
            {
                _binding.Update();
            }
        }

        private static bool FeatureEnabled()
        {
            var settings = Mod.Settings;
            return settings != null && settings.AttentionTrailEnabled;
        }

        private void Write(IJsonWriter writer)
        {
            // Most recent first, except the place the camera is still looking at. It remains in
            // memory and returns to this list as soon as the player moves away. The trail itself
            // is kept oldest first, because that is the order attention actually happened in.
            var places = new System.Collections.Generic.List<TrailPlace>(_trail.PlacesAwayFromCurrentView);

            writer.ArrayBegin((uint)places.Count);

            for (var i = places.Count - 1; i >= 0; i--)
            {
                var place = places[i];

                writer.TypeBegin("adhd.TrailPlace");

                writer.PropertyName("id");
                writer.Write(place.Id);

                writer.PropertyName("activities");
                writer.Write(place.ActivityMask);

                writer.PropertyName("lastSeenUnixUtc");
                writer.Write(place.LastSeenUnixUtc);

                writer.PropertyName("x");
                writer.Write(place.X);

                writer.PropertyName("z");
                writer.Write(place.Z);

                writer.TypeEnd();
            }

            writer.ArrayEnd();
        }
    }
}

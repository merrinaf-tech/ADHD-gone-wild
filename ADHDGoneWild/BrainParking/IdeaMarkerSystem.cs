using System;
using ADHDGoneWild.Memory;
using Game;
using Game.Rendering;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace ADHDGoneWild.BrainParking
{
    /// <summary>
    /// Draws a ring on the ground where each idea was parked.
    ///
    /// This is most of the point of parking a thought <em>somewhere</em>: a list in a panel says
    /// you had an idea, but only a mark on the map says you had it <em>here</em>. Without it the
    /// place the player deliberately chose is thrown away the moment they look away.
    ///
    /// Drawn through <see cref="OverlayRenderSystem"/>, which paints into a per-frame buffer and
    /// creates no entities: nothing is added to the world, nothing reaches a save, and the marks
    /// vanish the instant the feature is switched off.
    /// </summary>
    public partial class IdeaMarkerSystem : GameSystemBase
    {
        private const string LogPrefix = "[BrainParking] ";

        /// <summary>Ground diameter of a marker, in metres. Readable when zoomed out, not a blob when close.</summary>


        private const float OutlineWidth = 3f;

        private CityMemorySystem _memory;
        private OverlayRenderSystem _overlay;

        protected override void OnCreate()
        {
            base.OnCreate();

            _memory = World.GetOrCreateSystemManaged<CityMemorySystem>();
            _overlay = World.GetOrCreateSystemManaged<OverlayRenderSystem>();
        }

        protected override void OnUpdate()
        {
            if (!_memory.InGame || !_memory.Memory.IsLoaded)
            {
                return;
            }

            var settings = Mod.Settings;
            if (settings != null && (!settings.BrainParkingEnabled || !settings.ShowIdeaMarkers))
            {
                return;
            }

            var ideas = _memory.Memory.Ideas;
            if (ideas.Count == 0)
            {
                return;
            }

            try
            {
                JobHandle dependencies;
                var buffer = _overlay.GetBuffer(out dependencies);

                // The buffer is filled here on the main thread rather than from a job: the number
                // of markers is however many thoughts one player has parked in one city, which is
                // dozens at most, and a job would cost more to schedule than to skip.
                dependencies.Complete();

                for (var i = 0; i < ideas.Count; i++)
                {
                    var idea = ideas[i];
                    var p = idea.Position;

                    float r, g, b;
                    IdeaColours.Get(idea.Category, out r, out g, out b);

                    buffer.DrawCircle(
                        new Color(r, g, b, 0.95f),
                        new Color(r, g, b, 0.20f),
                        OutlineWidth,
                        // Projected, so the ring lies on the ground and follows a slope instead of
                        // floating as a flat disc through it.
                        OverlayRenderSystem.StyleFlags.Projected,
                        new float2(0f, 1f),
                        new float3(p.X, p.Y, p.Z),
                        IdeaMarker.Diameter);
                }
            }
            catch (Exception e)
            {
                // A frame without markers is a cosmetic loss. It must never take the city with it.
                Mod.Log.Error(e, LogPrefix + "Could not draw the idea markers this frame.");
            }
        }

    }
}

using Game.Simulation;
using Unity.Mathematics;
using UnityEngine;

namespace ADHDGoneWild.Core
{
    /// <summary>
    /// Turns a point on screen into a point on the ground.
    ///
    /// The game's own tool-raycast pipeline (<c>Game.Tools.ToolBaseSystem</c>) is built for this,
    /// but Brain Parking does not use it: after extensive testing, clicks through a custom
    /// <c>ToolBaseSystem</c> could not be made to register reliably, for reasons that did not
    /// resolve even after checking the engine's own decompiled activation and raycast code. A
    /// second mod in this workspace solves the same problem by projecting a screen point through
    /// the camera directly and sampling the terrain height at that spot - proven working - so
    /// Brain Parking uses the same technique rather than a second attempt at the tool pipeline.
    ///
    /// The trade-off, accepted for now: this bypasses the game's own UI-vs-world click routing,
    /// so while parking is active a click anywhere - including over another mod's panel - is
    /// read as a world position. Parking mode is short-lived and always cancellable, which is
    /// judged an acceptable cost for an interaction that actually works. See
    /// docs/TECHNICAL_FINDINGS.md.
    /// </summary>
    public static class GroundPoint
    {
        /// <summary>
        /// <paramref name="screenX"/>/<paramref name="screenY"/> are DOM client coordinates (origin
        /// top-left, as a React onClick reports them) - not Unity screen space (origin bottom-left).
        /// The flip happens here so callers never have to think about it.
        /// </summary>
        public static bool TryFromScreen(Camera camera, TerrainSystem terrain, float screenX, float screenY, out WorldPoint point)
        {
            point = default(WorldPoint);

            if (camera == null || terrain == null)
            {
                return false;
            }

            var ray = camera.ScreenPointToRay(new Vector3(screenX, camera.pixelHeight - screenY, 0f));

            // A ray parallel to the ground never crosses it.
            if (Mathf.Abs(ray.direction.y) < 0.0001f)
            {
                return false;
            }

            // First pass: intersect the sea-level plane to get an approximate XZ to sample the
            // real terrain height at. Second pass: intersect the plane at that actual height, so
            // the result sits on the ground rather than floating above or sinking into a slope.
            var t0 = -ray.origin.y / ray.direction.y;
            if (t0 < 0f)
            {
                return false;
            }

            var approx = ray.origin + ray.direction * t0;

            var heightData = terrain.GetHeightData();
            var terrainY = TerrainUtils.SampleHeight(ref heightData, new float3(approx.x, 0f, approx.z));

            var dt = ray.origin.y - terrainY;
            var t1 = dt / -ray.direction.y;
            var hit = t1 > 0f ? ray.origin + ray.direction * t1 : approx;

            point = new WorldPoint(hit.x, terrainY, hit.z);
            return true;
        }
    }
}

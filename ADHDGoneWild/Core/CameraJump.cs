using System;
using Game.Rendering;

namespace ADHDGoneWild.Core
{
    /// <summary>
    /// Taking the player somewhere, gently.
    ///
    /// Only the pivot moves. Zoom and angle stay exactly as they left them, because arriving in
    /// an unfamiliar framing means rebuilding your bearings before you can look at the thing you
    /// asked to see - which is the precise cost this mod exists to remove.
    ///
    /// Shared by every feature that can say "show me": parked ideas today, alerts now, whatever
    /// comes next.
    /// </summary>
    public static class CameraJump
    {
        public static bool To(CameraUpdateSystem camera, WorldPoint point)
        {
            try
            {
                if (camera == null || camera.activeCameraController == null)
                {
                    return false;
                }

                camera.activeCameraController.pivot = new UnityEngine.Vector3(point.X, point.Y, point.Z);
                return true;
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, "[Camera] Could not move to " + point + ".");
                return false;
            }
        }

        public static bool TryGetPivot(CameraUpdateSystem camera, out WorldPoint point)
        {
            point = default(WorldPoint);

            try
            {
                if (camera == null || camera.activeCameraController == null)
                {
                    return false;
                }

                var pivot = camera.activeCameraController.pivot;
                point = new WorldPoint(pivot.x, pivot.y, pivot.z);
                return true;
            }
            catch (Exception e)
            {
                Mod.Log.Warn("[Camera] Could not read the camera pivot: " + e.Message);
                return false;
            }
        }
    }
}

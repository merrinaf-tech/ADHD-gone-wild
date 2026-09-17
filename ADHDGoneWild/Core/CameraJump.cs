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

        /// <summary>
        /// Putting the player back exactly as they left it - pivot, zoom and angle together.
        ///
        /// The deliberate opposite of <see cref="To"/>, and the difference is the point. Sending
        /// somebody to a parked idea or an alert is showing them a *place*, and re-framing the
        /// view there would cost them their bearings on the way. Going back down their own trail
        /// is returning to a *view* they had twenty minutes ago, and arriving at the same spot
        /// from a different height and angle would not be the same thing at all.
        ///
        /// Every property here is writable on the game's own camera controller interface, which
        /// was checked rather than assumed. If any of it fails, the pivot has usually landed
        /// already and the player is at least in the right place.
        /// </summary>
        public static bool ToFraming(
            CameraUpdateSystem camera,
            WorldPoint pivot,
            float zoom,
            float rotationX,
            float rotationY,
            float rotationZ)
        {
            try
            {
                if (camera == null || camera.activeCameraController == null)
                {
                    return false;
                }

                var controller = camera.activeCameraController;

                controller.pivot = new UnityEngine.Vector3(pivot.X, pivot.Y, pivot.Z);
                controller.rotation = new UnityEngine.Vector3(rotationX, rotationY, rotationZ);
                controller.zoom = zoom;

                return true;
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, "[Camera] Could not restore the view at " + pivot + ".");
                return false;
            }
        }

        /// <summary>
        /// The whole framing as it is right now. Returns false rather than a half-read one, so a
        /// caller never records a pivot with somebody else's zoom on it.
        /// </summary>
        public static bool TryGetFraming(
            CameraUpdateSystem camera,
            out WorldPoint pivot,
            out float zoom,
            out float rotationX,
            out float rotationY,
            out float rotationZ)
        {
            pivot = default(WorldPoint);
            zoom = 0f;
            rotationX = 0f;
            rotationY = 0f;
            rotationZ = 0f;

            try
            {
                if (camera == null || camera.activeCameraController == null)
                {
                    return false;
                }

                var controller = camera.activeCameraController;
                var p = controller.pivot;
                var r = controller.rotation;

                pivot = new WorldPoint(p.x, p.y, p.z);
                zoom = controller.zoom;
                rotationX = r.x;
                rotationY = r.y;
                rotationZ = r.z;

                return true;
            }
            catch (Exception e)
            {
                Mod.Log.Warn("[Camera] Could not read the camera framing: " + e.Message);
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

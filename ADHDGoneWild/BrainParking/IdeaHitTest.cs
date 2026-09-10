using System.Collections.Generic;
using ADHDGoneWild.Core;

namespace ADHDGoneWild.BrainParking
{
    /// <summary>
    /// Which parked idea, if any, a click on the map landed on.
    ///
    /// Measured on the ground plane only - the height is thrown away. The marker is drawn
    /// projected, so on a hillside the ring lies along the slope while the point the click
    /// resolves to is wherever the terrain happens to be; comparing heights as well would make
    /// the rings on any slope quietly unclickable, which is exactly the sort of failure nobody
    /// would report because it looks like nothing happening.
    ///
    /// Nearest wins rather than first found. Two rings can overlap, and when they do the player
    /// meant the one they clicked closer to the middle of.
    /// </summary>
    public static class IdeaHitTest
    {
        /// <summary>
        /// The idea whose marker contains <paramref name="point"/>, or null. <paramref name="radius"/>
        /// is half the drawn diameter - see <see cref="IdeaMarkerSystem.MarkerDiameter"/>.
        /// </summary>
        public static Idea Nearest(IList<Idea> ideas, WorldPoint point, float radius)
        {
            if (ideas == null || ideas.Count == 0 || radius <= 0f)
            {
                return null;
            }

            // Compared squared, so nothing has to take a square root per idea per click.
            var bestDistance = radius * radius;
            Idea best = null;

            for (var i = 0; i < ideas.Count; i++)
            {
                var idea = ideas[i];
                if (idea == null)
                {
                    continue;
                }

                var dx = idea.Position.X - point.X;
                var dz = idea.Position.Z - point.Z;
                var distance = (dx * dx) + (dz * dz);

                // Strictly closer: a tie keeps the earlier one, so the same click always picks the
                // same idea rather than depending on the order the store happens to hold them in.
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = idea;
                }
            }

            return best;
        }
    }
}

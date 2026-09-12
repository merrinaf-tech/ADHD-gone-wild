using System.Globalization;

namespace ADHDGoneWild.UI
{
    /// <summary>
    /// The one colour the player can change: the background of the mod's own panels.
    ///
    /// Only the hue is theirs. Saturation and lightness are fixed at the values the default was
    /// already using, which is the whole reason this is safe to offer at all - a free colour
    /// picker can produce a white panel with white text on it, and then the mod is broken in a way
    /// the player did to themselves and cannot see how to undo. Here every possible setting is a
    /// dark, muted surface that the existing text colours read against.
    ///
    /// The default is the hue of #223141 - `--commonDarkBlue`, the game's own variable for panel
    /// backgrounds. Leaving the slider alone leaves the mod matching the game, which is where it
    /// should start.
    ///
    /// Nothing here touches the status colours. Those carry meaning - what is immediate, what is
    /// the player's own - and a meaning that shifts with a decoration setting is not a meaning.
    /// </summary>
    public static class InterfaceColour
    {
        /// <summary>The hue of #223141, and the value the slider starts at.</summary>
        public const int DefaultHue = 211;

        /// <summary>Fixed. See the note above on why these are not the player's to set.</summary>
        private const double Saturation = 0.313;

        private const double Lightness = 0.194;

        /// <summary>Opaque enough that a moving map does not show through and distract.</summary>
        private const double Alpha = 0.98;

        /// <summary>
        /// The panel background, as a CSS colour the UI can drop straight into a style attribute.
        /// </summary>
        public static string Surface(int hue)
        {
            int r, g, b;
            ToRgb(Wrap(hue), Saturation, Lightness, out r, out g, out b);

            return "rgba(" + r.ToString(CultureInfo.InvariantCulture) + ", " +
                   g.ToString(CultureInfo.InvariantCulture) + ", " +
                   b.ToString(CultureInfo.InvariantCulture) + ", " +
                   Alpha.ToString("0.##", CultureInfo.InvariantCulture) + ")";
        }

        /// <summary>
        /// Hue is a circle, so 370 is 10 and -20 is 340. A setting saved by a future version with a
        /// wider range, or a corrupt file, produces a colour rather than an exception.
        /// </summary>
        public static int Wrap(int hue)
        {
            var wrapped = hue % 360;
            return wrapped < 0 ? wrapped + 360 : wrapped;
        }

        private static void ToRgb(int hue, double s, double l, out int r, out int g, out int b)
        {
            var c = (1.0 - System.Math.Abs((2.0 * l) - 1.0)) * s;
            var h = hue / 60.0;
            var x = c * (1.0 - System.Math.Abs((h % 2.0) - 1.0));
            var m = l - (c / 2.0);

            double r1, g1, b1;

            if (h < 1.0) { r1 = c; g1 = x; b1 = 0.0; }
            else if (h < 2.0) { r1 = x; g1 = c; b1 = 0.0; }
            else if (h < 3.0) { r1 = 0.0; g1 = c; b1 = x; }
            else if (h < 4.0) { r1 = 0.0; g1 = x; b1 = c; }
            else if (h < 5.0) { r1 = x; g1 = 0.0; b1 = c; }
            else { r1 = c; g1 = 0.0; b1 = x; }

            r = Round((r1 + m) * 255.0);
            g = Round((g1 + m) * 255.0);
            b = Round((b1 + m) * 255.0);
        }

        private static int Round(double value)
        {
            var rounded = (int)System.Math.Round(value);
            return rounded < 0 ? 0 : (rounded > 255 ? 255 : rounded);
        }
    }
}

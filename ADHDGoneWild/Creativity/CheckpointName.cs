using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ADHDGoneWild.Creativity
{
    /// <summary>
    /// Builds the name the player reads in their own list of saves.
    ///
    /// This is not an internal identifier. The mod finds its checkpoint again by the name it
    /// recorded in the city's memory, so nothing here has to be parseable - which leaves it free
    /// to be written for a person, and it has to be: the file sits in the same list as the saves
    /// they made themselves, and a row that reads like a machine's scratch file is a small
    /// unanswered question every time they scroll past it.
    ///
    /// It says, in the order of what survives being cut short by a narrow panel: which city, that
    /// it is a way back rather than something they made, when it was taken, and what made it.
    ///
    ///     Pittston - safety net - 04 September 14-32 (ADHD gone wild)
    ///
    /// What it leaves out is everything the game's own save panel already shows beside it -
    /// population, money, the date inside the city - because all of that travels in the SaveInfo
    /// we hand over, and repeating it here would crowd out the part that does not.
    ///
    /// It lives apart from <see cref="CheckpointSystem"/>, and takes the time as an argument
    /// rather than reading the clock, so it can be tested without the game running.
    /// </summary>
    public static class CheckpointName
    {
        /// <summary>How much of a city name the save name carries before it is cut short.</summary>
        public const int MaxCityNameLength = 40;

        /// <summary>
        /// Shown so a player meeting this file months later can tell what made it, and that
        /// deleting it costs them nothing. Not translated: it is the mod's name, not a sentence.
        /// </summary>
        public const string ModName = "ADHD gone wild";

        /// <summary>Used when the city has no name to give.</summary>
        public const string UnnamedCity = "City";

        /// <summary>
        /// <paramref name="marker"/> is the mod's own words for what this is, already translated.
        /// <paramref name="localNow"/> is local time, because it is being read by a person who
        /// was here when it happened.
        /// </summary>
        public static string Build(string cityName, string marker, DateTime localNow)
        {
            var city = FileSafe(cityName);
            if (city.Length == 0)
            {
                city = UnnamedCity;
            }

            var what = FileSafe(marker);
            if (what.Length == 0)
            {
                what = "safety net";
            }

            // Hyphens rather than colons: a save name becomes a file name, and Windows will not
            // have a colon in one. The game's own autosave writes its times the same way.
            var when = FileSafe(localNow.ToString("dd MMMM HH-mm", CultureInfo.CurrentCulture));

            return city + " - " + what + " - " + when + " (" + ModName + ")";
        }

        /// <summary>
        /// Strips what a file name cannot hold and tidies what it can. A player may call their
        /// city anything at all, including things this has to quietly drop.
        /// </summary>
        public static string FileSafe(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var invalid = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(value.Length);
            var lastWasSpace = false;

            foreach (var c in value)
            {
                if (builder.Length >= MaxCityNameLength)
                {
                    break;
                }

                if (Array.IndexOf(invalid, c) >= 0)
                {
                    continue;
                }

                if (char.IsWhiteSpace(c))
                {
                    // A run of spaces would survive into the file name and read as a mistake.
                    // Leading ones are dropped entirely.
                    if (builder.Length > 0 && !lastWasSpace)
                    {
                        builder.Append(' ');
                        lastWasSpace = true;
                    }

                    continue;
                }

                builder.Append(c);
                lastWasSpace = false;
            }

            // A trailing dot or space is legal to ask for and not legal to have.
            return builder.ToString().Trim().TrimEnd('.').Trim();
        }
    }
}

using System;
using System.Text;

namespace ADHDGoneWild.Persistence
{
    /// <summary>
    /// Turns a city name into a filename.
    ///
    /// The game exposes no stable identifier for the city being played - SaveInfo carries one but
    /// is not reachable from a loaded world, so the name is what there is. Two cities sharing a
    /// name therefore share a file, and renaming a city starts a fresh one. Both are recorded in
    /// docs/TECHNICAL_FINDINGS.md; neither can lose anything except this mod's own notes.
    ///
    /// The hash suffix keeps names that differ only in punctuation or case apart once the
    /// readable part has been stripped down to something a filesystem accepts.
    /// </summary>
    public static class CityKey
    {
        private const int MaxReadableLength = 40;

        public static string FromCityName(string cityName)
        {
            if (string.IsNullOrEmpty(cityName))
            {
                cityName = "unnamed";
            }

            var readable = new StringBuilder(MaxReadableLength);
            foreach (var c in cityName)
            {
                if (readable.Length >= MaxReadableLength)
                {
                    break;
                }

                if (char.IsLetterOrDigit(c))
                {
                    readable.Append(char.ToLowerInvariant(c));
                }
                else if (readable.Length > 0 && readable[readable.Length - 1] != '-')
                {
                    readable.Append('-');
                }
            }

            var stem = readable.ToString().Trim('-');
            if (stem.Length == 0)
            {
                stem = "city";
            }

            return stem + "-" + ShortHash(cityName);
        }

        /// <summary>
        /// FNV-1a over the original name. Deliberately not GetHashCode: that is allowed to differ
        /// between runs, and a key that changes between sessions would silently orphan a file.
        /// </summary>
        private static string ShortHash(string value)
        {
            unchecked
            {
                const uint offset = 2166136261;
                const uint prime = 16777619;

                var hash = offset;
                var bytes = Encoding.UTF8.GetBytes(value);
                for (var i = 0; i < bytes.Length; i++)
                {
                    hash ^= bytes[i];
                    hash *= prime;
                }

                return hash.ToString("x8");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Colossal;

namespace ADHDGoneWild.Localization
{
    /// <summary>
    /// Says, in the log, which lines a translation is missing.
    ///
    /// It exists because the failure it catches is silent. A key absent from a translation falls
    /// back to English, so a half-translated mod looks like a working one to whoever built it and
    /// like a sloppy one to whoever plays it. Nothing throws, nothing turns red, and the only way
    /// to find out is to read every panel in every language.
    ///
    /// This is the check the test suite cannot do. The locale sources need <c>AdhdSettings</c>,
    /// which needs the game, so they cannot be linked into a project that runs outside it - see
    /// tests/ADHDGoneWild.Tests. One line at load is the next best thing.
    ///
    /// English is the source language and therefore the reference. A key present in a translation
    /// but not in English is reported too: it means a key was renamed and one file was missed,
    /// and that entry is now dead weight nothing will ever read.
    /// </summary>
    public static class LocaleCoverage
    {
        /// <summary>How many missing keys to name before giving up and just counting them.</summary>
        private const int MaxNamed = 12;

        public static void Report(string language, IDictionarySource reference, IDictionarySource translation)
        {
            try
            {
                var expected = KeysOf(reference);
                var actual = KeysOf(translation);

                if (expected.Count == 0)
                {
                    return;
                }

                var missing = expected.Where(k => !actual.Contains(k)).ToList();
                var extra = actual.Where(k => !expected.Contains(k)).ToList();

                if (missing.Count == 0 && extra.Count == 0)
                {
                    Mod.Log.Info("[L10n] " + language + ": complete (" + expected.Count + " entries).");
                    return;
                }

                if (missing.Count > 0)
                {
                    Mod.Log.Warn("[L10n] " + language + " is missing " + missing.Count + " of " +
                                 expected.Count + " entries; they will show in English. " + Name(missing));
                }

                if (extra.Count > 0)
                {
                    Mod.Log.Warn("[L10n] " + language + " has " + extra.Count +
                                 " entries English does not, which nothing will read. " + Name(extra));
                }
            }
            catch (Exception e)
            {
                // A diagnostic that breaks the load would be worse than no diagnostic.
                Mod.Log.Warn("[L10n] Could not check " + language + ": " + e.Message);
            }
        }

        private static HashSet<string> KeysOf(IDictionarySource source)
        {
            var keys = new HashSet<string>();
            if (source == null)
            {
                return keys;
            }

            var entries = source.ReadEntries(new List<IDictionaryEntryError>(), new Dictionary<string, int>());
            if (entries == null)
            {
                return keys;
            }

            foreach (var entry in entries)
            {
                keys.Add(entry.Key);
            }

            return keys;
        }

        private static string Name(List<string> keys)
        {
            var named = string.Join(", ", keys.Take(MaxNamed).ToArray());
            return keys.Count > MaxNamed ? named + ", ..." : named;
        }
    }
}

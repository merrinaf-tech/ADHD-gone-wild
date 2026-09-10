using System;
using System.IO;
using ADHDGoneWild.Core;
using Newtonsoft.Json;

namespace ADHDGoneWild.Persistence
{
    /// <summary>
    /// The per-city file, as JSON next to the game's user data.
    ///
    /// Why a sidecar file and not the save game: the mod's notes must never be the reason a city
    /// fails to open. Writing custom data into a save couples the save to the mod, and a player
    /// who uninstalls or downgrades pays for it. A file the game never reads cannot do that. The
    /// cost is that copying a save elsewhere leaves the notes behind, which is recorded in
    /// docs/TECHNICAL_FINDINGS.md as an accepted trade.
    ///
    /// Writes go to a temporary file first and are then moved into place, so an interrupted save
    /// leaves the previous file intact rather than a half-written one.
    /// </summary>
    public class JsonCityStore : ICityStore
    {
        private const string LogPrefix = "[Persistence] ";

        public CityData Load(string cityKey)
        {
            var path = PathFor(cityKey);

            try
            {
                if (!File.Exists(path))
                {
                    CityData adopted;
                    if (TryAdoptSharedFile(cityKey, out adopted))
                    {
                        return adopted;
                    }

                    Mod.Log.Info(LogPrefix + "No stored data for '" + cityKey + "'; starting empty.");
                    return new CityData();
                }

                var json = File.ReadAllText(path);
                var data = JsonConvert.DeserializeObject<CityData>(json);

                if (data == null)
                {
                    Mod.Log.Warn(LogPrefix + "'" + cityKey + "' held no readable object; starting empty.");
                    return new CityData();
                }

                data = Migrate(data, cityKey);

                Mod.Log.Info(LogPrefix + "Loaded schema version " + data.SchemaVersion +
                             " for '" + cityKey + "' (" + (data.Ideas == null ? 0 : data.Ideas.Count) + " ideas).");
                return data;
            }
            catch (Exception e)
            {
                // A corrupt file costs the player their notes for this city, which is bad but
                // survivable. Throwing here would be worse: it would surface as a mod error while
                // they are trying to play.
                Mod.Log.Error(e, LogPrefix + "Could not read '" + path + "'; continuing with empty data.");
                return new CityData();
            }
        }

        public bool Save(string cityKey, CityData data)
        {
            if (data == null)
            {
                return false;
            }

            var path = PathFor(cityKey);
            var temp = path + ".tmp";

            try
            {
                Directory.CreateDirectory(ModPaths.CitiesRoot);

                data.SchemaVersion = CityData.CurrentSchemaVersion;
                File.WriteAllText(temp, JsonConvert.SerializeObject(data, Formatting.Indented));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                File.Move(temp, path);
                return true;
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not write '" + path + "'.");

                try
                {
                    if (File.Exists(temp))
                    {
                        File.Delete(temp);
                    }
                }
                catch
                {
                    // Nothing useful left to do; the stale temp file is harmless.
                }

                return false;
            }
        }

        /// <summary>
        /// Rescues the notes written while every city shared one file.
        ///
        /// The city name used to be read with <c>NameSystem.GetRenderedLabelName</c>, which
        /// returned nothing for the city entity. Every city therefore keyed to the name
        /// "unnamed", and every city wrote into the same file: one set of parked ideas and muted
        /// alerts, shared by all of them. Reading the name from
        /// <c>CityConfigurationSystem.cityName</c> fixed that, and in doing so left those notes
        /// behind a key nothing asks for any more.
        ///
        /// They are handed to the first city that opens without a file of its own. That city may
        /// not be the one they were written in - there is nothing in the file that says, because
        /// the ambiguity is exactly what the old key created - so this happens once and is
        /// logged plainly. Once is the point: the alternative is the same ideas appearing in
        /// every city forever.
        ///
        /// The shared file is renamed rather than deleted, and only after the new one is safely
        /// written. Losing something the player cannot get back is not a trade this mod makes,
        /// and a file they can still open in a text editor is the cheapest possible insurance.
        /// </summary>
        private bool TryAdoptSharedFile(string cityKey, out CityData adopted)
        {
            adopted = null;

            var sharedKey = CityKey.FromCityName("unnamed");
            if (cityKey == sharedKey)
            {
                // This city really has no name. The shared file is its own.
                return false;
            }

            var sharedPath = PathFor(sharedKey);

            try
            {
                if (!File.Exists(sharedPath))
                {
                    return false;
                }

                var data = JsonConvert.DeserializeObject<CityData>(File.ReadAllText(sharedPath));
                if (data == null)
                {
                    return false;
                }

                data = Migrate(data, sharedKey);

                // Any checkpoint named in here is carried over rather than dropped, even though
                // it may well have been taken in a different city. Dropping it would strand the
                // save it names - nothing would ever clean it up, and it would sit in the
                // player's list forever. CheckpointSystem checks whether it really belongs to
                // the city that opened, and tidies it up if it does not; that check can read the
                // save's own cityName, which this cannot.
                if (!Save(cityKey, data))
                {
                    // The new file could not be written, so nothing is retired: the shared file
                    // stays exactly where it is and the next city can try again.
                    return false;
                }

                File.Move(sharedPath, sharedPath + ".migrated");

                Mod.Log.Info(LogPrefix + "Notes from before cities had their own files (" +
                             data.Ideas.Count + " ideas, " + data.MutedAlerts.Count +
                             " muted alerts) have been given to '" + cityKey +
                             "'. The old shared file is kept as '" + Path.GetFileName(sharedPath) +
                             ".migrated' in case they belonged somewhere else.");

                adopted = data;
                return true;
            }
            catch (Exception e)
            {
                Mod.Log.Error(e, LogPrefix + "Could not carry over the shared notes; leaving them where they are.");
                return false;
            }
        }

        /// <summary>
        /// Brings older files forward. Version 1 is the first, so there is nothing to do yet -
        /// the shape exists so that the next version has somewhere to go.
        /// </summary>
        private static CityData Migrate(CityData data, string cityKey)
        {
            if (data.Ideas == null)
            {
                data.Ideas = new System.Collections.Generic.List<StoredIdea>();
            }

            if (data.MutedAlerts == null)
            {
                data.MutedAlerts = new System.Collections.Generic.List<string>();
            }

            if (data.CheckpointName == null)
            {
                data.CheckpointName = string.Empty;
            }

            if (data.SchemaVersion > CityData.CurrentSchemaVersion)
            {
                Mod.Log.Warn(LogPrefix + "'" + cityKey + "' was written by a newer version of the mod (schema " +
                             data.SchemaVersion + " > " + CityData.CurrentSchemaVersion +
                             "). Reading it as best we can; it will be rewritten in the older shape.");
            }

            return data;
        }

        private static string PathFor(string cityKey)
        {
            return Path.Combine(ModPaths.CitiesRoot, cityKey + ".json");
        }
    }
}

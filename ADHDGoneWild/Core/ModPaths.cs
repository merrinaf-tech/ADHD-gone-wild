using System.IO;
using Colossal.PSI.Environment;

namespace ADHDGoneWild.Core
{
    /// <summary>
    /// Where the mod keeps its own files.
    ///
    /// Everything lives beside the game's user data, never inside a save. A save written by a
    /// player who later removes this mod must still open, so the mod adds nothing to it.
    /// </summary>
    public static class ModPaths
    {
        /// <summary>%LOCALAPPDATA%Low/Colossal Order/Cities Skylines II/ModsData/ADHDGoneWild</summary>
        public static string DataRoot
        {
            get { return Path.Combine(Path.Combine(EnvPath.kUserDataPath, "ModsData"), Mod.Id); }
        }

        /// <summary>One file per city, under DataRoot/cities.</summary>
        public static string CitiesRoot
        {
            get { return Path.Combine(DataRoot, "cities"); }
        }
    }
}

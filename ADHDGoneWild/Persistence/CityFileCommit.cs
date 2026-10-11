using System.IO;

namespace ADHDGoneWild.Persistence
{
    /// <summary>
    /// Promotes a fully written temporary file without leaving a gap where the city file is gone.
    /// Both paths are in the same directory so the replacement stays on one volume.
    /// </summary>
    public static class CityFileCommit
    {
        public static void Commit(string temporaryPath, string destinationPath)
        {
            if (File.Exists(destinationPath))
            {
                // File.Replace also keeps the old file as a recovery copy. A later successful
                // save refreshes that copy with the version it just replaced.
                File.Replace(temporaryPath, destinationPath, destinationPath + ".bak");
            }
            else
            {
                File.Move(temporaryPath, destinationPath);
            }
        }
    }
}

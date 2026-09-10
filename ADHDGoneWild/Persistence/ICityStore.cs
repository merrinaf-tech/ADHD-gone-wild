namespace ADHDGoneWild.Persistence
{
    /// <summary>
    /// Reading and writing the mod's own per-city file.
    ///
    /// Implementations must never throw: a failure to load is an empty city, and a failure to
    /// save is a logged warning. Nothing here is allowed to stand between the player and their
    /// save game.
    /// </summary>
    public interface ICityStore
    {
        /// <summary>Never null. An unreadable or absent file yields fresh, empty data.</summary>
        CityData Load(string cityKey);

        /// <summary>Returns false if the write failed, having already logged why.</summary>
        bool Save(string cityKey, CityData data);
    }
}

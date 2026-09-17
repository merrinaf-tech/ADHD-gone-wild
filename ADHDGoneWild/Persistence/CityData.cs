using System.Collections.Generic;

namespace ADHDGoneWild.Persistence
{
    /// <summary>
    /// Everything the mod remembers about one city.
    ///
    /// This is the on-disk shape, deliberately dull: plain fields, no game types, no entities.
    /// It is versioned from the first release so a later schema can migrate rather than discard.
    /// Fields that no feature fills in yet are still declared, because adding a field to an
    /// existing schema is cheaper than changing what an old one meant.
    /// </summary>
    public class CityData
    {
        /// <summary>Bump when the meaning of an existing field changes, not when adding one.</summary>
        public const int CurrentSchemaVersion = 1;

        public int SchemaVersion = CurrentSchemaVersion;

        /// <summary>The city name at the time of writing. Diagnostic only - the key is the filename.</summary>
        public string CityName = string.Empty;

        /// <summary>When this city was last open, for Welcome Back. 0 means never recorded.</summary>
        public long LastSeenUnixUtc;

        /// <summary>Where the camera was resting when the player left. Welcome Back uses it.</summary>
        public float LastCameraX;
        public float LastCameraY;
        public float LastCameraZ;
        public bool HasLastCamera;

        public List<StoredIdea> Ideas = new List<StoredIdea>();

        /// <summary>
        /// The save name behind the safety net for this city, or empty. The save itself is a real
        /// one made through the game's own save path - this only remembers which one is ours, so
        /// the player never has to identify it in a list.
        /// </summary>
        public string CheckpointName = string.Empty;

        /// <summary>When that checkpoint was taken. Zero when there is none.</summary>
        public long CheckpointUnixUtc;

        /// <summary>
        /// Alert ids the player asked to stop hearing about. Muting is a decision, so it is
        /// remembered; it is not a promise to deal with the thing later.
        /// </summary>
        public List<string> MutedAlerts = new List<string>();
    }

    /// <summary>One parked thought. Not a task: it has no due date, no state and no progress.</summary>
    public class StoredIdea
    {
        public string Id = string.Empty;

        /// <summary>Matches ADHDGoneWild.BrainParking.IdeaCategory.</summary>
        public int Category;

        /// <summary>Optional. Most ideas never get one, and that is the point.</summary>
        public string Note = string.Empty;

        /// <summary>
        /// The long version, also optional. Absent from files written before it existed, which
        /// reads back as empty - exactly right, and why this needed no schema bump.
        ///
        /// One item per line, each carrying the colour of its bullet - see the encoding in
        /// UI/src/mods/brain-parking/description.ts. Stored as one string because that is all it
        /// has to be: nothing on the C# side ever reads inside it.
        /// </summary>
        public string Description = string.Empty;

        public float X;
        public float Y;
        public float Z;

        public long CreatedUnixUtc;
    }
}

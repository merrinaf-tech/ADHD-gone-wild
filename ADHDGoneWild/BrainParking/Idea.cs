using System;
using ADHDGoneWild.Core;

namespace ADHDGoneWild.BrainParking
{
    /// <summary>
    /// A thought the player put down somewhere so they could stop holding it.
    ///
    /// There is no "done", no due date and no progress. An idea is a memory, and the only things
    /// that ever happen to one are being looked at and being forgotten.
    /// </summary>
    public class Idea
    {
        public string Id { get; private set; }

        public IdeaCategory Category { get; set; }

        /// <summary>Optional, and usually empty. Typing is never required to park a thought.</summary>
        public string Note { get; set; }

        public WorldPoint Position { get; set; }

        public DateTime CreatedUtc { get; private set; }

        public Idea(string id, IdeaCategory category, string note, WorldPoint position, DateTime createdUtc)
        {
            Id = id;
            Category = category;
            Note = note ?? string.Empty;
            Position = position;
            CreatedUtc = createdUtc;
        }

        public static Idea Create(WorldPoint position, IdeaCategory category)
        {
            return new Idea(Guid.NewGuid().ToString("N"), category, string.Empty, position, DateTime.UtcNow);
        }
    }
}

namespace ADHDGoneWild.Trail
{
    /// <summary>
    /// Somewhere the player spent a while. Not a project, not a task, and carrying no opinion
    /// about whether anything there is finished.
    ///
    /// The centre is fixed when the place is created and never moves. That is what stops one
    /// entry from growing to cover half the city as the player pans: a sample either falls inside
    /// <see cref="AttentionTrail.MergeRadius"/> of that original point or it starts somewhere new.
    /// The framing, by contrast, is always the most recent one - going back should return the view
    /// the player last had, not the one they arrived with.
    /// </summary>
    public sealed class TrailPlace
    {
        public TrailPlace(string id, in TrailSample sample)
        {
            Id = id;
            X = sample.X;
            Y = sample.Y;
            Z = sample.Z;
            FirstSeenUnixUtc = sample.UnixUtc;
            Remember(sample, 0);
        }

        public string Id { get; }

        /// <summary>The fixed centre. Comparisons are XZ only; height never decides a place.</summary>
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public long FirstSeenUnixUtc { get; }
        public long LastSeenUnixUtc { get; private set; }

        /// <summary>
        /// Seconds of attention, capped. It exists to decide whether this is worth remembering at
        /// all - it is never shown, because a number counting up beside a place is a score, and
        /// the one thing this mod will not do is keep score of how the player spends their time.
        /// </summary>
        public int AttentionSeconds { get; private set; }

        /// <summary>Every <see cref="TrailActivity"/> seen here, as bits.</summary>
        public int ActivityMask { get; private set; }

        /// <summary>The camera as it was the last time this place had the player's attention.</summary>
        public float PivotX { get; private set; }
        public float PivotY { get; private set; }
        public float PivotZ { get; private set; }
        public float Zoom { get; private set; }
        public float RotationX { get; private set; }
        public float RotationY { get; private set; }
        public float RotationZ { get; private set; }

        /// <summary>Shown to the player only once it has earned it.</summary>
        public bool Promoted
        {
            get { return AttentionSeconds >= AttentionTrail.PromoteAfterSeconds; }
        }

        public void Remember(in TrailSample sample, int seconds)
        {
            LastSeenUnixUtc = sample.UnixUtc;

            if (AttentionSeconds < AttentionTrail.AttentionCapSeconds)
            {
                AttentionSeconds = System.Math.Min(
                    AttentionTrail.AttentionCapSeconds,
                    AttentionSeconds + seconds);
            }

            ActivityMask |= TrailActivities.Bit(sample.Activity);

            PivotX = sample.X;
            PivotY = sample.Y;
            PivotZ = sample.Z;
            Zoom = sample.Zoom;
            RotationX = sample.RotationX;
            RotationY = sample.RotationY;
            RotationZ = sample.RotationZ;
        }

        /// <summary>Squared XZ distance from this place's centre. Squared, so nothing takes a root.</summary>
        public float DistanceSquaredTo(float x, float z)
        {
            var dx = x - X;
            var dz = z - Z;
            return (dx * dx) + (dz * dz);
        }
    }
}

namespace ADHDGoneWild.Trail
{
    /// <summary>
    /// One second of the player's attention, as the only four things about it the mod can observe:
    /// when, where the camera was pointed, how it was framed, and which tool was in hand.
    ///
    /// No entity, no selection, no prefab, no intention. The clustering rule is fed nothing else,
    /// which is what makes it arguable in a test rather than only in a city.
    /// </summary>
    public struct TrailSample
    {
        /// <summary>Wall clock, in seconds. Real time, so pausing the game does not stop it.</summary>
        public readonly long UnixUtc;

        /// <summary>Where the camera is pointed - not where it is.</summary>
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        /// <summary>The framing, kept so the player can be put back exactly as they left it.</summary>
        public readonly float Zoom;
        public readonly float RotationX;
        public readonly float RotationY;
        public readonly float RotationZ;

        public readonly TrailActivity Activity;

        public TrailSample(
            long unixUtc,
            float x,
            float y,
            float z,
            float zoom,
            float rotationX,
            float rotationY,
            float rotationZ,
            TrailActivity activity)
        {
            UnixUtc = unixUtc;
            X = x;
            Y = y;
            Z = z;
            Zoom = zoom;
            RotationX = rotationX;
            RotationY = rotationY;
            RotationZ = rotationZ;
            Activity = activity;
        }
    }
}

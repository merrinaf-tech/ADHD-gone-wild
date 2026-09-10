namespace ADHDGoneWild.Core
{
    /// <summary>
    /// A place in the city, with no Unity in it.
    ///
    /// The game's own float3 would do the job, but dragging it through the alert model would make
    /// every rule that touches a position untestable outside the game. Conversion happens once,
    /// at the edge, in the systems that actually talk to ECS.
    /// </summary>
    public struct WorldPoint
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        public WorldPoint(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return X.ToString("F0") + ", " + Z.ToString("F0");
        }
    }
}

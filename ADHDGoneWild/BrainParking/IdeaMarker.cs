namespace ADHDGoneWild.BrainParking
{
    /// <summary>
    /// How big a parked idea's ring is, in metres.
    ///
    /// One number, in a file with no game in it, because two things need it and they must never
    /// disagree: <see cref="IdeaMarkerSystem"/> draws the ring, and <see cref="IdeaHitTest"/>
    /// decides whether a click landed on it. If those drift apart the ring stops matching its own
    /// clickable area, and the player gets a target that is not where it looks - the kind of fault
    /// that reads as "sometimes it just doesn't work".
    /// </summary>
    public static class IdeaMarker
    {
        public const float Diameter = 24f;

        public const float Radius = Diameter * 0.5f;
    }
}

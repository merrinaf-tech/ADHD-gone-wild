namespace ADHDGoneWild.Memory
{
    /// <summary>
    /// The only judgement call Welcome Back makes: has it been long enough to say something.
    ///
    /// Kept apart from the system that reads the clock and writes the binding, so the one rule
    /// that matters here - "not on every load, only after a real break" - can be argued about
    /// with two integers instead of by loading a save.
    /// </summary>
    public static class WelcomeBackRule
    {
        /// <summary>
        /// True once <paramref name="awayMinutes"/> have passed since <paramref name="lastSeenUnixUtc"/>.
        ///
        /// A city with no recorded visit (0) never qualifies - there is nothing to welcome the
        /// player back from on the very first time the mod sees it.
        /// </summary>
        public static bool ShouldShow(long lastSeenUnixUtc, long nowUnixUtc, int awayMinutes)
        {
            if (lastSeenUnixUtc <= 0)
            {
                return false;
            }

            var awaySeconds = nowUnixUtc - lastSeenUnixUtc;
            return awaySeconds >= (long)awayMinutes * 60L;
        }
    }
}

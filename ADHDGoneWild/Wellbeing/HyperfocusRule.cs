namespace ADHDGoneWild.Wellbeing
{
    /// <summary>
    /// When the mod should mention the time, and when it should keep quiet.
    ///
    /// The whole feature rests on one idea, so the idea is kept here on its own, in arithmetic,
    /// where it can be argued with: hyperfocus does not need to be interrupted, it needs the
    /// thing it removed handed back. What it removes is the time. So the mod says the time.
    ///
    /// What this deliberately cannot express, because it must never do it:
    ///
    /// - There is no count of how many times the player has been told. Nothing accumulates,
    ///   nothing escalates, and the wording cannot get firmer the longer they play.
    /// - There is no notion of a session being too long. The interval is a spacing between
    ///   mentions, not a limit, and nothing here knows what "too long" would even mean.
    /// - Dismissing costs nothing and is not remembered beyond the next due time.
    ///
    /// Time is counted in minutes actually spent in a city this run, not wall-clock minutes since
    /// the game launched: sitting in the main menu is not what anyone needs protecting from.
    /// </summary>
    public static class HyperfocusRule
    {
        /// <summary>
        /// How long "Later" buys. Short on purpose - it is a "not this second", not a snooze that
        /// quietly turns the feature off for the rest of the day.
        /// </summary>
        public const int SnoozeMinutes = 15;

        /// <summary>True when the card should be on screen.</summary>
        public static bool ShouldShow(bool enabled, double playedMinutes, double nextDueMinutes)
        {
            return enabled && playedMinutes >= nextDueMinutes;
        }

        /// <summary>
        /// The next time to say something, after the player has acknowledged this one.
        ///
        /// Measured from now rather than from when it was due, so a card left on screen for
        /// twenty minutes does not immediately owe another one.
        /// </summary>
        public static double AfterDismissed(double playedMinutes, int intervalMinutes)
        {
            return playedMinutes + Clamp(intervalMinutes);
        }

        /// <summary>
        /// "Not now." The same mention again shortly, not a cancellation.
        ///
        /// Never longer than the ordinary spacing. On a short interval a fixed fifteen minutes
        /// would make "Later" push the card *further* away than leaving it alone would, which is
        /// the opposite of what the word means.
        /// </summary>
        public static double AfterSnoozed(double playedMinutes, int intervalMinutes)
        {
            var interval = Clamp(intervalMinutes);
            return playedMinutes + (interval < SnoozeMinutes ? interval : SnoozeMinutes);
        }

        /// <summary>
        /// The first due time of a run, and the one to use whenever the interval changes.
        ///
        /// Changing the setting restarts the spacing from now in both directions. Lengthening it
        /// must not leave a card already owed and about to appear; shortening it must not have to
        /// wait out the old, longer interval before taking effect.
        /// </summary>
        public static double FromNow(double playedMinutes, int intervalMinutes)
        {
            return playedMinutes + Clamp(intervalMinutes);
        }

        /// <summary>
        /// A floor under the interval, and a deliberately low one: it catches zero and nonsense,
        /// not a short gap the player asked for.
        ///
        /// This used to floor at fifteen minutes, on the reasoning that nothing should be able to
        /// turn the card into a nag. That was the mod overruling a preference someone had stated
        /// out loud in the options, which is not its job - there is no single correct ADHD
        /// experience, and a player who wants the time every few minutes is allowed to have it.
        /// It also silently defeated the short setting that exists so the card can be seen
        /// without waiting two hours for it.
        /// </summary>
        private static int Clamp(int intervalMinutes)
        {
            return intervalMinutes < 1 ? 1 : intervalMinutes;
        }
    }
}

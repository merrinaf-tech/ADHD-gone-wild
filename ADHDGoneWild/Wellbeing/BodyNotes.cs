namespace ADHDGoneWild.Wellbeing
{
    /// <summary>
    /// Which note to put under the clock, and when.
    ///
    /// It rotates rather than repeats, so no single note becomes the thing the player learns to
    /// stop reading, and it never says the same thing twice in a row. The order is fixed rather
    /// than random: a mod that says something different every time you look is harder to ignore,
    /// and being easy to ignore is a requirement here, not a shortcoming.
    ///
    /// Pure, and kept apart from the system, so the one judgement it makes - what counts as late,
    /// and what to say then - can be argued about without launching the game.
    /// </summary>
    public static class BodyNotes
    {
        /// <summary>The rotation, in order. <see cref="BodyNote.LateHour"/> joins it only at night.</summary>
        private static readonly BodyNote[] Daytime =
        {
            BodyNote.Water,
            BodyNote.Stand,
            BodyNote.Eyes,
            BodyNote.Food,
            BodyNote.Shoulders
        };

        private static readonly BodyNote[] Nighttime =
        {
            BodyNote.LateHour,
            BodyNote.Water,
            BodyNote.Shoulders,
            BodyNote.Eyes,
            BodyNote.Stand
        };

        /// <summary>From this hour, the night rotation is used.</summary>
        public const int NightStartsAt = 23;

        /// <summary>And up to this one.</summary>
        public const int NightEndsAt = 5;

        /// <summary>
        /// True in the small hours, when the most useful thing to say is about the hour rather
        /// than about posture.
        /// </summary>
        public static bool IsLate(int localHour)
        {
            return localHour >= NightStartsAt || localHour < NightEndsAt;
        }

        /// <summary>
        /// The note for the <paramref name="shownCount"/>-th time the card has appeared this run,
        /// counting from zero.
        /// </summary>
        public static BodyNote Pick(int shownCount, int localHour)
        {
            var rotation = IsLate(localHour) ? Nighttime : Daytime;

            // A negative count would be a bug elsewhere, not a reason to throw at the player.
            var index = shownCount < 0 ? 0 : shownCount % rotation.Length;
            return rotation[index];
        }
    }
}

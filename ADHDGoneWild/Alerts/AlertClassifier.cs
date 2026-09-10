namespace ADHDGoneWild.Alerts
{
    /// <summary>
    /// A city-wide supply reading: how much there is, how much is wanted, how much arrived.
    ///
    /// Deliberately three plain numbers. Every network the game runs - electricity, fresh water,
    /// sewage - reports this same shape, so one rule covers all of them and can be argued about
    /// without launching the game.
    /// </summary>
    public struct UtilityReading
    {
        public readonly int Capacity;
        public readonly int Demand;
        public readonly int Fulfilled;

        public UtilityReading(int capacity, int demand, int fulfilled)
        {
            Capacity = capacity;
            Demand = demand;
            Fulfilled = fulfilled;
        }
    }

    /// <summary>
    /// Turns readings into a status, and nothing else.
    ///
    /// This is the one place the mod decides that something is worth the player's attention, so
    /// it is kept free of the game, of the UI and of any notion of what to do about it. Every
    /// rule below can be exercised from a test with three integers.
    ///
    /// The bias throughout is towards saying less. A false "everything is fine" costs the player
    /// nothing they were not already living with; a false alarm costs them attention, which is
    /// the exact thing this mod exists to protect.
    /// </summary>
    public static class AlertClassifier
    {
        /// <summary>
        /// Below this share of unmet demand, a shortfall is treated as rounding rather than as
        /// something happening to somebody. The game's numbers wobble by a unit or two between
        /// ticks and a flickering red row is worse than no row.
        /// </summary>
        public const float MaterialShortfall = 0.02f;

        /// <summary>Demand this close to capacity is worth knowing about before it bites.</summary>
        public const float TightHeadroom = 0.90f;

        /// <summary>
        /// The status of one utility network.
        ///
        /// Returns <see cref="Core.Status.Resolved"/> when there is nothing to say. Callers drop
        /// those rather than showing a green row for every network the city runs - a wall of
        /// reassurance is still a wall.
        /// </summary>
        public static Core.Status ClassifyUtility(UtilityReading reading)
        {
            // A network the city does not have yet. Not a problem, not a success, not a row.
            if (reading.Capacity <= 0 && reading.Demand <= 0)
            {
                return Core.Status.Resolved;
            }

            var unmet = reading.Demand - reading.Fulfilled;

            if (unmet > 0)
            {
                // Somebody is already going without. How many is what separates the two.
                var share = reading.Demand > 0 ? (float)unmet / reading.Demand : 1f;
                return share >= MaterialShortfall ? Core.Status.Immediate : Core.Status.Important;
            }

            // Everything asked for arrived. The only question left is how much room is left.
            if (reading.Capacity <= 0)
            {
                return Core.Status.Resolved;
            }

            var used = (float)reading.Demand / reading.Capacity;

            if (used >= 1f)
            {
                return Core.Status.Important;
            }

            return used >= TightHeadroom ? Core.Status.Monitor : Core.Status.Resolved;
        }

        /// <summary>
        /// The status of an in-world notification, from the priority the game itself gave it.
        ///
        /// Reading the game's own ranking rather than keeping a table of notification names means
        /// a type the mod has never heard of is still ranked correctly, and a patch that adds one
        /// does not silently mis-sort it.
        ///
        /// Priorities are Game.Notifications.IconPriority: Min 0, Info 10, Problem 50,
        /// Warning 100, MajorProblem 150, Error 200, FatalProblem 250, Max 255.
        /// </summary>
        public static Core.Status ClassifyNotification(int priority)
        {
            if (priority >= 200)
            {
                return Core.Status.Immediate;
            }

            if (priority >= 150)
            {
                return Core.Status.Important;
            }

            if (priority >= 50)
            {
                return Core.Status.Monitor;
            }

            return Core.Status.Muted;
        }
    }
}

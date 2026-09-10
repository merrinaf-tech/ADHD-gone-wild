namespace ADHDGoneWild.Core
{
    /// <summary>
    /// What a piece of information means, never which city system it came from.
    ///
    /// Colour represents status; the icon represents the subject. Electricity is not "yellow" -
    /// an electricity problem worth watching is. Keeping the two apart is the reason this enum
    /// exists at all, and it is the only vocabulary the UI is given for colouring anything.
    ///
    /// The matching colours and shapes live in UI/src/theme/tokens.ts. Nothing on this side of
    /// the bridge knows a hex value.
    /// </summary>
    public enum Status
    {
        /// <summary>Happening now and likely to matter. 🔴</summary>
        Immediate = 0,

        /// <summary>Real, but not on fire. 🟠</summary>
        Important = 1,

        /// <summary>Worth a glance later. 🟡</summary>
        Monitor = 2,

        /// <summary>Present, deliberately quiet. ⚪</summary>
        Muted = 3,

        /// <summary>Something the player made: an idea, a marker, a note. 🟣</summary>
        Personal = 4,

        /// <summary>Settled. Used only where saying so helps. 🟢</summary>
        Resolved = 5
    }
}

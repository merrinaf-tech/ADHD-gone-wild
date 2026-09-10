namespace ADHDGoneWild.Alerts
{
    /// <summary>
    /// Guesses what a notification is about from the game's name for it.
    ///
    /// This is a heuristic and is treated as one. It matches on words rather than on exact prefab
    /// names, because exact names are a moving target across patches and a table of them would
    /// rot quietly. When nothing matches, the answer is <see cref="AlertSubject.Other"/>, which
    /// still shows the notification, still counts it, and still carries the game's own name for
    /// it. A subject the mod fails to recognise loses its icon, never its existence.
    ///
    /// Getting one wrong costs an icon. That is why it is allowed to guess at all - and why it
    /// only ever decides the icon, never whether the player is told.
    /// </summary>
    public static class NotificationSubjects
    {
        public static AlertSubject For(string prefabName)
        {
            if (string.IsNullOrEmpty(prefabName))
            {
                return AlertSubject.Other;
            }

            var name = prefabName.ToLowerInvariant();

            // Order matters where words overlap: "sewage water" is sewage, and "water pollution"
            // is about water, so the more specific word is tested first.
            if (Has(name, "sewage") || Has(name, "wastewater"))
            {
                return AlertSubject.Sewage;
            }

            if (Has(name, "electric") || Has(name, "power"))
            {
                return AlertSubject.Electricity;
            }

            if (Has(name, "water"))
            {
                return AlertSubject.Water;
            }

            if (Has(name, "garbage") || Has(name, "trash") || Has(name, "landfill"))
            {
                return AlertSubject.Garbage;
            }

            if (Has(name, "death") || Has(name, "dead") || Has(name, "cemetery") || Has(name, "grave"))
            {
                return AlertSubject.Deathcare;
            }

            if (Has(name, "health") || Has(name, "sick") || Has(name, "hospital") || Has(name, "disease"))
            {
                return AlertSubject.Health;
            }

            if (Has(name, "fire") || Has(name, "burn"))
            {
                return AlertSubject.Fire;
            }

            if (Has(name, "crime") || Has(name, "police") || Has(name, "robb"))
            {
                return AlertSubject.Police;
            }

            if (Has(name, "school") || Has(name, "education") || Has(name, "student"))
            {
                return AlertSubject.Education;
            }

            if (Has(name, "mail") || Has(name, "post"))
            {
                return AlertSubject.Mail;
            }

            if (Has(name, "traffic") || Has(name, "road") || Has(name, "parking") || Has(name, "path"))
            {
                return AlertSubject.Traffic;
            }

            if (Has(name, "transport") || Has(name, "bus") || Has(name, "train") || Has(name, "tram") ||
                Has(name, "metro") || Has(name, "passenger") || Has(name, "stop"))
            {
                return AlertSubject.Transport;
            }

            if (Has(name, "homeless") || Has(name, "rent") || Has(name, "abandon") ||
                Has(name, "tenant") || Has(name, "household"))
            {
                return AlertSubject.Housing;
            }

            if (Has(name, "worker") || Has(name, "employee") || Has(name, "profit") ||
                Has(name, "customer") || Has(name, "company") || Has(name, "resource"))
            {
                return AlertSubject.Workplace;
            }

            return AlertSubject.Other;
        }

        private static bool Has(string haystack, string needle)
        {
            return haystack.IndexOf(needle, System.StringComparison.Ordinal) >= 0;
        }
    }
}

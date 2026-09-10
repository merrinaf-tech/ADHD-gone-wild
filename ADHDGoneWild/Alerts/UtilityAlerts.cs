using System.Collections.Generic;

namespace ADHDGoneWild.Alerts
{
    /// <summary>
    /// The city's supply networks, turned into at most one alert each.
    ///
    /// These are statements about the city as a whole, so they carry no building count and no
    /// places to visit - there is no single spot to fly to when the whole grid is short. That is
    /// the difference between this collector and the notification one, and the reason the alert
    /// model has room for both.
    ///
    /// Pure: it takes three readings and returns alerts. The system that owns the ECS handles
    /// getting the numbers.
    /// </summary>
    public static class UtilityAlerts
    {
        public const string ElectricityId = "utility.electricity";
        public const string WaterId = "utility.water";
        public const string SewageId = "utility.sewage";

        public static void Collect(
            List<Alert> into,
            UtilityReading electricity,
            UtilityReading freshWater,
            UtilityReading sewage)
        {
            Add(into, ElectricityId, AlertSubject.Electricity, electricity);
            Add(into, WaterId, AlertSubject.Water, freshWater);
            Add(into, SewageId, AlertSubject.Sewage, sewage);
        }

        private static void Add(List<Alert> into, string id, AlertSubject subject, UtilityReading reading)
        {
            var status = AlertClassifier.ClassifyUtility(reading);

            // Resolved means "nothing worth saying". The aggregator drops these anyway; not
            // building them keeps the common case - a city where everything is fine - free.
            if (status == Core.Status.Resolved)
            {
                return;
            }

            into.Add(new Alert(id, subject, status, AlertTitles.KeyFor(subject, status)));
        }
    }
}

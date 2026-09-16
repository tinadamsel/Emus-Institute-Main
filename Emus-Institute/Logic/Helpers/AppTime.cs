using System;

namespace Logic.Helpers
{
    /// <summary>
    /// Provides "now" in a fixed timezone (Nigeria / West Africa Time, UTC+1),
    /// regardless of what timezone the host machine/server is configured with.
    /// Use this instead of DateTime.Now anywhere the result affects business
    /// logic (greetings, live session scheduling, etc.), so behavior stays
    /// consistent between local development and production deployment.
    /// </summary>
    public static class AppTime
    {
        private static readonly TimeZoneInfo TimeZone = ResolveTimeZone();

        public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);

        private static TimeZoneInfo ResolveTimeZone()
        {
            // "W. Central Africa Standard Time" is the Windows id for UTC+1 (Lagos).
            // "Africa/Lagos" is the IANA id used on Linux (most cloud hosts/containers).
            string[] candidateIds = { "W. Central Africa Standard Time", "Africa/Lagos" };

            foreach (var id in candidateIds)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(id);
                }
                catch (TimeZoneNotFoundException)
                {
                }
                catch (InvalidTimeZoneException)
                {
                }
            }

            // Fallback: Nigeria does not observe daylight saving, fixed UTC+1.
            return TimeZoneInfo.CreateCustomTimeZone("WAT", TimeSpan.FromHours(1), "West Africa Time", "WAT");
        }
    }
}

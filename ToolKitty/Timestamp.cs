using System;

namespace ToolKitty
{
    public static class Timestamp
    {
        private static readonly DateTime
            DateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetTimestamp(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Local) {
                dateTime = dateTime.ToUniversalTime();
            }

            var timeSpan = dateTime - DateTime;

            return (long)timeSpan.TotalMilliseconds;
        }

        public static long GetTimestamp()
        {
            return GetTimestamp(DateTime.UtcNow);
        }

        public static DateTime GetDateTime(long timestamp)
        {
            return DateTime.AddMilliseconds(timestamp);
        }
    }
}

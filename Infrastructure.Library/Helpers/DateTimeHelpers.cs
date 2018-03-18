using System;

namespace Infrastructure.Library.Helpers
{
    public class DateTimeHelpers : IDateTimeHelpers
    {
        public string ConvertDateTimeToUniversalTimeISO8601String(DateTime dateTime)
        {
            string convertedDateTime = dateTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            return convertedDateTime;
        }
    }
}

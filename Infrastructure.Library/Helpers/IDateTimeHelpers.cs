using System;

namespace Infrastructure.Library.Helpers
{
    public interface IDateTimeHelpers
    {
        string ConvertDateTimeToUniversalTimeISO8601String(DateTime dateTime);
    }
}

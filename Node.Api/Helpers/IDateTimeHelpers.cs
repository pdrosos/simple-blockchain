using System;

namespace Node.Api.Helpers
{
    public interface IDateTimeHelpers
    {
        string ConvertDateTimeToUniversalTimeISO8601String(DateTime dateTime);
    }
}

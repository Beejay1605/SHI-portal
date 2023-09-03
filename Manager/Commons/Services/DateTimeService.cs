using Manager.Commons.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Services;

public class DateTimeService : IDateTimeService
{
    private static readonly TimeZoneInfo timeZone = InitTimezone();

    static TimeZoneInfo InitTimezone()
    {
        return TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
    }

    public DateTime Now => TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Humans.Web.Helpers
{
    public static class TimeUtility
    {
        public static void LongAgo(out string days, out string hours, out string mins, DateTime targetDateTime)
        {
            TimeSpan timeDiff = DateTime.Now - targetDateTime;

            days = Math.Round(timeDiff.TotalDays, 0) > 0 ? Math.Round(timeDiff.TotalDays, 0).ToString() + " Days" : string.Empty;
            hours = Math.Round(timeDiff.TotalHours, 0) > 0 ? (Math.Round(timeDiff.TotalHours, 0)%24).ToString() + " Hrs" : string.Empty;
            mins = Math.Round(timeDiff.TotalMinutes, 0) > 0 ? (Math.Round(timeDiff.TotalMinutes, 0)%60).ToString() + " Mins" : "1 Min";
        }
    }
}
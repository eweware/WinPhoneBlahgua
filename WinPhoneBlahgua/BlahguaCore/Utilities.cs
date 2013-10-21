using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinPhoneBlahgua
{
    public class Utilities
    {
        public static string ElapsedDateString(DateTime theDate)
        {
            string tailStr;
            long nowTicks = DateTime.Now.Ticks;
            long dateTicks = theDate.Ticks;
            long timeSpan;

            if (dateTicks > nowTicks)
            {
                timeSpan = (long)Math.Floor((dateTicks - nowTicks) / TimeSpan.TicksPerSecond);
                tailStr = " from now";
            }
            else
            {
                timeSpan = (long)Math.Floor((nowTicks - dateTicks) / TimeSpan.TicksPerSecond);
                tailStr = " ago";
            }

            var curYears = Math.Floor(timeSpan / 31536000);
            if (curYears > 0)
            {
                if (curYears > 2)
                {
                    return curYears + " years" + tailStr;
                }
                else
                {
                    return Math.Floor(timeSpan / 2592000) + " months" + tailStr;
                }
            }

            var curMonths = Math.Floor(timeSpan / 2592000); // average 30 days
            if (curMonths > 0)
            {
                if (curMonths >= 2)
                {
                    return curMonths + " months" + tailStr;
                }
                else
                {
                    return Math.Floor(timeSpan / 604800) + " weeks" + tailStr;
                }
            }

            var curDays = Math.Floor(timeSpan / 86400);
            if (curDays > 0)
            {
                if (curDays >= 2)
                {
                    return curDays + " days" + tailStr;
                }
                else
                {
                    return Math.Floor(timeSpan / 3600) + " hours" + tailStr;
                }
            }

            var curHours = Math.Floor(timeSpan / 3600);
            if (curHours > 0)
            {
                if (curHours >= 2)
                {
                    return curHours + " hours" + tailStr;
                }
                else
                {
                    return Math.Floor(timeSpan / 60) + " minutes" + tailStr;
                }
            }

            var curMinutes = Math.Floor(timeSpan / 60);
            if (curMinutes >= 2)
            {
                return curMinutes + " minutes" + tailStr;
            }

            if (timeSpan <= 1)
            {
                return "just now";
            }
            else
            {
                return timeSpan + " seconds" + tailStr;
            }

        }

    }

}

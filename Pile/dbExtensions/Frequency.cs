using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pile.db
{
    public partial class Frequency
    {
        public bool IsValidFrequency(DateTime routeDate, DateTime? lastRouteDate)
        {
            if (!lastRouteDate.HasValue)
                return true;  //Never worked, so it must be ok

            if (this.Id == 4)
                return false; // been worked before, 4 is a one-time scoop, no good!

            var daysBetweenRoutes = (routeDate - lastRouteDate.Value).TotalDays;

            if (daysBetweenRoutes >= 28)
                return true;  // types 1,2,3 all ok if it's been 28 days!

            return (this.Id == 1 || this.Id == 2) && daysBetweenRoutes >= this.Id * 7;   // 1 - 7 days ok, 2 - 14 days ok.
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Pile.db
{
    public partial class Route
    {
        public async static Task DeletePauseAndFinalRoutes(Customer cust)
        {
            if (cust.FinalServiceDate.HasValue)
            {
                await DeleteRoutes(cust, cust.FinalServiceDate.Value);
                return; 
            }

            var futurePauses = cust.Pauses.Where(x => x.PauseDate > DateTime.Now || x.RestartDate > DateTime.Now);
            foreach (var pause in futurePauses)
                await DeleteRoutes(cust, pause.PauseDate, pause.RestartDate);

        }
        public static void CheckInsertRoute(Customer cust)
        {
            if (cust.Status != "A")
                return;
            var routeDatesToCheck = new List<DateTime>();
            var weekStartsToCheck = new List<DateTime>() { ThisWeekStart(), NextWeekStart() };
            if (cust.RouteStartDate.HasValue)
                routeDatesToCheck.Add(cust.RouteStartDate.Value);

            var futurePauses = cust.Pauses.Where(x => x.PauseDate > DateTime.Now || x.RestartDate > DateTime.Now);
            foreach (var pause in futurePauses)
                routeDatesToCheck.Add(pause.RestartDate);

            foreach (var routeDate in routeDatesToCheck)
                foreach (var weekStartToCheck in weekStartsToCheck)
                    if (routeDate >= weekStartToCheck && routeDate <= weekStartToCheck.AddDays(6))
                        InsertRoute(cust, weekStartToCheck);
        }

        private async static Task DeleteRoutes(Customer cust, DateTime afterDate, DateTime? throughDate = null)
        {
            using (var db = new pileEntities())
            {
                if (throughDate == null || !throughDate.HasValue)
                    throughDate = DateTime.MaxValue;

                var routes = db.Routes.Where(
                        x => x.CustomerID == cust.CustomerId
                            && x.Date >= DateTime.Today
                            && x.Date >= afterDate
                            && x.Date < throughDate);

                db.Routes.RemoveRange(routes);
                await db.SaveChangesAsync();
            }
        }


        private static void InsertRoute(Customer cust, DateTime weekStart)
        {
            using (var db = new pileEntities())
            {
                if (cust.Status != "A")
                    throw new Exception(string.Format("Adding a route to an inactive customer is not allowed.  CustId {0}", cust.CustomerId));
                var thisWeekend = Route.ThisWeekEnd();
                //Make sure there aren't already routes for customer & timeframe
                if (!db.Routes.Any(x => x.CustomerID == cust.CustomerId && x.Date >= weekStart && x.Date <= thisWeekend))
                {
                    //Make sure at least one other customer has routes defined in timeframe (original logic).
                    if (!db.Routes.Any(x => x.Date >= weekStart && x.Date <= thisWeekend))
                        return;
                    
                    var svcDetails = db.ServiceDetails.Where(x => x.CustomerId == cust.CustomerId).ToList();
                    if (svcDetails == null || svcDetails.Count(x => x.CountDown > 0) == 0)
                        return;

                    var routesToAdd = new List<Route>();
                    foreach (var serviceDetail in svcDetails)
                    {
                        var service = db.Services.Single(x => x.ServiceId == serviceDetail.ServiceId);
                        if (RouteQualifies(weekStart, service, serviceDetail, cust))
                        {
                            var employee = db.Employees.Single(x => x.Crew == serviceDetail.Crew && x.Status != "I");
                            var total = serviceDetail.GetStopAmout();

                            //these discounts are already figured into the total above.  The discount is stored on the route
                            //for reference purposes, I suppose.  I didn't write it. . . I just rewrote it.
                            var discounts = serviceDetail.Discount;
                            var employeeAmount = service.GetEmployeeAmount(employee, total, serviceDetail.Qty);

                            routesToAdd.Add(new Route
                            {
                                Date = weekStart.AddDays(serviceDetail.Day),
                                CustomerID = cust.CustomerId,
                                Discount = discounts,
                                EmpAmount = employeeAmount.Amount,
                                EmployeeId = employee.EmployeeID,
                                EmpPerc = (float)employeeAmount.Percent,
                                EstNum = serviceDetail.EstNum,
                                ServiceDetailId = serviceDetail.ServiceDetailId, 
                                ServiceId = service.ServiceId,
                                Status = "A",
                                WeeklyRate = total
                            });

                            if (service.CountDown != 999)
                                service.CountDown -= 1;
                        }
                    }
                    db.Routes.AddRange(routesToAdd);
                    db.SaveChanges();
                }
            }
        }


        public static bool RouteQualifies(DateTime weekStart, Service service, ServiceDetail serviceDetail, Customer cust)
        {
            DateTime? nextRouteDate = weekStart.AddDays(serviceDetail.Day);
            DateTime? lastRouteDate = serviceDetail.LastRouteDate;
            if (!lastRouteDate.HasValue)
                return false;
            return RouteQualifies(weekStart, service, serviceDetail, cust, lastRouteDate.Value, nextRouteDate.Value);
        }

        public static bool RouteQualifies(DateTime weekStart, Service service, ServiceDetail serviceDetail, Customer cust, DateTime nextRouteDate, DateTime lastRouteDate)
        {
            //int CountDown, int Freq, DateTime NextRouteDate, DateTime LastRouteDate, DateTime CustStartDate, DateTime CustPauseDate, DateTime CustReStartDate, DateTime CustFinalDate)
            int lastRouteDays = 0;
            lastRouteDays = (int)((nextRouteDate - (lastRouteDate > DateTime.Today ? DateTime.Today : lastRouteDate)).TotalDays);

            //customer is done.
            if (serviceDetail.CountDown <= 0)
                return false;

            //before Start date
            if ((cust.RouteStartDate ?? DateTime.MinValue) > nextRouteDate)
                return false;

            //During pause timeframe
            var futurePauses = cust.Pauses.Where(x => x.PauseDate > DateTime.Now || x.RestartDate > DateTime.Now);
            foreach (var pause in futurePauses)
                if (nextRouteDate > pause.PauseDate && nextRouteDate < pause.RestartDate)
                    return false;

            //after final date
            if (cust.FinalServiceDate.HasValue && cust.FinalServiceDate.Value < nextRouteDate)
                return false;

            // Freq Values
            // Weekly = 1
            // Every Other Week = 2
            // Monthly = 3
            // One Time = 4
            if (service.Freq == 4 ||
                (service.Freq == 1 && lastRouteDays < 7)  ||
                (service.Freq == 2 && lastRouteDays < 14) ||
                (service.Freq == 3 && lastRouteDays < 28))
                    return false;

            return true;
        }


        public static DateTime ThisWeekStart()
        {
            return DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        }

        public static DateTime ThisWeekEnd()
        {
            return ThisWeekStart().AddDays(6);
        }

        public static DateTime NextWeekStart()
        {
            return ThisWeekStart().AddDays(7);
        }

        public static DateTime NextWeekEnd()
        {
            return NextWeekStart().AddDays(6);
        }

        public static DateTime StartOfWeekForDate(DateTime date)
        {
            return date.AddDays(-((int)date.DayOfWeek));
        }

    }
}
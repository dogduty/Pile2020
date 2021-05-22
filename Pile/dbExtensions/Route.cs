using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Pile.db
{
    public partial class Route
    {

        public static void UpdateEstNums(int ServiceDayId)
        {
            using (var db = new pileEntities())
            {
                var serviceDay = db.ServiceDays.SingleOrDefault(x => x.Id == ServiceDayId);
                //var serviceDaysToUpdate = db.ServiceDays.Where(x => x.Day == serviceDay.d)
            }
        }

        public static void GenerateRoutes(DateTime startWeekDate)
        {
            using (var db = new pileEntities())
            {
                var endWeekDate = startWeekDate.AddDays(7);
                if (db.Routes.Any(x => x.Date >= startWeekDate && x.Date < endWeekDate))
                    return;

                var activeCustomers = db.Customers.Where(x => x.Status == "A");
                var serviceDetails = db.ServiceDetails.Where(x => x.CountDown > 0).Join(activeCustomers, s => s.CustomerId, c => c.Id, (s, c) => s);
                var pauses = db.Pauses.Where(x => x.PauseDate <= endWeekDate && x.RestartDate > startWeekDate);

                var routeList = new List<Route>();
                foreach (var serviceDetail in serviceDetails)
                {
                    var customer = activeCustomers.Single(x => x.Id == serviceDetail.CustomerId);
                    var custRoute = GenerateCustomerRoute(customer, serviceDetail, startWeekDate);
                    //don't add 'em if paused.
                    if (IsRouteValid(custRoute, customer, pauses, serviceDetail))
                        routeList.Add(custRoute);
                }

                db.Routes.AddRange(routeList);
                var toCountDownList = serviceDetails.Where(x => x.CountDown < 999);
                foreach (var toCountDown in toCountDownList)
                    if (routeList.Any(x => x.CustomerID == toCountDown.CustomerId && x.ServiceId == toCountDown.ServiceId))
                        toCountDown.CountDown -= 1;

                db.SaveChanges();
            }
        }

        public static bool IsRouteValid(Route route, Customer customer, IQueryable<Pause> pauses, ServiceDetail serviceDetail)
        {
            if (pauses.Any(x => x.CustomerId == route.CustomerID && x.PauseDate <= route.Date && x.RestartDate > route.Date))
                return false;
            if (customer.FinalServiceDate < route.Date)
                return false;
            if (serviceDetail.CountDown <= 0)
                return false;
            if (customer.StartDate > route.Date)
                return false;

            return serviceDetail.Service.Frequency.IsValidFrequency(route.Date, serviceDetail.LastRouteDate);
        }

        public static Route GenerateCustomerRoute(Customer c, ServiceDetail sd, DateTime weekStart)
        {
            var stopAmount = sd.GetStopAmout();
            var employee = sd.ServiceDay.Crew.Employees.Single();
            var route = new Route
            {
                CustomerID = c.Id,
                ServiceId = sd.ServiceId,
                Date = weekStart.AddDays(sd.ServiceDay.Day),
                EmployeeId = employee.Id,
                Status = "A",
                Discount = sd.Discount,
                EmpPerc = employee.PayPerc,
                EmpAmount = employee.GetWeeklyAmount(stopAmount, sd),
                ServiceDetailId = sd.Id
            };

            return route;

            
        }


        #region CodeToReview-NotReferencedInNew


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
            var weekStartsToCheck = new List<DateTime>() { ThisWeekStart, NextWeekStart };
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
                        x => x.CustomerID == cust.Id
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
                    throw new Exception(string.Format("Adding a route to an inactive customer is not allowed.  CustId {0}", cust.Id));
                var thisWeekend = Route.ThisWeekEnd;
                //Make sure there aren't already routes for customer & timeframe
                if (!db.Routes.Any(x => x.CustomerID == cust.Id && x.Date >= weekStart && x.Date <= thisWeekend))
                {
                    //Make sure at least one other customer has routes defined in timeframe (original logic).
                    if (!db.Routes.Any(x => x.Date >= weekStart && x.Date <= thisWeekend))
                        return;

                    var svcDetails = db.ServiceDetails.Where(x => x.CustomerId == cust.Id).ToList();
                    if (svcDetails == null || svcDetails.Count(x => x.CountDown > 0) == 0)
                        return;

                    var routesToAdd = new List<Route>();
                    foreach (var serviceDetail in svcDetails)
                    {
                        var service = db.Services.Single(x => x.Id == serviceDetail.ServiceId);
                        if (RouteQualifies(weekStart, service, serviceDetail, cust))
                        {
                            var employee = serviceDetail.ServiceDay.Crew.Employees.First(); //db.Employees.Single(x => x.Crew == serviceDetail.ServiceDay.Crew && x.Status != "I");
                            var total = serviceDetail.GetStopAmout();

                            //these discounts are already figured into the total above.  The discount is stored on the route
                            //for reference purposes, I suppose.  I didn't write it. . . I just rewrote it.
                            var discounts = serviceDetail.Discount;
                            var employeeAmount = service.GetEmployeeAmount(employee, total, serviceDetail.Qty);

                            routesToAdd.Add(new Route
                            {
                                Date = weekStart.AddDays(serviceDetail.ServiceDay.Day),
                                CustomerID = cust.Id,
                                Discount = discounts,
                                EmpAmount = employeeAmount.Amount,
                                EmployeeId = employee.Id,
                                EmpPerc = employeeAmount.Percent,
                                EstNum = serviceDetail.ServiceDay.EstNum,
                                ServiceDetailId = serviceDetail.Id,
                                ServiceId = service.Id,
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
            DateTime? nextRouteDate = weekStart.AddDays(serviceDetail.ServiceDay.Day);
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
                (service.Freq == 1 && lastRouteDays < 7) ||
                (service.Freq == 2 && lastRouteDays < 14) ||
                (service.Freq == 3 && lastRouteDays < 28))
                return false;

            return true;
        }


        public static DateTime ThisWeekStart
        {
            get { return DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek); }
        }

        public static DateTime ThisWeekEnd
        {
            get { return ThisWeekStart.AddDays(6); }
        }

        public static DateTime NextWeekStart
        {
            get { return ThisWeekStart.AddDays(7); }
        }

        public static DateTime NextWeekEnd
        {
            get { return NextWeekStart.AddDays(6); }
        }

        public static DateTime StartOfWeekForDate(DateTime date)
        {
            return date.AddDays(-((int)date.DayOfWeek));
        } 

        #endregion

    }
}
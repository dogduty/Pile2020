using Pile.db;
using Pile.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Pile.Controllers
{
    [RoutePrefix("api/Routes")]
    public class ApiRoutesController : ApiController
    {
        private pileEntities db = new pileEntities();

        //[Route("")]
        //[HttpGet]
        //public async Task<IHttpActionResult> GetAll()
        //{
        //    db.Configuration.LazyLoadingEnabled = false;

        //    var services = await db.Services.ToListAsync();
        //    var crews = await db.Crews.Include(x => x.Employees).ToListAsync();

        //    var infos = await db.Customers
        //                        .Where(x => x.Status == "A")
        //                        .Join(db.ServiceDays, c => c.Id, s => s.CustomerId, (Customer, ServiceDay) => new MapInfo {
        //                            CustomerId = Customer.Id,
        //                            CustomerName = Customer.FirstName + " " + Customer.LastName ?? "",
        //                            Address = Customer.Addresses.FirstOrDefault(x => x.AddressType == "Site").Address1,
        //                            Lat = Customer.Addresses.FirstOrDefault(x => x.AddressType == "Site").Lat,
        //                            Lng = Customer.Addresses.FirstOrDefault(x => x.AddressType == "Site").Lng,
        //                            CrewId = ServiceDay.CrewId,
        //                            Day = ServiceDay.Day,
        //                            Color = ServiceDay.Crew.MarkerColor,
        //                            TextColor = ServiceDay.Crew.TextColor,
        //                            Stop = ServiceDay.EstNum,
        //                            ServiceIds = ServiceDay.ServiceDetails.Select(x => x.ServiceId)
        //                        }).ToListAsync();
        //    foreach (var info in infos)
        //    {
        //        info.Services = string.Join(", ", services.Where(x => info.ServiceIds.Contains(x.Id)).Select(x => x.Description));
        //        //    Services = string.Join(", ", ServicesList.Where(x => serviceIds.Contains(x.ServiceId)).Select(x => x.Description));
        //    }

        //    var days = infos.Select(x => new { Day = x.Day, Dow = Enum.GetName(typeof(DayOfWeek), x.Day) }).OrderBy(x => x.Day).Distinct().ToArray();

        //    return Ok(new { Days = days, Crews = crews, Infos = infos });
        //}

        [Route("RouteList")]
        [HttpGet]
        public async Task<IHttpActionResult> GetRouteList([FromUri] bool nextWeek = false)
        {
            try
            {
                db.Configuration.LazyLoadingEnabled = false;
                var routeInfos = new List<RouteInfo>();
                var employees = await db.Employees.Where(x => x.Status != "I" && x.CrewId != null).Select(x => new {
                    firstName = x.FirstName,
                    crewId = x.CrewId,
                    id = x.Id,
                    selected = true
                }).ToListAsync();

                var services = await db.Services.ToListAsync();
                //TODO: Make this real
                DateTime startDate = new DateTime(2019, 05, 12).AddDays(nextWeek ? 7 : 0);
                DateTime endDate = new DateTime(2019, 05, 18).AddDays(nextWeek ? 7 : 0);
                //DateTime startDate = Route.ThisWeekStart.AddDays(nextWeek ? 7 : 0);
                //DateTime endDate = Route.ThisWeekEnd.AddDays(nextWeek ? 7 : 0);
                var routes = db.Routes.Where(x => x.Date >= startDate && x.Date <= endDate)
                    .Include(x => x.Customer)
                    .Include(x => x.Customer.Addresses)
                    .Include(x => x.Customer.Notes)
                    .Include(x => x.Customer.ServiceDetails)
                    .Include(x => x.Customer.ServiceDetails.Select(c => c.ServiceDay));

                foreach (var route in routes)
                {
                    var routeDay = (int)route.Date.DayOfWeek;
                    if (routeInfos.Any(x => x.CustomerId == route.CustomerID && x.EmployeeId == route.EmployeeId && routeDay == x.Day))
                        continue; //list is unique by the above criteria
                    var info = new RouteInfo
                    {
                        Id = route.Id,
                        Selected = false,
                        Address = route.Customer.Addresses.First(x => x.AddressType == "Site").Address1,
                        CustomerId = route.Customer.Addresses.First(x => x.AddressType == "Site").CustomerId,
                        Code = route.Customer.Code,
                        Day = routeDay,
                        FirstName = route.Customer.FirstName,
                        LastName = route.Customer.LastName,
                        Flag = route.Customer.Flag,
                        GateCode = route.Customer.Combo,
                        LatePayment = route.Customer.LatePmt,
                        Visible = true
                    };
                    var routeServiceIds = route.Customer.ServiceDetails.Where(s => s.ServiceDay.Day == routeDay).Select(s => s.ServiceId).ToList();
                    var routeServices = services.Where(x => routeServiceIds.Contains(x.Id));
                    if (routeServices != null)
                        info.Services = string.Join(",", routeServices.Select(x => x.Description).ToList());
                    info.Notes = string.Join(" | ", route.Customer.Notes.OrderByDescending(x => x.Id).Select(x => x.Content).ToList());
                    var employee = employees.SingleOrDefault(x => x.id == route.EmployeeId);
                    if (employee != null)
                    {
                        info.EmployeeId = employee.id;
                        info.CrewId = employee.crewId ?? 0;
                        info.EmployeeName = employee.firstName;
                    }
                    routeInfos.Add(info);
                }

                return Ok(new { startDate, endDate, employees, selectAll = false, transferTo = 0, routeInfos = routeInfos.OrderBy(x => x.Day).ThenBy(x => x.CrewId).ToList() });

            }

            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [Route("Transfer/{crewId}")]
        [HttpPost]
        public async Task<IHttpActionResult> TransferRoutes(int crewId, List<int> RouteIds)
        {
            try
            {
                var emp = db.Employees.Single(x => x.CrewId == crewId);
                var donorRoutes = db.Routes.Where(x => RouteIds.Contains(x.Id) && x.EmployeeId != emp.Id).OrderBy(x => x.Date).ThenBy(x => x.EmployeeId).ThenBy(x => x.EstNum);
                var servicDetailsIds = await donorRoutes.Select(x => x.ServiceDetailId).Distinct().ToListAsync();
                var serviceDetails = db.ServiceDetails.Where(x => servicDetailsIds.Contains(x.Id));
                var days = donorRoutes.Select(x => x.Date).Distinct();

                foreach (var day in days)
                {
                    var lastRecipientRouteEstNum = db.Routes.Where(x => x.EmployeeId == emp.Id && x.Date == day).Max(x => x.EstNum);

                    foreach (var route in donorRoutes)
                    {
                        route.EmployeeId = emp.Id;
                        route.EstNum = ++lastRecipientRouteEstNum;
                        route.EmpPerc = emp.PayPerc;
                        route.EmpAmount = emp.GetWeeklyAmount(route.WeeklyRate.Value, serviceDetails.Single(x => x.Id == route.ServiceDetailId));
                    }
                }

                await db.SaveChangesAsync();
                return Ok(new { message = "Routes updated" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}

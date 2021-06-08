using Microsoft.AspNet.Identity.EntityFramework;
using Pile.db;
using Pile.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Pile.Controllers
{
    [RoutePrefix("api/MobileRoutes")]
    public class ApiMobileRouteController : ApiController
    {
        private pileEntities db = new pileEntities();

        [Route("")]
        public async Task<IHttpActionResult> GetStops()
        {
            db.Configuration.LazyLoadingEnabled = false;
            int employeeId = ApplicationUserManager.GetCurrentUserEmployeeId();
            var emp = await db.Employees.SingleAsync(x => x.Id == employeeId);
            var earliestDate = DateTime.Now.AddDays(-1 * Settings.GetValue("MaxMissedDays", 7));

            //TODO remove this:
            earliestDate = new DateTime(2019, 5, 22);

            var routes = db.Routes.Where(x => x.EmployeeId == employeeId &&
                (x.LastServiceDate == null || x.LastServiceDate == DateTime.Today) &&
                x.Date > earliestDate );

            var customerIds = routes.Select(x => x.CustomerID).Distinct();
            var serviceIds = routes.Select(x => x.ServiceId).Distinct();
            
            var stops = await routes.Select(x => new { x.Date, x.EstNum, x.CustomerID, x.ServiceId, x.ServiceDetailId, x.LastServiceDate }).OrderBy(x => x.Date).ThenBy(x => x.EstNum).ToListAsync();
            var serviceDescriptions = await db.Services.Where(x => serviceIds.Contains(x.Id)).Select(x => new { x.Id, x.Description }).ToListAsync();

            var customers = await db.Customers.Where(x => customerIds.Contains(x.Id)).Distinct().Select(x => new {
                x.Id,
                x.FirstName,
                x.LastName,
                x.Addresses.FirstOrDefault(a => a.AddressType == "Site").Address1,
                x.Addresses.FirstOrDefault(a => a.AddressType == "Site").Address2,
                x.RouteStartDate,
                x.Code,
                x.Combo,
                x.Notes,
                x.Phones
            }).ToListAsync();

            List<MobileRouteStopViewModel> stopInfos = new List<MobileRouteStopViewModel>();
            var yardProblems = Settings.GetValue("yardConditions", "").Split('|');
            var yardConditions = yardProblems.Select(x => new YardConditionsViewModel
            {
                Selected = x == "",
                ShowCheck = x != "",
                Problem = x
            }).ToList();

            foreach(var stop in stops)
            {
                var stopInfo = stopInfos.SingleOrDefault(x => x.Date == stop.Date && x.CustomerId == stop.CustomerID);
                var customer = customers.FirstOrDefault(x => x.Id == stop.CustomerID);

                if (stopInfo == null)
                {
                    stopInfo = new MobileRouteStopViewModel
                    {
                        Address1 = customer.Address1,
                        Address2 = customer.Address2,
                        CustomerId = customer.Id,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        RouteStartDate = customer.RouteStartDate.Value,
                        Code = customer.Code,
                        Combo = customer.Combo,
                        Notes = customer.Notes.ToList(),
                        Phones = customer.Phones.ToList(),
                        Completed = stop.LastServiceDate != null,
                        Date = stop.Date,
                        EstNum = stop.EstNum,
                        Details = new List<MobileRouteStopDetailViewModel>(),
                        Conditions = yardConditions,
                        Expand = false
                    };
                    stopInfos.Add(stopInfo);
                }

                stopInfo.Details.Add(new MobileRouteStopDetailViewModel()
                {
                    ServiceDetailId = stop.ServiceDetailId,
                    Completed = stopInfo.Completed,
                    Description = serviceDescriptions.FirstOrDefault(x => x.Id == stop.ServiceId).Description
                });
            }


            return Ok(new { emp.FirstName, stopInfos });

        }

        [Route("")]
        [HttpPost]
        public async Task<IHttpActionResult> MarkCompleted(MobileRouteStopViewModel stop)
        {
            try
            {
                int employeeId = ApplicationUserManager.GetCurrentUserEmployeeId();
                var routes = db.Routes.Where(x => x.EmployeeId == employeeId && x.Date == stop.Date && x.CustomerID == stop.CustomerId);
                foreach (var route in routes)
                    if (!stop.Details.Any(x => x.ServiceDetailId == route.ServiceDetailId))
                        return BadRequest("Not all services completed.  Try reloading page.");

                foreach (var route in routes)
                    route.LastServiceDate = route.LastServiceDate ?? DateTime.Today;

                string problems = string.Join(", ", stop.Conditions.Where(x => x.Selected && x.Problem != "").Select(x => x.Problem).ToList());
                if (problems != "")
                    db.ServiceHistories.Add(new ServiceHistory
                    {
                        CustomerId = stop.CustomerId,
                        ServiceDate = DateTime.Now,
                        EmployeeId = employeeId,
                        Complaint = false,
                        Description = problems
                    });


                await db.SaveChangesAsync();
                return Ok();
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
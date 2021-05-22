using Pile.db;
using Pile.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

namespace Pile.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/ServiceDetails")]
    public class apiServiceDetailsController : ApiController
    {
        private pileEntities db = new pileEntities();

        [Route("details/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            var customer = db.Customers.Single(x => x.Id == id);
            var crews = await db.Employees.Where(x => x.CrewId != null).ToListAsync();// db.Crews.Select(x => x.Employees).ToListAsync(); //db.Employees.Where(x => x.Status != "I" && x.Crew != null).OrderBy(e => e.Crew);
            var serviceDays = await db.ServiceDays.Where(x => x.CustomerId == id).Include(x => x.ServiceDetails).OrderBy(x => x.Day).ToListAsync();
            var serviceIdsInUse = serviceDays.SelectMany(x => x.ServiceDetails).Select(x => x.ServiceId).Distinct().ToList();
            var services = await db.Services.Where(x => x.Active == true || serviceIdsInUse.Contains(x.Id)).OrderBy(x => x.DisplayOrder).ToListAsync();

            return Request.CreateResponse(HttpStatusCode.OK, new { Customer = customer, ServiceDays = serviceDays, Crews = crews, Services = services });
        }

        [Route("{customerId}")]
        [HttpPost]
        public async Task<HttpResponseMessage> CreateOrUpdate(int customerId, List<ServiceDay> serviceDays)
        {
            var validationMessage = ValidateServiceDays(customerId, serviceDays);
            if (validationMessage != "")
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, validationMessage);

            try
            {
                var newServiceDays = serviceDays.Where(x => x.Id == 0);
                var updatedServiceDays = serviceDays.Where(x => x.Id != 0);
                var updatedIds = updatedServiceDays.Select(x => x.Id);
                var serviceDaysToDelete = db.ServiceDays.Where(x => x.CustomerId == customerId && !updatedIds.Contains(x.Id));

                if (serviceDaysToDelete.Count() > 0)
                    db.ServiceDays.RemoveRange(serviceDaysToDelete);

                foreach (var newDay in newServiceDays)
                    db.ServiceDays.Add(newDay);

                foreach (var update in updatedServiceDays) { 
                    db.Entry(update).State = EntityState.Modified;
                    foreach (var sd in update.ServiceDetails) {
                        if (sd.Id == 0)
                            db.ServiceDetails.Add(sd);
                        else
                            db.Entry(sd).State = EntityState.Modified;
                    }
                }


                await db.SaveChangesAsync();

                //{
                //    var entry = db.Entry(update);
                //    if (entry.State == EntityState.Detached || entry.State == EntityState.Modified)
                //    {
                //        entry.State = EntityState.Modified;
                //        db.Set<ServiceDay>().Attach(update);
                //    }
                //}

                //await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            var resp = Request.CreateResponse(HttpStatusCode.NoContent); //, new GenericCreateResponse());
            resp.Headers.Location = new Uri($"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/ServiceDetails/Edit?id={customerId}");

            return resp;

        }

        private string ValidateServiceDays(int CustomerId, List<ServiceDay> serviceDays)
        {
            foreach (var sd in serviceDays)
            {
                if (sd.CustomerId == 0)
                    sd.CustomerId = CustomerId;
                if (sd.CustomerId != sd.CustomerId)
                    return ($"CustomerId mismatch on ServiceDay object: {sd.CustomerId} - customer: {CustomerId}");
                if (sd.Day < 0 || sd.Day > 6)
                    return ($"Invalid day given: {sd.Day}");
                if (sd.NumDogs <= 0)
                    return ($"Number of dogs must be greater than zero.");
                if (sd.EstNum <= 0)
                    return ($"EstNum must be greater than zero.");
                if (sd.ServiceDetails.Count() < 1)
                    return ($"Service Day {Enum.GetName(typeof(DayOfWeek), sd.Day)} must include at least one Service");
                var sdValidation = ValidateServiceDetails(sd);
                if (sdValidation != "")
                    return sdValidation;
            }
            var duplicateDays = serviceDays.GroupBy(x => x.Day).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (duplicateDays.Count() > 0)
                return ($"Duplicated day - {Enum.GetName(typeof(DayOfWeek), duplicateDays.First())}");

            return "";
        }

        private string ValidateServiceDetails(ServiceDay serviceDay)
        {
            foreach (var s in serviceDay.ServiceDetails)
            {
                if (s.ServiceDayId == 0)
                    s.ServiceDayId = serviceDay.Id;
                if (s.ServiceDayId != serviceDay.Id)
                    return $"ServiceDayId mismatch.  Rejected.";
                if (s.Qty < 0)
                    return $"Qty must be zero or greater.";
            }

            return "";
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

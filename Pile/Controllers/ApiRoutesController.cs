using Pile.db;
using Pile.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Pile.Controllers
{
    [RoutePrefix("api/Routes")]
    public class ApiRoutesController : ApiController
    {
        private pileEntities db = new pileEntities();

        [Route("")]
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            var services = await db.Services.ToListAsync();
            var crews = await db.Crews.ToListAsync();

            var infos = await db.Customers
                                .Where(x => x.Status == "A")
                                .Join(db.ServiceDays, c => c.CustomerId, s => s.CustomerId, (Customer, ServiceDay) => new MapInfo {
                                    CustomerId = Customer.CustomerId,
                                    CustomerName = Customer.FirstName + " " + Customer.LastName ?? "",
                                    Address = Customer.Addresses.FirstOrDefault(x => x.AddressType == "Site").Address1,
                                    Lat = Customer.Addresses.FirstOrDefault(x => x.AddressType == "Site").Lat,
                                    Lng = Customer.Addresses.FirstOrDefault(x => x.AddressType == "Site").Lng,
                                    CrewId = ServiceDay.CrewId,
                                    Day = ServiceDay.Day,
                                    Color = ServiceDay.Crew.MarkerColor,
                                    TextColor = ServiceDay.Crew.TextColor,
                                    Stop = ServiceDay.EstNum,
                                    ServiceIds = ServiceDay.ServiceDetails.Select(x => x.ServiceId)
                                }).ToListAsync();
            foreach (var info in infos)
            {
                info.Services = string.Join(", ", services.Where(x => info.ServiceIds.Contains(x.ServiceId)).Select(x => x.Description));
                //    Services = string.Join(", ", ServicesList.Where(x => serviceIds.Contains(x.ServiceId)).Select(x => x.Description));
            }

            var days = infos.Select(x => new { Day = x.Day, Dow = Enum.GetName(typeof(DayOfWeek), x.Day) }).OrderBy(x => x.Day).Distinct().ToArray();

            return Ok(new { Days = days, Crews = crews, Infos = infos });
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

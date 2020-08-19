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
    [RoutePrefix("api/ServiceHistories")]
    public class ApiServiceHistoriesController : ApiController
    {
        private pileEntities db = new pileEntities();

        [Route("customer")]
        public async Task<IHttpActionResult> GetForCustomer([FromUri] int id)
        {
            var customer = await db.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();

            var histories = await db.ServiceHistories.Where(x => x.CustomerId == id).OrderByDescending(x => x.Id)
                                .Join(db.Employees, h => h.EmployeeId, e => e.EmployeeID, (h, e) => new {
                                        detail = h, employeeFirst = e.FirstName, employeeLast = e.LastName
                                }).ToListAsync();
                                
            return Ok(new { customer, histories });
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

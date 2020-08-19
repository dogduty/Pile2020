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
    [RoutePrefix("api/Employees")]
    public class ApiEmployeesController : ApiController
    {
        private pileEntities db = new pileEntities();


        [Route("")]
        public async Task<IHttpActionResult> GetEmployees([FromUri] bool showall=false)
        {
            List<Employee> employees;

            if (showall)
                employees = await db.Employees.ToListAsync();
            else
                employees = await db.Employees.Where(x => x.Status != "I").ToListAsync();

            return Ok(employees);
        }

        [Route("{id}")]
        public async Task<IHttpActionResult> GetEmployee(int id)
        {
            var employee = await db.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            return Ok(employee);
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

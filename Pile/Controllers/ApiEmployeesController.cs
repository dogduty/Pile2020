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
    [RoutePrefix("api/Employees")]
    public class ApiEmployeesController : ApiController
    {
        private pileEntities db = new pileEntities();


        [Route("")]
        public async Task<IHttpActionResult> GetEmployees([FromUri] bool showall=false)
        {
            db.Configuration.LazyLoadingEnabled = false;
            var users = await ApplicationUserManager.GetApplicationUsers();
            var employees = await db.Employees.ToListAsync();

            foreach (var user in users)
                employees.Single(x => x.Id == user.EmployeeId).User = user;

            return Ok(users);
        }

        [Route("new")] //this should just be a post to create a new. Maybe shared with update
        public async Task<IHttpActionResult> NewEmployee()
        {
            var empLogin = EmployeeLogin.Create();
            return Ok(empLogin);
        }

        [Route("activate/{id}")]
        public async Task<IHttpActionResult> ActivateEmployee(int id)
        {
            var empLogin = EmployeeLogin.Create();
            empLogin.Employee = await db.Employees.FindAsync(id);
            if (empLogin.Employee == null)
                return NotFound();

            empLogin.ApplicationUser.UserName = empLogin.Employee.Email;
            empLogin.ApplicationUser.EmployeeId = empLogin.Employee.Id;

            return Ok(empLogin);
        }

        [Route("details/{id}")]
        public async Task<IHttpActionResult> GetEmployee(int id)
        {
            var empLogin = EmployeeLogin.Create();
            empLogin.Employee = await db.Employees.FindAsync(id);
            if (empLogin.Employee == null)
                return NotFound();

            empLogin.ApplicationUser = await ApplicationUserManager.GetApplicationUser(id);

            return Ok(empLogin);
        }

        [Route("")]
        [HttpPost]
        public async Task<HttpResponseMessage> Create([FromBody] EmployeeLogin empLogin)
        {
            return await Update(0, empLogin);
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> Update(int id, [FromBody] EmployeeLogin empLogin)
        {
            bool isNew = empLogin.Employee.Id == 0;
            Employee dbEmp;

            if (isNew)
                dbEmp = db.Employees.Add(empLogin.Employee);
            else
                dbEmp = await db.Employees.FindAsync(empLogin.Employee.Id);

            string validation = null;
            var props = dbEmp.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).ToList();
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var value in ModelState.Values)
                    foreach (var error in value.Errors)
                        errors.Add(error.ErrorMessage);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(" | ", errors));
            }
            //else
            //    validation = customer.Validate();

            if (validation != null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, validation);

            var resp = Request.CreateResponse(isNew ? HttpStatusCode.Created : HttpStatusCode.NoContent, new GenericCreateResponse());

            //try
            //{
            //    DeleteUnfinishedChildren(customer);
            //    if (isNew)
            //    {
            //        db.Customers.Add(customer);
            //    }
            //    else
            //    {
            //        StampCustIdOnChildObjects(customer);
            //        db.Entry(customer).State = EntityState.Modified;
            //        customer.SaveChildObjects(db);
            //    }

            //    await db.SaveChangesAsync();

            //    if (isNew)
            //        resp.Headers.Location = new Uri($"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/Customers/edit?id={customer.Id}");
            //}

            //catch (DbEntityValidationException ve)
            //{
            //    var errorList = new List<string>();
            //    foreach (var e in ve.EntityValidationErrors)
            //        errorList.Add($"props:{string.Join(",", e.ValidationErrors.Select(x => x.PropertyName).ToArray())} | errors:{string.Join(",", e.ValidationErrors.Select(x => x.ErrorMessage).ToArray())} ");
            //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join("<br />", errorList));
            //}

            //catch (Exception ex)
            //{
            //    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            //}

            return resp;
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

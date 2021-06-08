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

            return Ok(employees);
        }

        [Route("new")] //this should just be a post to create a new. Maybe shared with update
        public async Task<IHttpActionResult> NewEmployee()
        {
            var emp = Employee.Create();
            return Ok(emp);
        }

        [Route("deactivate/{id}")]
        public async Task<IHttpActionResult> DeactivateEmployee(int id)
        {
            var emp = await db.Employees.FindAsync(id);
            if (emp == null)
                return NotFound();

            var appUser = await ApplicationUserManager.GetApplicationUserAsync(id);

            if (appUser.Id == ApplicationUserManager.GetCurrentUser().Id)
                return BadRequest("You cannot delete yourself!");

            await ApplicationUserManager.DeleteApplicationUserAsync(appUser);

            return Ok(emp);
        }

        [Route("activate/{id}")]
        public async Task<IHttpActionResult> ActivateEmployee(int id)
        {
            var emp = await db.Employees.FindAsync(id);
            if (emp == null)
                return NotFound();

            var appUser = new ApplicationUser();
            appUser.UserName = emp.Email;
            appUser.EmployeeId = emp.Id;

            emp.User = appUser;

            return Ok(emp);
        }

        [Route("details/{id}")]
        public async Task<IHttpActionResult> GetEmployee(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            var emp = await db.Employees.FindAsync(id);
            if (emp == null)
                return NotFound();

            emp.User = await ApplicationUserManager.GetApplicationUserAsync(id);

            return Ok(emp);
        }

        [Route("")]
        [HttpPost]
        public async Task<HttpResponseMessage> Create([FromBody] Employee emp)
        {
            return await Update(0, emp);
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> Update(int id, [FromBody] Employee emp)
        {
            bool isNew = emp.Id == 0;

            string validation = null;
            var props = emp.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).ToList();
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var value in ModelState.Values)
                    foreach (var error in value.Errors)
                        errors.Add(error.ErrorMessage);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(" | ", errors));
            }

            var appUser = await ApplicationUserManager.GetApplicationUserAsync(id);

            if (emp.User != null)
            {
                if (appUser == null)
                {
                    var manager = ApplicationUserManager.GetCurrent();
                    appUser = new ApplicationUser { UserName = emp.User.UserName, Email = emp.User.Email, EmployeeId = emp.Id };

                    var result = await manager.CreateAsync(appUser, "qwerASDF1234!@#$asdf");
                    //var result = await manager.CreateAsync(appUser, "qwerASDF1234!@#$asdf");
                    if (result.Succeeded)
                    {
                        //appUser = await ApplicationUserManager.GetApplicationUserAsync(id);
                        await appUser.SendEmailConfirmation(manager);
                    }
                    else
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, $"Error creating Application User {emp.User.UserName}.  Error = {string.Join("|", result.Errors)}");
                }

            }


            if (validation != null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, validation);

            try
            {
                if (isNew)
                    db.Employees.Add(emp);
                else
                    db.Entry(emp).State = EntityState.Modified;

                await db.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            var resp = Request.CreateResponse(isNew ? HttpStatusCode.Created : HttpStatusCode.NoContent, new GenericCreateResponse());

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

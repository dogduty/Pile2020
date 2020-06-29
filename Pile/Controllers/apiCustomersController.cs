using Pile.db;
using Pile.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Pile.Controllers
{
    [RoutePrefix("api/Customers")]
    public class apiCustomersController : ApiController
    {
        private pileEntities db = new pileEntities();

        [Route("")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAll(bool showAll = false)
        {
            var customers = db.Customers.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
            if (showAll)
                customers = (IOrderedQueryable<Customer>)customers.Where(x => x.Status == "A");

            var custGrid = customers.Select(x => new {
                Status = x.Status,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Addresses = x.Addresses.FirstOrDefault(a => a.AddressType == "Site"),
                Phonees = x.Phones,
                Email = x.EmailAddresses.FirstOrDefault(e => e.IsPrimary == true),
                SpouseFirstName = x.SpouseFirstName,
                SpouseLastName = x.SpouseLastName
            });

            var display = new { ShowAll = showAll, Customers = await custGrid.ToListAsync() };

            return Request.CreateResponse(HttpStatusCode.OK, display);
        }

        [Route("new")]
        [HttpGet]
        public async Task<HttpResponseMessage> New()
        {
            var customer = new Customer();
            customer.CustomerId = 6527;
            customer.Addresses.Add(new Address { AddressType = "Site" });
            customer.Addresses.Add(new Address { AddressType = "Mailing" });
            return Request.CreateResponse(HttpStatusCode.OK, customer);
        }

        [Route("")]
        [HttpPost]
        public async Task<HttpResponseMessage> Create([FromBody] Customer customer)
        {
            var resp = Request.CreateResponse(HttpStatusCode.Created, new GenericCreateResponse());
            resp.Headers.Location = new Uri($"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/Customers/{customer.CustomerId}");
            return resp;
        }

        [Route("DupeCheck")]
        public async Task<HttpResponseMessage> Get(string email, string lastName, string address)
        {
            var possibleMatches = new List<Customer>();

            if (email != null) 
                possibleMatches.AddRange(db.EmailAddresses.Where(e => e.Email.ToLower() == email.ToLower()).Select(e => e.Customer));
            if (lastName != null)
                possibleMatches.AddRange(db.Customers.Where(x => x.LastName.ToLower().Contains(lastName)));

            var matches = possibleMatches.Select(x => new
            {
                CustomerId = x.CustomerId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.EmailAddresses.FirstOrDefault(e => e.Email.ToLower() == email.ToLower()).Email,
                Address = x.Addresses.FirstOrDefault(a => a.AddressType == "Site").Address1,
                City = x.Addresses.FirstOrDefault(a => a.AddressType == "Site").City,
                Zip = x.Addresses.FirstOrDefault(a => a.AddressType == "Site").Zip,
            });

            return Request.CreateResponse(HttpStatusCode.OK, matches);
        }

        [Route("Tasks")]
        public async Task<HttpResponseMessage> GetTasks()
        {
            //var tasks = await db.CustomerTasks.Where(x => !x.Completed).ToListAsync();
            //return Request.CreateResponse(tasks);
            return null;
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

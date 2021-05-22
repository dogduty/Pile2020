using Pile.db;
using Pile.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Pile.Controllers
{
    [RoutePrefix("api/Customers")]
    public class ApiCustomersController : ApiController
    {
        private pileEntities db = new pileEntities();

        [Route("")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAll(bool showAll = false)
        {
            var customers = db.Customers.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);
            if (!showAll)
                customers = (IOrderedQueryable<Customer>)customers.Where(x => x.Status == "A");

            var custGrid = customers.Select(x => new {
                Id = x.Id,
                Status = x.Status,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Address = x.Addresses.Select(a => new {
                        AddressType = a.AddressType,
                        Address1 = a.Address1,
                        City = a.City,
                        Zip = a.Zip
                    }).FirstOrDefault(y => y.AddressType == "Site"),
                Mobile = x.Phones.Select(p => new {
                    p.PhoneType,
                    p.Number
                }).FirstOrDefault(p => p.PhoneType == "Mobile"),
                Home = x.Phones.Select(p => new {
                    p.PhoneType,
                    p.Number
                }).FirstOrDefault(p => p.PhoneType == "Home"),
                Email = x.EmailAddresses.FirstOrDefault(e => e.IsPrimary == true).Email,
                SpouseFirstName = x.SpouseFirstName,
                SpouseLastName = x.SpouseLastName
            });

            var display = new { ShowAll = showAll, Customers = await custGrid.ToListAsync() }; //, HowFounds = howFounds, PaymentMethods = paymentMethods, InvoiceMethods = invoiceMethods };

            return Request.CreateResponse(HttpStatusCode.OK, display);
        }

        [Route("new")]
        [HttpGet]
        public async Task<HttpResponseMessage> New()
        {
            var customer = new Customer { Status = "A", Type = "R" };
            customer.Addresses.Add(new Address { AddressType = "Site", State = "TX" });
            customer.EmailAddresses.Add(new EmailAddress { IsPrimary = true, ServiceEmails = true });

            var resp = new { Customer = customer }; //, HowFounds = howFounds, PaymentMethods = paymentMethods, InvoiceMethods = invoiceMethods };
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

        [Route("")]
        [HttpPost]
        public async Task<HttpResponseMessage> Create([FromBody] Customer customer)
        {
            return await Update(0, customer);
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<HttpResponseMessage> Update(int id, [FromBody] Customer customer)
        {
            bool isNew = customer.Id == 0;
            string validation = null;
            var props = customer.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).ToList();
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var value in ModelState.Values)
                    foreach (var error in value.Errors)
                        errors.Add(error.ErrorMessage);
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join(" | ", errors));
            }
            else
                validation = customer.Validate();

            if (validation != null)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, validation);

            var resp = Request.CreateResponse(isNew ? HttpStatusCode.Created : HttpStatusCode.NoContent, new GenericCreateResponse());

            try
            {
                DeleteUnfinishedChildren(customer);
                if (isNew)
                {
                    db.Customers.Add(customer);
                }
                else
                {
                    StampCustIdOnChildObjects(customer);
                    db.Entry(customer).State = EntityState.Modified;
                    customer.SaveChildObjects(db);
                }

                await db.SaveChangesAsync();

                if (isNew)
                    resp.Headers.Location = new Uri($"{Request.RequestUri.Scheme}://{Request.RequestUri.Authority}/Customers/edit?id={customer.Id}");
            }

            catch (DbEntityValidationException ve)
            {
                var errorList = new List<string>();
                foreach (var e in ve.EntityValidationErrors)
                    errorList.Add($"props:{string.Join(",", e.ValidationErrors.Select(x => x.PropertyName).ToArray())} | errors:{string.Join(",", e.ValidationErrors.Select(x => x.ErrorMessage).ToArray())} ");
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, string.Join("<br />", errorList));
            }

            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

            return resp;
        }

        [Route("details/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            var cust = await db.Customers
                .Include(x => x.Addresses)
                .Include(x => x.EmailAddresses)
                .Include(x => x.Notes)
                .Include(x => x.Phones)
                .SingleOrDefaultAsync(x => x.Id == id);

            var svcSummary = ServiceSummary.GetServiceSummary(db, id);           

            var resp = new { Customer = cust, ServiceSummary = svcSummary }; //, HowFounds = howFounds, PaymentMethods = paymentMethods, InvoiceMethods = invoiceMethods };
            return Request.CreateResponse(HttpStatusCode.OK, resp);
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
                CustomerId = x.Id,
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

        private void StampCustIdOnChildObjects(Customer cust)
        {
            foreach (var phone in cust.Phones.Where(x => x.CustomerId == 0))
                phone.CustomerId = cust.Id;
            foreach (var address in cust.Addresses.Where(x => x.CustomerId == 0))
                address.CustomerId = cust.Id;
            foreach (var email in cust.EmailAddresses.Where(x => x.CustomerId == 0))
                email.CustomerId = cust.Id;
            foreach (var note in cust.Notes.Where(x => x.CustomerId == 0))
                note.CustomerId = cust.Id;
        }


        private void DeleteUnfinishedChildren(Customer cust)
        {
            //clicked to add phone number, but didn't supply one.  Garbage.
            var phoneList = cust.Phones.ToList();
            for (int i = phoneList.Count() - 1; i >= 0; i--)
                if (phoneList[i].Id == 0 && string.IsNullOrWhiteSpace(phoneList[i].Number))
                    cust.Phones.Remove(phoneList[i]);

            //addresses

            var addressList = cust.Addresses.ToList();
            for (int i = addressList.Count() - 1; i >= 0; i--)
                if (addressList[i].Id == 0 && (
                    string.IsNullOrWhiteSpace(addressList[i].Address1) ||
                    string.IsNullOrWhiteSpace(addressList[i].City) ||
                    string.IsNullOrWhiteSpace(addressList[i].State) ||
                    string.IsNullOrWhiteSpace(addressList[i].Zip)
                ))
                    cust.Addresses.Remove(addressList[i]);


            var emailList = cust.EmailAddresses.ToList();
            for (int i = emailList.Count() - 1; i >= 0; i--)
                if (string.IsNullOrWhiteSpace(emailList[i].Email))
                    cust.EmailAddresses.Remove(emailList[i]);

            var notesList = cust.Notes.ToList();
            for (int i = notesList.Count() - 1; i >= 0; i--)
                if (string.IsNullOrWhiteSpace(notesList[i].Content))
                    cust.Notes.Remove(notesList[i]);

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

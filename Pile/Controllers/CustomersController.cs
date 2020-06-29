using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Pile.db;
using Pile.Models;
using System.Reflection;

namespace Pile.Controllers
{
    public class CustomersController : Controller
    {
        private pileEntities db = new pileEntities();

        // GET: Customers
        public async Task<ActionResult> Index(bool showAll = false)
        {
            //HttpCookie darkMode = new HttpCookie("DarkMode");
            //darkMode.Expires = DateTime.Now.AddMinutes(1);
            //Response.Cookies.Add(darkMode);

            ViewBag.ShowAll = showAll;
            using (var db = new pileEntities())
            {
                var customers = db.Customers.OrderBy(x => x.LastName).ThenBy(x => x.FirstName);

                if (!showAll)
                    customers = (IOrderedQueryable<Customer>)customers.Where(x => x.Status == "A");
                
                return View(await customers.Include(x => x.Addresses).Include(x => x.EmailAddresses).Include(x => x.Phones).ToListAsync());
            }
        }

        // GET: Search
        public async Task<ActionResult> Search(string SearchValue, string SearchAll="off")
        {
            using (var db = new pileEntities())
            {
                SearchValue = SearchValue.ToLower();
                var customers = db.Customers.Where( x =>
                        x.LastName.ToLower().Contains(SearchValue) ||
                        x.FirstName.ToLower().Contains(SearchValue) ||
                        x.SpouseFirstName.ToLower().Contains(SearchValue) ||
                        x.SpouseLastName.ToLower().Contains(SearchValue) ||
                        //x.Email.ToLower().Contains(SearchValue) ||
                        //x.Address.ToLower().Contains(SearchValue) ||
                        x.CustomerId.ToString() == SearchValue
                    );

                if (SearchAll == "on" )
                    return View("Index", await customers.ToListAsync());


                //customers = (IOrderedQueryable<Customer>)customers.Where(x => x.Status == "A");

                return View("Index", await customers.Where(x => x.Status == "A").ToListAsync());
            }
        }

        // GET: Customers/Details/5
        public async Task<ActionResult> Details(int? id, string gpsMessage = null)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var summary = await GetCustomerSummary(id);

            if (summary == null)
                return HttpNotFound();

            if (gpsMessage != null)
                ModelState.AddModelError("", gpsMessage);

            return View(summary);
        }

        // GET: Customers/Create
        public async Task<ActionResult> Create()
        {
            var cust = new Customer
            {
                Status = "A"
            };

            //await ViewBagInfoForEdit(cust);
            return View(cust);
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Customer cust)
        {
            string validation = null;
            var props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance).ToList();

            if (!ModelState.IsValid)
                ModelState.AddModelError(string.Empty, "There are errors on this form.");
            else
                validation = cust.Validate(props);

            if (!ModelState.IsValid || validation != null)
            {
                if (validation != null)
                    ModelState.AddModelError(string.Empty, validation);

                //await ViewBagInfoForEdit(cust);
                return View(cust);
            }

            var addyLookup = await cust.UpdateQbAndGps(props);
            var gpsMessage = addyLookup == null ? null : addyLookup.ErrorMessage ?? addyLookup.LocationMessage;

            //db.Customers.Add(cust);
            //await db.SaveChangesAsync();

            //await cust.RecordChangeHistoryCustomerAdd(props);

            ///I don't see the point in updating EstNums nor inserting routes.
            ///Brand new customer will not have services.  Those are added separately later.

            //ddRoute.CheckInsertRoute(cust);
            //ddServiceDetail.UpdateEstNums(cust.CustomerId);

            return RedirectToAction("Details", new { id = cust.CustomerId, gpsMessage = gpsMessage });

        }

        // GET: Customers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var cust = await db.Customers.FindAsync(id);
            if (cust == null)
                return HttpNotFound();

            //await ViewBagInfoForEdit(cust);
            return View(cust);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Customer cust)
        {
            //Don't forget to check source and whyquit for value 0, null those out.

            string validation = null;
            List<PropertyInfo> changedProps = null;

            if (!ModelState.IsValid)
                ModelState.AddModelError(string.Empty, "There are errors on this form.");
            else
            {
                changedProps = cust.GetChanges();
                validation = cust.Validate(changedProps);
            }

            if (!ModelState.IsValid || validation != null)
            {
                if (validation != null)
                    ModelState.AddModelError(string.Empty, validation);

                //await ViewBagInfoForEdit(cust);
                return View(cust);
            }

            //All good.  Save changes, etc.
            var addyLookup = await cust.UpdateQbAndGps(changedProps);
            var gpsMessage = addyLookup == null ? null : addyLookup.ErrorMessage ?? addyLookup.LocationMessage;

            await cust.RecordChangeHistory(changedProps);

            db.Entry(cust).State = EntityState.Modified;
            await db.SaveChangesAsync();

            Route.CheckInsertRoute(cust);

            if (changedProps.Any(x => x.Name == "Status"))
                ServiceDetail.UpdateEstNums(cust.CustomerId);

            if (!string.IsNullOrWhiteSpace(cust.Flag) && Request.Form["cbFlagResend"] != null && Request.Form["cbFlagResend"] == "on") //should be checking a chckbox "flagResend" not in page yet)
                cust.SendFlagEmail();


            //if (changedProps.Any(x => x.Name == "Status") && cust.Status == "I")
            //    cust.SendEmail("OutProcess");

            //if (changedProps.Any(x => x.Name == "PauseDate" || x.Name == "ReStartDate"))
            //    cust.SendEmail("Pause");

            //This will delete any routes between pause and restart date as well as all due to Status = I 
            await Route.DeletePauseAndFinalRoutes(cust);

            return RedirectToAction("Details", new { id = cust.CustomerId, gpsMessage = gpsMessage });

        }

        // GET: Customers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            db.Customers.Remove(customer);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //private  async Task ViewBagInfoForEdit(Customer cust)
        //{
        //    ViewBag.FormattedMeet = cust.MeetSchDate.HasValue ? cust.MeetSchDate.Value.ToString("yyyy-MM-dd") : "";
        //    ViewBag.FormattedRoute = cust.RouteStartDate.HasValue ? cust.RouteStartDate.Value.ToString("yyyy-MM-dd") : "";
        //    ViewBag.FormattedPause = cust.PauseDate.HasValue ? cust.PauseDate.Value.ToString("yyyy-MM-dd") : "";
        //    ViewBag.FormattedRestart = cust.ReStartDate.HasValue ? cust.ReStartDate.Value.ToString("yyyy-MM-dd") : "";
        //    ViewBag.FormattedFinal = cust.FinalServiceDate.HasValue ? cust.FinalServiceDate.Value.ToString("yyyy-MM-dd") : "";

        //    var dropdowns = new List<ddDropDownValue>
        //    {
        //        new ddDropDownValue { DropDownValue = "", DropDownText = "", DropDownName = "ddWhyQuit" },
        //        new ddDropDownValue { DropDownValue = "", DropDownText = "", DropDownName = "ddHowFound" },
        //        new ddDropDownValue { DropDownValue = "", DropDownText = "", DropDownName = "ddPaymentMethod" },
        //        new ddDropDownValue { DropDownValue = "", DropDownText = "", DropDownName = "ddInvoiceMethod" }
        //    };
        //    dropdowns.AddRange(await db.ddDropDownValues.ToListAsync());

        //    var ddlWhyQuit = dropdowns.Where(x => x.DropDownName == "ddWhyQuit" || x.DropDownName == "blank");
        //    ViewBag.WhyQuit = new SelectList(dropdowns.Where(x => x.DropDownName == "ddWhyQuit"), "DropDownValueId", "DropDownText", cust.WhyQuit);
        //    ViewBag.HowFound = new SelectList(dropdowns.Where(x => x.DropDownName == "ddHowFound"), "DropDownValueId", "DropDownText", cust.HowFound ?? 0);
        //    ViewBag.PaymentMethod = new SelectList(dropdowns.Where(x => x.DropDownName == "ddPaymentMethod"), "DropDownValueId", "DropDownText", cust.PaymentMethod ?? 0);
        //    ViewBag.InvoiceMethod = new SelectList(dropdowns.Where(x => x.DropDownName == "ddInvoiceMethod"), "DropDownValueId", "DropDownText", cust.InvoiceMethod ?? 0);
        //    ViewBag.Status = new SelectList(Models.CustomerStatus.GetList(), "DbStatus", "Status", cust.Status);
        //    ViewBag.Type = new SelectList(Models.CustomerType.GetList(), "DbType", "CustType", cust.Type);
        //    ViewBag.Flag = new SelectList(Flag.GetFlags(), "Id", "Meaning", cust.Flag);

        //}

        private async Task<CustomerSummary> GetCustomerSummary(int? id)
        {
            var summary = new CustomerSummary();
            var customer = await db.Customers.FindAsync(id);

            if (customer == null)
                return null;

            var ServiceDetails = (from d in db.ServiceDetails
                                  join s in db.Services on d.ServiceId equals s.ServiceId
                                  join e in db.Employees on d.Crew equals e.Crew
                                  where d.CustomerId == id && e.Status != "I"
                                  orderby d.Day, d.Crew, s.DisplayOrder
                                  select new
                                  {
                                      ServiceName = s.Description,
                                      Day = d.Day,
                                      Crew = d.Crew,
                                      CrewName = e.Crew.Value.ToString().Trim() + " - " + e.FirstName,
                                      Price = d.Price,
                                      Discount = d.Discount,
                                      QtyPrice = d.QtyPrice,
                                      Qty = d.Qty,
                                      AddnlAmount = d.AdditAmount,
                                      PayPercent = e.PayPerc,
                                      NumDogs = d.NumDogs,
                                      InvoiceAmt = d.InvoiceAmount,
                                      EmpPayAdj = d.EmpPayAdj,
                                      LastService = d.LastServiceDate,
                                      ExcludeEmpPay = s.ExcludeEmpPay,
                                      QtyFlatPayAmount = s.QtyFlatPayAmt,
                                      FlatEmpPayAmount = s.FlatEmpPayAmt
                                  }).ToList();

            int lastDay = -1;
            int lastCrew = -1;

            summary.Services = new ServiceSummary();
            ServiceDaily service = null;

            foreach (var detail in ServiceDetails)
            {
                if (lastDay != detail.Day || lastCrew != detail.Crew)
                {
                    if (service != null)
                        summary.Services.DailyServices.Add(service);
                    service = new ServiceDaily();
                }
                lastDay = detail.Day;
                lastCrew = detail.Crew;
                service.Crew = detail.Crew;
                service.CrewName = detail.CrewName;
                service.Day = detail.Day;
                service.LastServiceDate = detail.LastService.GetValueOrDefault();
                service.NumDogs = detail.NumDogs.GetValueOrDefault();
                service.ServiceList.Add(detail.ServiceName);

                var qty = detail.Qty - 1;
                var itemTotal = detail.InvoiceAmt != 0
                    ? detail.InvoiceAmt
                    : detail.Price + (detail.QtyPrice * qty) + detail.AddnlAmount - detail.Discount;

                service.Total += itemTotal;

                service.EmpAmount += service.CalcEmpTotal(detail.ExcludeEmpPay, detail.QtyFlatPayAmount, detail.Qty, detail.FlatEmpPayAmount, detail.PayPercent, itemTotal);
            }
            if (ServiceDetails.Count() > 0)
                summary.Services.DailyServices.Add(service);

            summary.Customer = customer;



            ViewBag.Actions = new List<SelectListItem>
            {

                new SelectListItem { Value = string.Format("/CustomerTasks?CustomerId={0}", customer.CustomerId), Text = "Tasks" },
                new SelectListItem { Value = string.Format("/CustomerContactLog.aspx?CustID={0}", customer.CustomerId), Text = "New Contact" },
                new SelectListItem { Value = string.Format("/CustomerChangeLog.aspx?CustID={0}", customer.CustomerId), Text = "New Change" },
                new SelectListItem { Value = string.Format("/CustomerComplaintLog.aspx?CustID={0}", customer.CustomerId), Text = "Complaints" },
                new SelectListItem { Value = string.Format("/CustomerInvoiceListing.aspx?CustID={0}", customer.CustomerId), Text = "Invoice Detail" },
                new SelectListItem { Value = string.Format("/CustomerContactListing.aspx?CustID={0}", customer.CustomerId), Text = "Contact Log" },
                new SelectListItem { Value = string.Format("/CustomerHistoryListing.aspx?CustID={0}", customer.CustomerId), Text = "Change Log" },
                new SelectListItem { Value = string.Format("/CustomerServiceListing.aspx?CustID={0}", customer.CustomerId), Text = "Service History" },
                new SelectListItem { Value = string.Format("/QBAccountHistory.aspx?CustID={0}", customer.CustomerId), Text = "Account History" }
            };

            return summary;
        }


    }
}

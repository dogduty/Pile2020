using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Pile.db
{
    [MetadataType(typeof(MetaCustomer))]
    public partial class Customer
    {
        public void SaveChildObjects(pileEntities db)
        {
            foreach (var address in Addresses.Where(x => x.Id != 0))
                db.Entry(address).State = EntityState.Modified;
            foreach (var email in EmailAddresses.Where(x => x.Id != 0))
                db.Entry(email).State = EntityState.Modified;
            foreach (var note in Notes.Where(x => x.Id != 0))
                db.Entry(note).State = EntityState.Modified;
            foreach (var phone in Phones.Where(x => x.Id != 0))
                db.Entry(phone).State = EntityState.Modified;


            foreach (var address in Addresses.Where(x => x.Id == 0))
                db.Addresses.Add(address);
            foreach (var email in EmailAddresses.Where(x => x.Id == 0))
                db.EmailAddresses.Add(email);
            foreach (var note in Notes.Where(x => x.Id == 0))
                db.Notes.Add(note);
            foreach (var phone in Phones.Where(x => x.Id == 0))
                db.Phones.Add(phone);

            db.SaveChanges();
        }

        public string Validate()//List<PropertyInfo> changedProps)
        {
            //other validations are accomplished via the Model.IsValid calls typically in controllers.
            //check out the model in ddCustomerMeta that takes care of the ddCustomer class
            //
           
            if (this.MeetScheduled && !this.CustomerSetup)
                return "Customer Setup required before Meet Scheduled.";

            if (this.MeetPerformed && !this.CustomerSetup)
                return "Customer Setup required before Meet Performed.";

            if (this.MeetPerformed && !this.MeetScheduled)
                return "Meet Scheduled required before Meet Performed.";

            if (this.MeetScheduled && (this.MeetSchDate == null || string.IsNullOrWhiteSpace(this.MeetSchTime)))
                return "Meet Scheduled Checked, Meet Date and Time not set.";

            var siteAddress = this.Addresses.SingleOrDefault(x => x.AddressType == "Site");

            if (siteAddress == null)
                return "Site Address must be supplied";

            if (string.IsNullOrWhiteSpace(siteAddress.Address1) || string.IsNullOrWhiteSpace(siteAddress.City) || string.IsNullOrWhiteSpace(siteAddress.State) || string.IsNullOrWhiteSpace(siteAddress.Zip))
                return "Site Address must include Address, City, State, and Zip.";

            var mailing = this.Addresses.SingleOrDefault(x => x.AddressType == "Mailing");
            if (mailing != null && (string.IsNullOrWhiteSpace(mailing.Address1) || string.IsNullOrWhiteSpace(mailing.City) || string.IsNullOrWhiteSpace(mailing.State) || string.IsNullOrWhiteSpace(mailing.Zip)))
                return "Mailing Address must include Address, City, State, and Zip.";

            if (this.Phones == null || this.Phones.Count() == 0)
                return "At least one phone number must be supplied";

            bool atLeastOnePhone = false;
            foreach (var phone in this.Phones)
            {
                if (!string.IsNullOrWhiteSpace(phone.Number))
                {
                    atLeastOnePhone = true;
                    break;
                }
            }
            if (!atLeastOnePhone)
                return "At least one phone number must be supplied";

            var primaryEmail = this.EmailAddresses.FirstOrDefault(x => x.IsPrimary);
            if (primaryEmail == null || string.IsNullOrWhiteSpace(primaryEmail.Email))
                return "Must supply a primary email address.";

            var eValidator = new EmailAddressAttribute();
            foreach (var email in this.EmailAddresses)
            {
                if (string.IsNullOrWhiteSpace(email.Email))
                    continue;
                if (!eValidator.IsValid(email.Email))
                    return $"The supplied email address, {email.Email}, is not a valid email address";
            }



            //if (this.Status == "I" && !this.FinalServiceDate.HasValue)
            //    return "Customer inactive without Final Service Date Set.";

            //if (this.FinalServiceDate.HasValue && this.QuitReason == null )
            //    return "Why Quit not selected";

            //if (this.Status == "A" && this.RouteStartDate.Value < DateTime.Today && changedProps.Any(x => x.Name == "Status"))
            //    return "Reactivated customer cannot have start date in past.";

            //TODO: reactivate and fix the following validation
            //if ((this.Status == "A" && changedProps.Any(x => x.Name == "Status")) && (this.PauseDate.HasValue || this.ReStartDate.HasValue || this.FinalServiceDate.HasValue))
            //    return "Reactivated customer should not have Pause Date nor Restart Date nor Final Service Date.";

            return null;
        }

        public List<PropertyInfo> GetChanges()
        {
            using (var db = new pileEntities())
            {
                var old = db.Customers.Single(x => x.Id == Id);
                var changedProps = new List<PropertyInfo>();
                var props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
                foreach (var prop in props)
                {
                    if (prop.Name == "LastUpdate")
                        continue;

                    object oldVal = prop.GetValue(old);
                    object newVal = prop.GetValue(this);

                    if (prop.PropertyType == typeof(string))
                        if (string.IsNullOrEmpty(oldVal as string) && string.IsNullOrEmpty(oldVal as string))
                            continue;

                    if (oldVal == null && newVal == null)
                        continue;

                    if ((oldVal == null && newVal != null) || (oldVal != null && newVal == null) || (!prop.GetValue(this).Equals(prop.GetValue(old))))
                        changedProps.Add(prop);

                }
                return changedProps;
            }
        }



        //public async Task<AddressLookup> UpdateQbAndGps(List<PropertyInfo> changes)
        //{
        //    AddressLookup addyLookup = null;
        //    List<string> gpsTriggers = new List<string> { "Address", "City", "State", "Zip" };
        //    List<string> QbTriggers = new List<string>(new List<string> { "FirstName", "LastName", "MailAddress", "MailCity", "MailState", "MailZip", "Home", "Mobile", "WorkPhone", "OtherPhone", "Email", "PaymentMethod", "InvoiceMethod" });
        //    QbTriggers.AddRange(gpsTriggers);

        //    if (FinalServiceDate.HasValue)
        //        Status = "I";

        //    if (string.IsNullOrWhiteSpace(GPS) || changes.Any(x => gpsTriggers.Contains(x.Name)))
        //    {
        //        addyLookup = await GetGPS();
        //        if (!addyLookup.HasError)
        //            GPS = addyLookup.LatLng;
        //    }

        //    if (changes.Any(x => QbTriggers.Contains(x.Name)))
        //        LastUpdate = DateTime.Now;

        //    return addyLookup;
        //}

        public async Task RecordChangeHistoryCustomerAdd(List<PropertyInfo> changes)
        {
            //everything has changed.  Let's just grab the "status" to record new record
            await RecordChangeHistory(changes.Where(x => x.Name == "Status").ToList(), ChangeType.New);
        }

        public Pause NextPauseDate()
        {
            return this.Pauses.Where(x => x.RestartDate > DateTime.Now).OrderBy(x => x.PauseDate).FirstOrDefault();
        }

        public async Task RecordChangeHistory(List<PropertyInfo> changes, ChangeType changeType = ChangeType.Change)
        {
            using (var db = new pileEntities())
            {
                var old = db.Customers.Single(x => x.Id == Id);
                var histories = new List<CustomerHistory>();
                foreach (var change in changes)
                {
                    string oldval = (change.GetValue(old) ?? "<null>").ToString(); // : change.GetValue(old).ToString();
                    string newval = change.GetValue(this) == null ? "<null>" : change.GetValue(this).ToString();

                    var hist = new CustomerHistory
                    {
                        CustomerId = this.Id,
                        Date = DateTime.Now,
                        EmployeeId = ApplicationUserManager.GetCurrentUserEmployeeId(),
                        Field = change.Name,
                        Type = 33,
                        Notes = string.Format("Old value: {0} - New value: {1}", oldval, newval)
                    };


                    switch (changeType)
                    {
                        case ChangeType.Change:
                            hist.Type = change.GetValue(this) as string == "A" ? 32 : 31;
                            break;
                        case ChangeType.New:
                            hist.Type = 30;
                            break;
                    }

                    histories.Add(hist);
                }

                if (histories.Count() > 0)
                    db.CustomerHistories.AddRange(histories);

                await db.SaveChangesAsync();
            }
        }

        //TODO:  Re-enable GetGPS for a new or edited address .
        public async Task<AddressLookup> GetGPS()
        {
            var lookup = new AddressLookup();
            string address = this.MainAddress().Address1.Trim();

            int pos = address.IndexOf("#");
            if (pos > 0)
                address = address.Substring(0, pos - 1);

            var fullAddress = new string[] {
                HttpUtility.UrlEncode(address.Trim()),
                HttpUtility.UrlEncode(this.MainAddress().City.Trim()),
                HttpUtility.UrlEncode(this.MainAddress().State.Trim()),
                HttpUtility.UrlEncode(this.MainAddress().Zip.Trim())
            };

            var apiUri = string.Format("?key={0}&address={1}", Config.MapApiKey, string.Join(",", fullAddress));

            GoogleGeocoding geo = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Config.MapApiBaseUri);
                var resp = await client.GetAsync(apiUri);
                if (resp.IsSuccessStatusCode)
                    geo = await resp.Content.ReadAsAsync<GoogleGeocoding>();
            }

            if (geo == null || geo.results.Count != 1)
            {
                lookup.ErrorMessage = string.Format("Error:  {0} Results from Google.  Recheck entire address.", geo == null ? 0 : geo.results.Count);
                return lookup;
            }

            lookup.LatLng = string.Format("{0}, {1}", geo.results[0].geometry.location.lat, geo.results[0].geometry.location.lng);
            if (geo.results[0].geometry.location_type != "ROOFTOP")
                lookup.LocationMessage = string.Format("Google Map API results not exact.  Lookup result was {0}", geo.results[0].geometry.location_type);

            return lookup;
        }

        public void SendFlagEmail()
        {
            string emailTypeName = "Flag-Call";
            switch (Flag)
            {
                case "L":
                    emailTypeName = "Flag-Late";
                    break;
                case "F":
                    emailTypeName = "Flag-Final";
                    break;
            }

            SendEmail(emailTypeName);

        }

        public void SendEmail(string emailTypeName)
        {
            using (var email = new Email(emailTypeName, Id))
            {
                email.SendTargetedEmail();
            }
        }

        public Address MainAddress()
        {
            return this.Addresses.FirstOrDefault(x => x.AddressType == "Site");
        }

        public EmailAddress GetMainEmail()
        {
            return this.EmailAddresses.FirstOrDefault(x => x.IsPrimary);
    }

        public enum ChangeType
        {
            Change,
            New,
            Delete
        }

        //public async void DeleteRoutesDuringPausePeriod()
        //{
        //    if (PauseDate.HasValue && PauseDate.Value >= ddRoute.ThisWeekStart())
        //    {
        //        using (var db = new pileEntities())
        //        {
        //            var routes = db.ddRoutes.Where(x => x.CustomerID == CustomerId && x.Date >= DateTime.Today && x.Date >= PauseDate.Value
        //                                            && x.Date < (ReStartDate.HasValue ? ReStartDate.Value : DateTime.MaxValue) && x.LastServiceDate == null);
        //            db.ddRoutes.RemoveRange(routes);
        //            await db.SaveChangesAsync();
        //        }
        //    }
        //}
    }
}
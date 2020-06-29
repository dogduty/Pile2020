using Pile.db;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Pile
{
    public class Email : IDisposable
    {

        private pileEntities db = new pileEntities();
        private SmtpClient client;
        private MailMessage message;
        private IEnumerable<CompanyInfo> companyInfo;

        public EmailAccount From { get; set; }
        public Customer Customer { get; set; }
        public EmailTemplate Template { get; set; }
        public string OverrideTo { get; set; }

        //public static void SendCustomerEmail(string emailType, int customerId, string overrideTo = "chris.stodds@gmail.com")
        //{
        //    using (var email = new Email(emailType, customerId, overrideTo))
        //    {
        //        email.SendTargetedEmail();
        //    }
        //}

        public Email(string emailTypeName, int customerId, string overrideTo = "chris.stodds@gmail.com")
        {
            Customer = db.Customers.Find(customerId);
            var emailTypeId = db.EmailTypes.SingleOrDefault(x => x.Name == emailTypeName);
            if (emailTypeId == null)
                throw new Exception(string.Format("Email Type {0} not found in database.  Please fix.", emailTypeName));
            Template = db.EmailTemplates.OrderBy(x => x.Id).FirstOrDefault(x => x.EmailTypeId == emailTypeId.Id);
            if (Template == null)
                throw new Exception(string.Format("No email template found with type of {0}.  Please fix.", emailTypeName));
            From = Template.EmailAccount;
            OverrideTo = overrideTo;
            Init();
        }

        public Email(Customer customer, EmailTemplate emailTemplate, string overrideTo = "chris.stodds@gmail.com")
        {
            Customer = customer;
            Template = emailTemplate;
            From = emailTemplate.EmailAccount;
            OverrideTo = overrideTo;
            Init();
        }

        private void Init()
        {
            companyInfo = db.CompanyInfoes;
            InitSmtpClient();
            InitMailMessage();
        }

        public void SendTargetedEmail()
        {
            message.Body = ReplacePlaceholders();
            foreach (var email in Customer.EmailAddresses.Where(x => x.ServiceEmails))
                message.To.Add(email.Email);

            if (OverrideTo != null)
            {
                message.To.Clear();
                message.To.Add(OverrideTo);
            }

            client.Send(message);
        }

        private void InitSmtpClient()
        {
            client = new SmtpClient();
            client.EnableSsl = false;
            client.Host = From.EmailDomain.EmailServer.Server;
            client.Port = From.EmailDomain.EmailServer.Port;
            client.Credentials = new NetworkCredential { UserName = From.EmailAddress, Password = From.Password };
        }

        private void InitMailMessage()
        {
            message = new MailMessage();
            message.From = new MailAddress(From.EmailAddress, From.DisplayName);
            message.Subject = Template.Title;
            message.IsBodyHtml = true;
        }



        private string ReplacePlaceholders()
        {
            var result = Template.BodyTemplate;

            var placeHoler = GetNextPlaceholder(result, 0);
            while (placeHoler.Item1 != null)
            {
                result = result.Replace(placeHoler.Item1, GetPlaceholderValue(placeHoler.Item1));
                placeHoler = GetNextPlaceholder(result, placeHoler.Item2);
            }

            return result;
        }
        

        public static Tuple<string, int> GetNextPlaceholder(string email, int lastLocation)
        {
            if (string.IsNullOrEmpty(email))
                return new Tuple<string, int>(null, 0);

            int start = email.IndexOf("{{", lastLocation + 1);
            if (start == -1)
                return new Tuple<string, int>(null, 0);

            int len = email.IndexOf("}}", start) + 2 - start;

            return new Tuple<string, int>(email.Substring(start, len), start);
        }

        public string GetPlaceholderValue(string placeHolder)
        {
            var ph = placeHolder.ToLower().Replace("{{", "").Replace("}}", "").Split(new[] { '.' }, 3);
            object target = null;
            switch (ph[0])
            {
                case "customer":
                    target = Customer;
                    break;
                case "companyinfo":
                    var info = companyInfo.SingleOrDefault(x => x.Name.ToLower() == ph[1]);
                    if (info == null)
                        throw new Exception(string.Format("Unknown Company info type: {0}", ph[1]));
                    return info.Value.ToString();
                default:
                    throw new Exception(string.Format("Unknown placeholder {0}", placeHolder));
            }

            var prop = target.GetType().GetProperty(ph[1], BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (prop == null)
                throw new Exception(string.Format("Unknown property {0} for object {1}", ph[1], ph[0]));

            object value = null;
            value = prop.GetValue(target);
            if (value == null)
                return "";

            if (ph.Length > 2)
                return string.Format("{0:" + ph[2] + "}", value);

            return value.ToString();

        }





        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
                    client.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
using Microsoft.AspNet.Identity.EntityFramework;
using Pile.db;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Pile.Models
{
    public partial class ApplicationUser : IdentityUser
    {

        public async Task SendEmailConfirmation(UrlHelper url, ApplicationUserManager userManager)
        {
            //string code = await userManager.GenerateEmailConfirmationTokenAsync(this.Id);
            //var callbackUrl = url.Action("ConfirmEmail", "Account", new { userId = this.Id, code = code }, protocol: HttpContext.Current.Request.Url.Scheme);
            //await SendEmailConfirmation(callbackUrl, userManager);
            await SendEmailConfirmation(userManager);
        }

        public async Task SendEmailConfirmation(ApplicationUserManager userManager)
        {
            string code = await userManager.GenerateEmailConfirmationTokenAsync(this.Id);
            var callbackUrl = String.Format(Settings.GetValue("tokenEmailUrlTemplate", "URL not configured in pile.  {0} {1}"), this.Id, code); 
            await userManager.SendEmailAsync(this.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.  Or by pasting this in your browser's address bar: " + callbackUrl);
        }

        public int EmployeeId { get; set; }
    }
}
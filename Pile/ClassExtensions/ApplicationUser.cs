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

        public async Task SendEmailConfirmation(UrlHelper Url, ApplicationUserManager UserManager)
        {

            string code = await UserManager.GenerateEmailConfirmationTokenAsync(this.Id);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = this.Id, code = code }, protocol: HttpContext.Current.Request.Url.Scheme);
            await UserManager.SendEmailAsync(this.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>.  Or by pasting this in your browser's address bar: " + callbackUrl);
        }

        public int EmployeeId { get; set; }
    }
}
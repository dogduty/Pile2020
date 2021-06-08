using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Pile.Models;

namespace Pile
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {

        }

        public static ApplicationUserManager GetCurrent()
        {
            return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            manager.EmailService = new EmailService();


            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    TokenLifespan = System.TimeSpan.FromMinutes(60)
                };
            }
            return manager;
        }


        public static int GetCurrentUserEmployeeId()
        {
            return GetCurrentUser().EmployeeId;
        }

        public static ApplicationUser GetCurrentUser()
        {
            var manager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            return manager.FindById(HttpContext.Current.User.Identity.GetUserId());
        }

        public static async Task<List<ApplicationUser>> GetApplicationUsers()
        {

            var context = ApplicationDbContext.Create();
            
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            return await manager.Users.ToListAsync();
        }

        public static async Task DeleteApplicationUserAsync(ApplicationUser user)
        {
            var context = ApplicationDbContext.Create();
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            await manager.DeleteAsync(user);
        }

        public static async Task<ApplicationUser> GetApplicationUserAsync(int employeeId)
        {

            var context = ApplicationDbContext.Create();
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            return await manager.Users.Where(x => x.EmployeeId == employeeId).SingleOrDefaultAsync();
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager, DefaultAuthenticationTypes.ApplicationCookie);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }

    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            ///It is based on the same context as the ApplicationUserManager
            var appRoleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));

            return appRoleManager;
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.

            var from = new MailAddress(Settings.GetValue("emailFrom"), Settings.GetValue("emailFromDisplay")); 
            var to = new MailAddress(message.Destination);

            var creds = new NetworkCredential(from.Address, Settings.GetValue("emailPassword"));
            var smtp = new SmtpClient
            {
                Host = Settings.GetValue("smtpHost"),
                Port = int.Parse(Settings.GetValue("smtpPort")),
                EnableSsl = bool.Parse(Settings.GetValue("smtp.EnableSsl")),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = creds, 
            };


            using (var email = new MailMessage(from, to) { Subject = message.Subject, Body = message.Body, IsBodyHtml = true })
            {
                smtp.Send(email);
            }

            return Task.FromResult(0);
        }
    }

}

using Pile.db;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Pile.Controllers
{
    [RoutePrefix("api/EmailAddresses")]
    public class ApiEmailAddressesController : ApiController
    {
        private pileEntities db = new pileEntities();

        [Route("{id}")]
        [ResponseType(typeof(Note))]
        public async Task<IHttpActionResult> DeleteNote(long id)
        {
            EmailAddress emailAddress = await db.EmailAddresses.FindAsync(id);
            if (emailAddress == null)
            {
                return NotFound();
            }

            if (!(await db.EmailAddresses.AnyAsync(x => x.Id != id && x.CustomerId == emailAddress.CustomerId)))
                return BadRequest("Must have at least one email address");

            db.EmailAddresses.Remove(emailAddress);
            await db.SaveChangesAsync();

            return Ok(emailAddress);
        }


        [Route("new")]
        [HttpGet]
        public IHttpActionResult New([FromUri] bool isPrimary)
        {
            return Ok(new EmailAddress { IsPrimary = false, ServiceEmails = true });
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

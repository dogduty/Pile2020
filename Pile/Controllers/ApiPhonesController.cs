using Pile.db;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;


namespace Pile.Controllers
{
    [RoutePrefix("api/Phones")]
    public class ApiPhonesController : ApiController
    {
        private pileEntities db = new pileEntities();

        [Route("{id}")]
        [ResponseType(typeof(Note))]
        public async Task<IHttpActionResult> DeletePhone(long id)
        {
            Phone phone = await db.Phones.FindAsync(id);
            if (phone == null)
            {
                return NotFound();
            }

            if (!(await db.Phones.AnyAsync(x => x.Id != id && x.CustomerId == phone.CustomerId)))
                return BadRequest("Must have at least one phone number.");

            db.Phones.Remove(phone);
            await db.SaveChangesAsync();

            return Ok(phone);
        }


        [Route("new/{phoneType}")]
        [HttpGet]
        public IHttpActionResult New(string phoneType)
        {
            return Ok(new Phone { PhoneType = phoneType });
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

using Pile.db;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Pile.Controllers
{
    [RoutePrefix("api/Addresses")]
    public class ApiAddressesController : ApiController
    {
        private pileEntities db = new pileEntities();

        [Route("new/{addressType}")]
        [HttpGet]
        public IHttpActionResult New(string addressType)
        {
            return Ok(new Address { AddressType = addressType });
        }

        [Route("{id}")]
        [ResponseType(typeof(Note))]
        public async Task<IHttpActionResult> DeleteAddress(long id)
        {
            Address address = await db.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            db.Addresses.Remove(address);
            await db.SaveChangesAsync();

            return Ok(address);
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

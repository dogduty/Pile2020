using Pile.db;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Pile.Controllers
{
    

    [RoutePrefix("api/Notes")]
    public class ApiNotesController : ApiController
    {
        private pileEntities db = new pileEntities();

        [Route("{id}")]
        [ResponseType(typeof(Note))]
        public async Task<IHttpActionResult> DeleteNote(long id)
        {
            Note note = await db.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            db.Notes.Remove(note);
            await db.SaveChangesAsync();

            return Ok(note);
        }

        [Route("new")]
        [HttpGet]
        public IHttpActionResult New()
        {
            return Ok(new Note { NoteTime = DateTime.Now });
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

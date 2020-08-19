using Pile.db;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


namespace Pile.Controllers
{

    [RoutePrefix("api/HowFounds")]
    public class ApiHowFoundsController : ApiController
    {
        [Route("new")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            using (pileEntities db = new pileEntities())
            {
                var howFounds = await db.HowFounds.Select(x => new { Id = x.Id, Name = x.Name }).ToListAsync();
                return Request.CreateResponse(HttpStatusCode.OK, howFounds);
            }
        }
    }

    [RoutePrefix("api/PaymentMethods")]
    public class ApiPaymentMethodsController : ApiController
    {
        [Route("new")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            using (pileEntities db = new pileEntities())
            {
                var payMehods = await db.PaymentMethods.Select(x => new { Id = x.Id, Name = x.Name }).ToListAsync();
                return Request.CreateResponse(HttpStatusCode.OK, payMehods);
            }
        }
    }

    [RoutePrefix("api/InvoiceMethods")]
    public class ApiInvoiceMethodsController : ApiController
    {
        [Route("new")]
        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            using (pileEntities db = new pileEntities())
            {
                var invMethods = await db.InvoiceMethods.Select(x => new { Id = x.Id, Name = x.Name }).ToListAsync();
                return Request.CreateResponse(HttpStatusCode.OK, invMethods);
            }
        }
    }


}

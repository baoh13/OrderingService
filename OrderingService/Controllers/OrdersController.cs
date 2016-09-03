using System;
using System.Web.Http;

namespace OrderingService.Controllers
{
    public class OrdersController : ApiController
    {
        public IHttpActionResult Get()
        {
            try
            {

                return Ok();
            }
            catch (Exception)
            {
                
                return InternalServerError();
            }
        }
    }
}

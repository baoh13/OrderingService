using OrderingService.Models;
using System;
using System.Web.Http;

namespace OrderingService.Controllers
{
    public class OrdersController : ApiController
    {
        private OrdersContext _context;

        public OrdersController()
        {
            _context = new OrdersContext();
        }

        public IHttpActionResult Get()
        {
            try
            {
                var product = new Product
                {
                    Id = 1,
                    Name = "CocaCola",
                    Quantity = 5
                };
                return Ok(product);
            }
            catch (Exception)
            {
                
                return InternalServerError();
            }
        }
    }
}

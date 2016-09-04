using OrderingService.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace OrderingService.Controllers
{
    public class OrdersController : ApiController
    {
        private readonly OrdersRepository _ordersRepository = new OrdersRepository();

        public IHttpActionResult Get()
        {
            try
            {
                return Ok(_ordersRepository.GetProducts().ToList());
            }
            catch (Exception)
            {
                // Log the error.
                return InternalServerError();
            }
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody]Product product)
        {
            try
            {
                if (product.Quantity < 0)
                    return BadRequest($"Invalid quantity: {product.Quantity}");

                _ordersRepository.Add(product);

                var url = Request.RequestUri + "/" + product.Name;

                return Created(url, content: product);
            }
            catch (Exception)
            {
                //Log the error
                return InternalServerError();
            }
        }
    }
}

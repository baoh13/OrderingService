using OrderingService.Models;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace OrderingService.Controllers
{
    public class OrdersController : ApiController
    {
        private IOrdersRepository _ordersRepository = new OrdersRepository();

        // In real applications. Dependency injection is used instead. 
        public IOrdersRepository OrdersRepository
        {
            get { return _ordersRepository; }
            set { _ordersRepository = value; }
        }

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

        [HttpPut]
        public IHttpActionResult Put([FromBody] Product product)
        {
            try
            {
                var requestedProduct = _ordersRepository.GetProduct(product.Name);
                if (requestedProduct == null)
                    return NotFound();

                requestedProduct = _ordersRepository.Update(product);

                return Ok(requestedProduct);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(string name)
        {
            try
            {
                var requestedProduct = _ordersRepository.GetProduct(name);
                if (requestedProduct == null)
                    return NotFound();

                _ordersRepository.Remove(requestedProduct);

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}

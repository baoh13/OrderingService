using OrderingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace OrderingService.Controllers
{
    public class OrdersController : ApiController
    {
        public static IList<Product> ProductList = new List<Product>();

        public IHttpActionResult Get()
        {
            try
            {
                return Ok(ProductList.ToList());
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

                ProductList.Add(product);

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

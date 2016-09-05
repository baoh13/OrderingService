using OrderingService.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace OrderingService.Controllers
{
    [Authorize]
    public class OrdersController : ApiController
    {
        private IOrdersRepository _ordersRepository = new OrdersRepository();
        private const int maxPageSize = 10;
        
        [HttpGet]
        [Route("api/orders", Name = "OrdersList")]
        public IHttpActionResult Get(int page = 1, int pageSize = maxPageSize)
        {
            try
            {
                var products = _ordersRepository.GetProducts().ToList();

                // ensure the page size isn't larger than the maximum.
                if (pageSize > maxPageSize)
                {
                    pageSize = maxPageSize;
                }

                var totalCount = products.Count();

                var response = Request.CreateResponse(HttpStatusCode.OK,
                                                      products.Skip(pageSize * (page - 1))
                                                              .Take(pageSize)
                                                              .ToList());
                
                response.Headers.Add("X-Pagination", CreatePaginationHeader(Request, totalCount, pageSize, page));

                return ResponseMessage(response);
            }
            catch (Exception)
            {
                // Log the error.
                return InternalServerError();
            }
        }

        
        [HttpGet]
        [Route("api/orders/{name}")]
        public IHttpActionResult Get(string name)
        {
            try
            {
                var requestedProduct = _ordersRepository.GetProduct(name);
                if (requestedProduct == null)
                    return NotFound();

                return Ok(requestedProduct);
            }
            catch (Exception)
            {
                // Log the error.
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("api/orders")]
        public IHttpActionResult Post([FromBody]Product product)
        {
            try
            {
                if (product.Quantity < 0)
                    return BadRequest($"Invalid quantity: {product.Quantity}");

                var createdProduct = _ordersRepository.Add(product);

                var url = Request.RequestUri + "/" + product.Name;

                return Created(url, content: createdProduct);
            }
            catch (Exception)
            {
                //Log the error
                return InternalServerError();
            }
        }

        [HttpPut]
        [Route("api/orders")]
        public IHttpActionResult Put([FromBody] Product product)
        {
            try
            {
                if (product.Quantity < 0)
                    return BadRequest(string.Format("Invalid quantity {0}", product.Quantity));

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
        [Route("api/orders/{name}")]
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

        private static string CreatePaginationHeader(
            HttpRequestMessage request, int totalProducts, int pageSize, int page)
        {
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            var urlHelper = new UrlHelper(request);
            var prevLink = page > 1 ? urlHelper.Link("OrdersList",
                new
                {
                    page = page - 1,
                    pageSize = pageSize
                }) : "";
            var nextLink = page < totalPages ? urlHelper.Link("OrdersList",
                new
                {
                    page = page + 1,
                    pageSize = pageSize
                }) : "";

            var paginationHeader = new
            {
                currentPage = page,
                pageSize = pageSize,
                totalCount = totalProducts,
                totalPages = totalPages,
                previousPageLink = prevLink,
                nextPageLink = nextLink
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader);
        }

    }
}

using OrderingService.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace OrderingService.Controllers
{
    public class OrdersController : ApiController
    {
        private IOrdersRepository _ordersRepository = new OrdersRepository();
        private const int maxPageSize = 10;

        // In real applications. Dependency injection is used instead. 
        public IOrdersRepository OrdersRepository
        {
            get { return _ordersRepository; }
            set { _ordersRepository = value; }
        }

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

                // calculate data for metadata
                var totalCount = products.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var urlHelper = new UrlHelper(Request);
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
                    totalCount = totalCount,
                    totalPages = totalPages,
                    previousPageLink = prevLink,
                    nextPageLink = nextLink
                };


                var response = Request.CreateResponse(HttpStatusCode.OK,
                                                      products.Skip(pageSize * (page - 1))
                                                              .Take(pageSize)
                                                              .ToList());
                
                response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

                return ResponseMessage(response);
            }
            catch (Exception)
            {
                // Log the error.
                return InternalServerError();
            }
        }

        // Changed the default rout to "api/orders/{name}"
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
        
        // Changed the default rout to "api/orders/{name}"
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

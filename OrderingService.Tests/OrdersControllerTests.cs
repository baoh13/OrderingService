using NUnit.Framework;
using OrderingService.Controllers;
using OrderingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace OrderingService.Tests
{
    [TestFixture]
    public class OrdersControllerTests
    {
        [Test]
        public void OrdersController_Get_ReturnsAListOfOrderedProductsWithDetails()
        {
            var request = CreateRequest(HttpMethod.Post);

            var ordersController = CreateOrdersController(request);

            var product = new Product
            {
                Quantity = 3,
                Name = "Coca"
            };
            OrdersController.ProductList = new List<Product> { product };

            var result = ordersController.Get() as OkNegotiatedContentResult<List<Product>>;

            Assert.IsNotNull(result);
            Assert.AreEqual(product.Quantity, result.Content.First().Quantity);
            Assert.AreEqual(product.Name, result.Content.First().Name);
        }
        
        [Test]
        public void OrdersController_Get_ReturnsAListOfOrderedProducts()
        {
            var request = CreateRequest(HttpMethod.Get);

            var ordersController = CreateOrdersController(request);
            OrdersController.ProductList = new List<Product>
            {
                new Product
                {
                    Quantity = 3,
                    Name = "Coca"
                },
                new Product
                {
                    Name = "Pepsi",
                    Quantity = 4
                }
            };

            var result = ordersController.Get() as OkNegotiatedContentResult<List<Product>>;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Content.Count);
        }

        [Test]
        public void OrdersController_Post_RetursTheCreatedProduct()
        {
            var request = CreateRequest(HttpMethod.Post);

            var controller = CreateOrdersController(request: request);

            var product = new Product
            {
                Quantity = 3,
                Name = "Pepsi"
            };

            var result = controller.Post(product) as CreatedNegotiatedContentResult<Product>;

            Assert.IsNotNull(result);
            Assert.AreEqual("http://localhost/api/orders/Pepsi", result.Location.ToString());
            Assert.AreEqual(product.Name, result.Content.Name);
            Assert.AreEqual(product.Quantity, result.Content.Quantity);
        }

        [Test]
        public void OrdersController_Post_BadRequest()
        {
            var request = CreateRequest(HttpMethod.Post);

            var controller = CreateOrdersController(request: request);

            var product = new Product
            {
                Quantity = -4,
                Name = "Pepsi"
            };

            var result = controller.Post(product) as BadRequestErrorMessageResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Invalid quantity: -4", result.Message);
        }
        

        private OrdersController CreateOrdersController(HttpRequestMessage request)
        {
            var ordersController = new OrdersController
            {
                Configuration = new HttpConfiguration(),
                Request = request
            };
            
            return ordersController;
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, Uri uri = null)
        {
            return new HttpRequestMessage
            {
                Method = method,
                RequestUri = uri ?? new Uri("http://localhost/api/orders")
            };
        }
    }
}
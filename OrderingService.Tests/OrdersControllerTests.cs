﻿using NUnit.Framework;
using OrderingService.Controllers;
using OrderingService.Models;
using OrderingService.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace OrderingService.Tests
{
    [TestFixture]
    public class OrdersControllerTests
    {
        [Test]
        public void OrdersController_Put_ReturnsNotFound()
        {
            var ordersController = CreateOrdersController(CreateRequest(HttpMethod.Put));

            var product = new Product
            {
                Quantity = 3,
                Name = "Coca"
            };

            OrdersRepository.OrderList = new List<Product>();

            var result = ordersController.Put(product) as NotFoundResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void OrdersController_Delete_ReturnsNotFound()
        {
            var ordersController = CreateOrdersController(CreateRequest(HttpMethod.Delete));

            var product = new Product
            {
                Quantity = 3,
                Name = "Coca"
            };

            OrdersRepository.OrderList = new List<Product>();

            var result = ordersController.Delete(product.Name) as NotFoundResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void OrdersController_Delete_ReturnsNoContentResult()
        {
            var ordersController = CreateOrdersController(CreateRequest(HttpMethod.Delete));

            var product = new Product
            {
                Quantity = 3,
                Name = "Coca"
            };

            OrdersRepository.OrderList = new List<Product>
            {
                product
            };

            var result = ordersController.Delete(product.Name) as StatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public void OrdersController_Put_ReturnsBadRequest()
        {
            var ordersController = CreateOrdersController(CreateRequest(HttpMethod.Put));

            var product = new Product
            {
                Quantity = 3,
                Name = "Coca"
            };

            OrdersRepository.OrderList = new List<Product>
            {
                product
            };

            var result = ordersController.Put(new Product
            {
                Quantity = -5,
                Name = "Coca"
            }) as BadRequestErrorMessageResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void OrdersController_Put_ReturnsUpdatedProduct()
        {
            var ordersController = CreateOrdersController(CreateRequest(HttpMethod.Put));

            var product = new Product
            {
                Quantity = 3,
                Name = "Coca"
            };

            OrdersRepository.OrderList = new List<Product>
            {
                product
            };

            var result = ordersController.Put(new Product
            {
                Quantity = 5,
                Name = "Coca"
            }) as OkNegotiatedContentResult<Product>;

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Content.Quantity);
            Assert.AreEqual("Coca", result.Content.Name);
        }

        [Test]
        public void OrdersController_Get_ReturnsTheSearchedProduct()
        {
            var ordersController = CreateOrdersController(CreateRequest(HttpMethod.Get));

            var product = new Product
            {
                Quantity = 3,
                Name = "Coca"
            };

            OrdersRepository.OrderList = new List<Product> { product };

            var result = ordersController.Get("Coca") as OkNegotiatedContentResult<Product>;

            Assert.IsNotNull(result);
            Assert.AreEqual(product.Quantity, result.Content.Quantity);
            Assert.AreEqual(product.Name, result.Content.Name);
        }

        [Test]
        public void OrdersController_Get_ReturnsNotFound()
        {
            var ordersController = CreateOrdersController(CreateRequest(HttpMethod.Get));

            var product = new Product
            {
                Quantity = 3,
                Name = "Coca"
            };

            OrdersRepository.OrderList = new List<Product> { product };

            var result = ordersController.Get("CocaNotFound") as NotFoundResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void OrdersController_Get_ReturnsAListOfOrderedProductsWithDetails()
        {
            var request = CreateRequest(HttpMethod.Get);
            
            var ordersController = CreateOrdersController(request);
            var product = new Product
            {
                Quantity = 3,
                Name = "Coca"
            };

            OrdersRepository.OrderList = new List<Product> { product };

            
            var result = ordersController.Get() as ResponseMessageResult;

            Assert.IsNotNull(result);
            var products = (result.Response.Content as ObjectContent).Value as List<Product>;

            Assert.IsNotNull(products);
            Assert.AreEqual(product.Quantity, products.First().Quantity);
            Assert.AreEqual(product.Name, products.First().Name);
        }
        
        [Test]
        public void OrdersController_Get_ReturnsAListOfOrderedProducts()
        {
            var request = CreateRequest(HttpMethod.Get);

            var ordersController = CreateOrdersController(request);
            

            OrdersRepository.OrderList = new List<Product>
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

            var result = ordersController.Get() as ResponseMessageResult;

            Assert.IsNotNull(result);
            var products = (result.Response.Content as ObjectContent).Value as List<Product>;

            Assert.IsNotNull(result);
            Assert.AreEqual(2, products.Count);
        }

        [Test]
        public void OrdersController_Post_RetursTheCreatedProduct()
        {
            var request = CreateRequest(HttpMethod.Post);

            var controller = CreateOrdersController(request: request);

            var product = new Product
            {
                Quantity = 3,
                Name = "Pepsi1"
            };

            var result = controller.Post(product) as CreatedNegotiatedContentResult<Product>;

            Assert.IsNotNull(result);
            Assert.AreEqual("http://localhost/api/orders/Pepsi1", result.Location.ToString());
            Assert.AreEqual(product.Name, result.Content.Name);
            Assert.AreEqual(product.Quantity, result.Content.Quantity);
        }

        [Test]
        public void OrdersController_Post_MultipleTimesOfTheSameProduct()
        {
            var request = CreateRequest(HttpMethod.Post);

            var controller = CreateOrdersController(request: request);

            var product = new Product
            {
                Quantity = 3,
                Name = "Pepsi"
            };

            controller.Post(product);

            var result = controller.Post(new Product
            {
                Name = "Pepsi",
                Quantity = 5
            }) as CreatedNegotiatedContentResult<Product>;

            Assert.IsNotNull(result);
            Assert.AreEqual("http://localhost/api/orders/Pepsi", result.Location.ToString());
            Assert.AreEqual(product.Name, result.Content.Name);
            Assert.AreEqual(8, result.Content.Quantity);
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
                Request = request,
                
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
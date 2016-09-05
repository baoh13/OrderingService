using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderingService.Models
{
    public class OrdersRepository : IOrdersRepository
    {
        public static IList<Product> OrderList = new List<Product>();
        
        public IEnumerable<Product> GetProducts()
        {
            return OrderList;
        }

        public Product GetProduct(string name)
        {
            return OrderList.SingleOrDefault(p => p.Name == name);
        }

        public Product Update(Product product)
        {
            var requestedProduct = GetProduct(product.Name);

            if (requestedProduct == null)
                throw new ArgumentException($"Product {product.Name} does not exist.");

            
            requestedProduct.Quantity = product.Quantity;

            return requestedProduct;
        }

        public void Remove(Product requestedProduct)
        {
            OrderList.Remove(requestedProduct);
        }

        public Product Add(Product product)
        {
            var requestedProduct = GetProduct(product.Name);

            // Todo scenario quantity is negative
            if (requestedProduct != null)
            {
                requestedProduct.Quantity += product.Quantity;
            }
            else
            {
                requestedProduct = product;
                OrderList.Add(requestedProduct);
            }

            return requestedProduct;
        }
    }
}
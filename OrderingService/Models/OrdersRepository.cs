using System.Collections.Generic;

namespace OrderingService.Models
{
    public class OrdersRepository
    {
        public static IList<Product> OrderList = new List<Product>();
        
        public IEnumerable<Product> GetProducts()
        {
            return OrderList;
        } 

        public Product Add(Product product)
        {
            OrderList.Add(product);

            return product;
        }
    }
}
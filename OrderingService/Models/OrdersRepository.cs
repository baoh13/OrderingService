using System.Collections.Generic;
using System.Linq;

namespace OrderingService.Models
{
    public class OrdersRepository
    {
        private IList<Product> _orderList;

        public OrdersRepository()
        {
            _orderList = new List<Product>();
        }

        public IList<Product> Products
        {
            get { return _orderList; }
            set { _orderList = value; }
        }

        public IEnumerable<Product> GetProducts()
        {
            return _orderList.ToList();
        } 

        public Product Add(string drink, int quantity)
        {
            var product = new Product
            {
                Quantity = quantity,
                Name = drink
            };

            _orderList.Add(product);

            return product;
        }
    }
}
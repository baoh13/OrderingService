using System.Collections.Generic;

namespace OrderingService.Models
{
    public class OrdersContext
    {
        private IList<Product> _orderList;

        public OrdersContext()
        {
            _orderList = new List<Product>();
        }

        public IList<Product> OrderList { get; set; }
    }
}
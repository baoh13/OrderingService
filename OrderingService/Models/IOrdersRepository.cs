using System.Collections.Generic;

namespace OrderingService.Models
{
    public interface IOrdersRepository
    {
        Product Add(Product product);
        IEnumerable<Product> GetProducts();
    }
}
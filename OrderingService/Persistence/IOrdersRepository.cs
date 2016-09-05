using System.Collections.Generic;
using OrderingService.Models;

namespace OrderingService.Persistence
{
    public interface IOrdersRepository
    {
        Product Add(Product product);
        IEnumerable<Product> GetProducts();
        Product GetProduct(string name);
        Product Update(Product product);
        void Remove(Product requestedProduct);
    }
}
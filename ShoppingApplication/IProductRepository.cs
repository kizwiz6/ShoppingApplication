using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingApplication
{
    public interface IProductRepository
    {
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void RemoveProduct(string id);
        Product GetProductById(string id);
        IEnumerable<Product> GetAllProducts();
        bool ProductExists(string id);
    }
}

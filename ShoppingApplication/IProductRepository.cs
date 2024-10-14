using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingApplication
{
    public interface IProductRepository
    {
        void AddProduct(Product product); // Adds a new product
        void UpdateProduct(Product product); // Updates an existing product
        void RemoveProduct(string id); // Removes a product by its ID
        Product GetProductById(string id); // Retrieves a product by its ID
        IEnumerable<Product> GetAllProducts(); // Retrieves all products
        bool ProductExists(string id); // Checks if a product exists by its ID
    }
}

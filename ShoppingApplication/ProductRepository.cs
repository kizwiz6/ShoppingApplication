using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingApplication
{
    internal class ProductRepository : IProductRepository
    {
        // Dictionary to store products by ID
        private static Dictionary<string, Product> productCatalog = new Dictionary<string, Product>();

        public void AddProduct(Product product)
        {
            productCatalog[product.Id] = product; // Adds or updates product
        }

        public void UpdateProduct(Product product)
        {
            if (productCatalog.ContainsKey(product.Id))
            {
                productCatalog[product.Id] = product;
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return productCatalog.Values;
        }

        public Product GetProductById(string id)
        {
            productCatalog.TryGetValue(id, out var product);
            return product;
        }

        public bool ProductExists(string id)
        {
            return productCatalog.ContainsKey(id);
        }

        public void RemoveProduct(string id)
        {
            productCatalog.Remove(id);
        }
    }
}

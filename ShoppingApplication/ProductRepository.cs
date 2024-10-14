using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShoppingApplication
{
    internal class ProductRepository : IProductRepository
    {
        private const string FilePath = "products.json";

        // Dictionary to store products by ID
        private static Dictionary<string, Product> productCatalog = new Dictionary<string, Product>();

        public ProductRepository()
        {
            LoadProducts();
        }

        public void AddProduct(Product product)
        {
            productCatalog[product.Id] = product; // Adds or updates product
            SaveProducts();
        }

        public void UpdateProduct(Product product)
        {
            if (productCatalog.ContainsKey(product.Id))
            {
                productCatalog[product.Id] = product;
                SaveProducts();
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
            if (productCatalog.Remove(id))
            {
                SaveProducts();
            }
        }

        private void LoadProducts()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var jsonData = File.ReadAllText(FilePath);
                    var products = JsonConvert.DeserializeObject<List<Product>>(jsonData);

                    if (products != null)
                    {
                        foreach (var product in products)
                        {
                            productCatalog[product.Id] = product;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error or handle it appropriately
                Console.WriteLine($"Error loading products: {ex.Message}");
                throw;
            }

        }

        private void SaveProducts()
        {
            var jsonData = JsonConvert.SerializeObject(productCatalog.Values, Formatting.Indented);
            File.WriteAllText(FilePath, jsonData);
        }
    }
}

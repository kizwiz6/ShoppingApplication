using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ShoppingApplication
{
    /// <summary>
    /// Represents a repository for managing products, including their reviews.
    /// Provides methods to add, update, retrieve, and remove products from a JSON file.
    /// </summary>
    internal class ProductRepository : IProductRepository
    {
        private const string FilePath = "products.json";

        // Dictionary to store products by ID
        private static Dictionary<string, Product> productCatalog = new Dictionary<string, Product>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRepository"/> class.
        /// Loads products from a JSON file on initialization.
        /// </summary>
        public ProductRepository()
        {
            LoadProducts();
        }

        /// <summary>
        /// Adds a new product or updates an existing product in the repository.
        /// Saves changes to the JSON file.
        /// </summary>
        /// <param name="product">The product to add or update.</param>
        public void AddProduct(Product product)
        {
            productCatalog[product.Id] = product; // Adds or updates product
            SaveProducts(); // Save changes to file
        }

        /// <summary>
        /// Updates an existing product in the repository.
        /// Saves changes to the JSON file.
        /// </summary>
        /// <param name="product">The product to update.</param>
        public void UpdateProduct(Product product)
        {
            if (productCatalog.ContainsKey(product.Id))
            {
                productCatalog[product.Id] = product;
                SaveProducts(); // Save changes to file
            }
        }

        /// <summary>
        /// Gets all products in the repository.
        /// </summary>
        /// <returns>An enumerable collection of all products.</returns>
        public IEnumerable<Product> GetAllProducts()
        {
            return productCatalog.Values;
        }

        /// <summary>
        /// Retrieves a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        /// <returns>The product if found; otherwise, null.</returns>
        public Product GetProductById(string id)
        {
            productCatalog.TryGetValue(id, out var product);
            return product;
        }

        /// <summary>
        /// Checks if a product exists in the repository.
        /// </summary>
        /// <param name="id">The ID of the product to check.</param>
        /// <returns>true if the product exists; otherwise, false.</returns>
        public bool ProductExists(string id)
        {
            return productCatalog.ContainsKey(id);
        }

        /// <summary>
        /// Removes a product from the repository by its ID.
        /// Saves changes to the JSON file.
        /// </summary>
        /// <param name="id">The ID of the product to remove.</param>
        public void RemoveProduct(string id)
        {
            if (productCatalog.Remove(id))
            {
                SaveProducts(); // Save changes to file
            }
        }

        /// <summary>
        /// Loads products from the JSON file into the repository.
        /// </summary>
        private void LoadProducts()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var jsonData = File.ReadAllText(FilePath);
                    // Deserialize the product catalog as a Dictionary instead of a List
                    productCatalog = JsonConvert.DeserializeObject<Dictionary<string, Product>>(jsonData)
                        ?? new Dictionary<string, Product>();
                }
            }
            catch (Exception ex)
            {
                // Log error or handle it appropriately
                Console.WriteLine($"Error loading products: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Saves the current product catalog to the JSON file.
        /// </summary>
        private void SaveProducts()
        {
            // Serialize the product catalog as a Dictionary to include reviews
            var jsonData = JsonConvert.SerializeObject(productCatalog, Formatting.Indented);
            File.WriteAllText(FilePath, jsonData);
        }
    }
}

using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingApplication
{
    public class ProductManager : IProductManager
    {
        private readonly IProductRepository productRepository;
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public ProductManager(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        /// <summary>
        /// Adds a new product to the product catalog.
        /// Ensures input validation for Product ID, Product Name, Product Price, and Product Description.
        /// Allows user to return to the main menu by typing 'back'.
        /// </summary>
        public void AddProduct()
        {
            try
            {
                logger.Info("Attempting to add a new product.");  // Log attempt to add a new product
                Console.WriteLine("\n=== Add a New Product ===");

                string newId = GetProductId();
                if (newId == null) return; // User chose to go back to the main menu.

                string newName = GetProductName();
                decimal newPrice = GetProductPrice();
                string newDescription = GetProductDescription();
                string newCategory;
                do
                {
                    Console.Write("Enter Product Category:");
                    newCategory = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(newCategory))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Product Category cannot be empty.");
                        Console.ResetColor();
                    }
                }
                while (string.IsNullOrWhiteSpace(newCategory));

                // Create and add the new product to the repository
                var newProduct = new Product(newId, newName, newPrice, newDescription, newCategory);
                productRepository.AddProduct(newProduct);

                // Confirmation message
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Product added successfully!");
                Console.ResetColor();

                logger.Info($"Product added: ID={newId}, Name={newName}, Price={newPrice}, Description={newDescription}");  // Log successful addition
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.ResetColor();
                logger.Error(ex, "Error occurred while adding a product.");  // Log error
            }
        }


        public void UpdateProduct()
        {
            Console.WriteLine("\n=== Update an Existing Product ===");
            Console.Write("Enter Product ID to update: ");
            string updateId = Console.ReadLine();

            var product = productRepository.GetProductById(updateId);
            if (product == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Product with ID {updateId} not found.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"Current Product: {product.Name} - ${product.Price} ({product.Description})");

            Console.Write("Enter new Product Name: ");
            string updatedName = Console.ReadLine();

            Console.Write("Enter new Product Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal updatedPrice))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid price. Please enter a valid decimal number.");
                Console.ResetColor();
                return;
            }

            Console.Write("Enter new Product Description: ");
            string updatedDescription = Console.ReadLine();

            string updatedCategory;
            do
            {
                Console.Write("Enter new Product Category (leave blank to keep current): ");
                updatedCategory = Console.ReadLine();
                updatedCategory = string.IsNullOrWhiteSpace(updatedCategory) ? product.Category : updatedCategory;

                if (string.IsNullOrWhiteSpace(updatedCategory))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Product Category cannot be empty.");
                    Console.ResetColor();
                }
            } while (string.IsNullOrWhiteSpace(updatedCategory));

            productRepository.UpdateProduct(new Product(updateId, updatedName, updatedPrice, updatedDescription, updatedCategory)); // Using UpdateProduct

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Product {updateId} updated successfully.");
            Console.ResetColor();
        }


        public void RemoveProduct()
        {
            Console.WriteLine("\n=== Remove a Product ===");
            Console.Write("Enter Product ID to remove: ");
            string removeId = Console.ReadLine();

            if (productRepository.ProductExists(removeId))
            {
                productRepository.RemoveProduct(removeId);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Product {removeId} removed successfully.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Product with ID {removeId} not found.");
                Console.ResetColor();
            }
        }

        public void DisplayAllProducts()
        {
            var allProducts = productRepository.GetAllProducts();

            if (allProducts.Any())
            {
                // Main logic of displaying products
                Console.WriteLine("Product Catalog:");
                foreach (var product in allProducts)
                {
                    Console.WriteLine($"{product.Id}: {product.Name} - ${product.Price} ({product.Description}) [Category: {product.Category}]");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No products available in the catalog.");
                Console.ResetColor();
                return;
            }
        }

        /// <summary>
        /// Prompts the user to enter a Product ID and validates the input.
        /// </summary>
        /// <returns>The validated Product ID or null if the user chooses to go back.</returns>
        private string GetProductId()
        {
            string newId;
            do
            {
                Console.Write("Enter Product ID (type 'back' to return): ");
                newId = Console.ReadLine();

                if (newId.ToLower() == "back")
                {
                    Console.WriteLine("Returning to main menu...");
                    logger.Info("User chose to return to the main menu while entering Product ID.");  // Log back action
                    return null; // Indicates user wants to go back.
                }

                if (string.IsNullOrEmpty(newId))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Product ID cannot be empty.");
                    Console.ResetColor();
                    logger.Warn("Empty Product ID entered.");  // Log warning
                }
                else if (productRepository.ProductExists(newId))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Product with ID {newId} already exists.");
                    Console.ResetColor();
                    logger.Warn($"Product with ID {newId} already exists.");  // Log warning
                    newId = null; // Force another attempt
                }
            } while (string.IsNullOrWhiteSpace(newId));

            logger.Info($"Product ID validated: {newId}");  // Log successful validation
            return newId;
        }

        /// <summary>
        /// Prompts the user to enter a Product Name and validates the input.
        /// </summary>
        /// <returns>The validated Product Name.</returns>
        private string GetProductName()
        {
            string newName;
            do
            {
                Console.Write("Enter Product Name: ");
                newName = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Product Name cannot be empty.");
                    Console.ResetColor();
                    logger.Warn("Empty Product Name entered.");  // Log warning
                }
            } while (string.IsNullOrWhiteSpace(newName));

            logger.Info($"Product Name validated: {newName}");  // Log successful validation
            return newName;
        }

        /// <summary>
        /// Prompts the user to enter a Product Price and validates the input.
        /// </summary>
        /// <returns>The validated Product Price as a decimal.</returns>
        private decimal GetProductPrice()
        {
            decimal newPrice;
            do
            {
                Console.Write("Enter Product Price: ");
                string priceInput = Console.ReadLine();

                if (!decimal.TryParse(priceInput, out newPrice) || newPrice < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid price. Please enter a valid non-negative decimal number.");
                    Console.ResetColor();
                    logger.Warn($"Invalid price entered: {priceInput}");  // Log warning
                }
            } while (newPrice < 0);

            logger.Info($"Product Price validated: {newPrice}");  // Log successful validation
            return newPrice;
        }

        /// <summary>
        /// Prompts the user to enter a Product Description and validates the input.
        /// </summary>
        /// <returns>The validated Product Description.</returns>
        private string GetProductDescription()
        {
            string newDescription;
            do
            {
                Console.Write("Enter Product Description: ");
                newDescription = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(newDescription))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Product Description cannot be empty.");
                    Console.ResetColor();
                    logger.Warn("Empty Product Description entered.");  // Log warning
                }
            } while (string.IsNullOrWhiteSpace(newDescription));

            logger.Info($"Product Description validated: {newDescription}");  // Log successful validation
            return newDescription;
        }

        public void SearchProducts()
        {
            Console.WriteLine("\n=== Search Products ===");
            Console.Write("Enter a keyword (Product ID or Name): ");
            string keyword = Console.ReadLine();

            // Get all products from the repository
            var allProducts = productRepository.GetAllProducts();

            // Filter products based on the search keyword
            var searchResults = allProducts.Where(p =>
                p.Id.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

            // Display search results
            if (searchResults.Any())
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Search Results:");
                foreach (var product in searchResults)
                {
                    Console.WriteLine($"{product.Id}: {product.Name} - ${product.Price} ({product.Description}) [Category: {product.Category}]");
                }
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No products found matching your search criteria.");
                Console.ResetColor();
            }
        }


    }
}

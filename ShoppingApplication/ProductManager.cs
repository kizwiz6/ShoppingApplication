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

        public ProductManager(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }

        /// <summary>
        /// Adds a new product to the product catalog.
        /// Ensures input validation for Product ID, Product Name, Product Price, and Product Description.
        /// </summary>
        public void AddProduct()
        {
            try
            {
                Console.WriteLine("\n=== Add a New Product ===");

                string newId;

                // Validate Product ID
                do
                {
                    Console.Write("Enter Product ID: ");
                    newId = Console.ReadLine();

                    if (newId.ToLower() == "back")
                    {
                        Console.WriteLine("Returning to main menu...");
                        return; // Exit AddProduct and go back to the main menu.
                    }

                    if (string.IsNullOrEmpty(newId))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Product ID cannot be empty.");
                        Console.ResetColor();
                    }
                    else if (productRepository.ProductExists(newId))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Product with ID {newId} already exists.");
                        Console.ResetColor();
                        newId = null; // Force another attempt
                    }
                } while (string.IsNullOrWhiteSpace(newId));

                // Validate Product Name
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
                    }
                } while (string.IsNullOrWhiteSpace(newName));

                // Validate Product Price
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
                    }
                } while (newPrice < 0);

                // Validate Product Description
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
                    }
                } while (string.IsNullOrWhiteSpace(newDescription));

                // Create and add the new product to the repository
                var newProduct = new Product(newId, newName, newPrice, newDescription);
                productRepository.AddProduct(newProduct);

                // Confirmation message
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Product added successfully!");
                Console.ResetColor();
            }
            catch (FormatException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: Invalid input format. {ex.Message}");
                Console.ResetColor();
                throw;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Console.ResetColor();
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

            productRepository.UpdateProduct(new Product(updateId, updatedName, updatedPrice, updatedDescription)); // Using UpdateProduct

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
                    Console.WriteLine($"{product.Id}: {product.Name} - ${product.Price} ({product.Description})");
                }
            }
            else
            {
                Console.WriteLine("No products available in the catalog.");
                return;
            }
        }
    }
}

using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

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
                logger.Info("Attempting to add a new product.");
                Console.WriteLine("\n=== Add a New Product ===");

                string newId = GetProductId();
                if (newId == null) return; // User chose to go back to the main menu.

                string newName = GetProductName();
                decimal newPrice = GetProductPrice();
                string newDescription = GetProductDescription();
                string newCategory = GetNonEmptyInput("Enter Product Category:");

                var newProduct = new Product(newId, newName, newPrice, newDescription, newCategory);
                productRepository.AddProduct(newProduct);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Product added successfully!");
                Console.ResetColor();

                logger.Info($"Product added: ID={newId}, Name={newName}, Price={newPrice}, Description={newDescription}");
            }
            catch (Exception ex)
            {
                HandleError(ex);
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
                NotifyNotFound(updateId);
                return;
            }

            Console.WriteLine($"Current Product: {product.Name} - ${product.Price} ({product.Description})");

            string updatedName = GetProductName();
            decimal updatedPrice = GetProductPrice();
            string updatedDescription = GetProductDescription();
            string updatedCategory = GetNonEmptyInput("Enter new Product Category (leave blank to keep current):", product.Category);

            productRepository.UpdateProduct(new Product(updateId, updatedName, updatedPrice, updatedDescription, updatedCategory));

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
                NotifyNotFound(removeId);
            }
        }

        public IEnumerable<Product> DisplayAllProducts(string sortBy = "name", int pageNumber = 1, int pageSize = 5)
        {
            var allProducts = productRepository.GetAllProducts();

            // Sort the products based on the specified criteria
            allProducts = sortBy.ToLower() switch
            {
                "price" => allProducts.OrderBy(p => p.Price).ToList(),
                "category" => allProducts.OrderBy(p => p.Category).ToList(),
                _ => allProducts.OrderBy(p => p.Name).ToList(),
            };

            int totalPages = (int)Math.Ceiling((double)allProducts.Count() / pageSize);

            if (!ValidatePageNumber(pageNumber, totalPages)) return Enumerable.Empty<Product>();

            var productsToDisplay = allProducts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            if (productsToDisplay.Any())
            {
                Console.WriteLine($"Product Catalog (Page {pageNumber}/{totalPages}):");
                foreach (var product in productsToDisplay)
                {
                    Console.WriteLine($"{product.Id}: {product.Name} - ${product.Price} ({product.Description}) [Category: {product.Category}]");
                }
            }
            else
            {
                Console.WriteLine("No products available in the catalog.");
            }

            DisplayPaginationOptions(sortBy, pageNumber, totalPages, pageSize);

            return productsToDisplay;
        }

        public void SearchProducts()
        {
            Console.WriteLine("\n=== Search Products ===");
            Console.Write("Enter a keyword (Product ID or Name): ");
            string keyword = Console.ReadLine();

            var allProducts = productRepository.GetAllProducts();

            var searchResults = allProducts.Where(p =>
                p.Id.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();

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

        public void AddReviewToProduct()
        {
            Console.Write("Enter Product ID to review: ");
            string productId = Console.ReadLine();
            var product = productRepository.GetProductById(productId);

            if (product == null)
            {
                NotifyNotFound(productId);
                return;
            }

            string userName = GetNonEmptyInput("Enter your name:");
            int rating = GetRatingInput();
            string comment = GetNonEmptyInput("Enter your review comment:");

            var review = new Review(userName, rating, comment);
            product.AddReview(review);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Review added successfully!");
            Console.ResetColor();
        }

        public void DisplayProductReviews()
        {
            Console.Write("Enter Product ID to view reviews: ");
            string productId = Console.ReadLine();
            var product = productRepository.GetProductById(productId);

            if (product == null)
            {
                NotifyNotFound(productId);
                return;
            }

            Console.WriteLine($"Reviews for {product.Name}:");
            if (product.Reviews.Any())
            {
                foreach (var review in product.Reviews)
                {
                    Console.WriteLine($"- {review.User} rated {review.Rating}/5: {review.Comment} (on {review.Date})");
                }
            }
            else
            {
                Console.WriteLine("No reviews for this product yet.");
            }
        }

        /// <summary>
        /// Gets a non-empty input from the user, with an optional default value.
        /// </summary>
        /// <param name="prompt">The message to display to the user.</param>
        /// <param name="defaultValue">The default value to use if the user inputs nothing.</param>
        /// <returns>The validated input.</returns>
        private string GetNonEmptyInput(string prompt, string defaultValue = "")
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    input = defaultValue;
                }

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{prompt} cannot be empty.");
                    Console.ResetColor();
                }
            } while (string.IsNullOrWhiteSpace(input));

            logger.Info($"{prompt.TrimEnd(':')} validated: {input}");
            return input;
        }

        /// <summary>
        /// Gets a valid rating from the user (1 to 5).
        /// </summary>
        /// <returns>The validated rating.</returns>
        private int GetRatingInput()
        {
            int rating;
            do
            {
                Console.Write("Enter your rating (1-5): ");
                if (!int.TryParse(Console.ReadLine(), out rating) || rating < 1 || rating > 5)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
                    Console.ResetColor();
                }
            } while (rating < 1 || rating > 5);

            return rating;
        }

        /// <summary>
        /// Notifies the user when a product is not found.
        /// </summary>
        /// <param name="productId">The ID of the product that was not found.</param>
        private void NotifyNotFound(string productId)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Product with ID {productId} not found.");
            Console.ResetColor();
        }

        private bool ValidatePageNumber(int pageNumber, int totalPages)
        {
            if (pageNumber < 1 || pageNumber > totalPages)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid page number");
                Console.ResetColor();
                return false; // Indicates invalid page number
            }
            return true; // Indicates valid page number
        }

        private void DisplayPaginationOptions(string sortBy, int pageNumber, int totalPages, int pageSize)
        {
            Console.WriteLine("Navigation: (N)ext, (P)revious, or enter a page number to jump to a specific page.");
            string input = Console.ReadLine();

            if (input.Equals("N", StringComparison.OrdinalIgnoreCase) && pageNumber < totalPages)
            {
                DisplayAllProducts(sortBy, pageNumber + 1, pageSize);
            }
            else if (input.Equals("P", StringComparison.OrdinalIgnoreCase) && pageNumber > 1)
            {
                DisplayAllProducts(sortBy, pageNumber - 1, pageSize);
            }
            else if (int.TryParse(input, out int newPageNumber) && newPageNumber >= 1 && newPageNumber <= totalPages)
            {
                DisplayAllProducts(sortBy, newPageNumber, pageSize);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input.");
                Console.ResetColor();
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
                if (!decimal.TryParse(priceInput, out newPrice) || newPrice <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid price. Please enter a positive decimal number.");
                    Console.ResetColor();
                }
            } while (newPrice <= 0);

            logger.Info($"Product Price validated: {newPrice}");  // Log successful validation
            return newPrice;
        }

        /// <summary>
        /// Prompts the user to enter a Product Description and validates the input.
        /// </summary>
        /// <returns>The validated Product Description.</returns>
        private string GetProductDescription()
        {
            Console.Write("Enter Product Description: ");
            string newDescription = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newDescription))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Product Description cannot be empty.");
                Console.ResetColor();
                return GetProductDescription(); // Recursive call for re-entry
            }

            logger.Info($"Product Description validated: {newDescription}");  // Log successful validation
            return newDescription;
        }

        public void DisplayProductCatalog(IEnumerable<Product> products)
        {
            if (products == null || !products.Any())
            {
                Console.WriteLine("No products available in the catalog.");
                return;
            }

            Console.WriteLine("Product Catalog:");
            foreach (var product in products)
            {
                Console.WriteLine($"{product.Id}: {product.Name} - ${product.Price} ({product.Description}) [Category: {product.Category}]");
            }
        }

        public void HandleError(Exception ex)
        {
            // Log the error message (this can be enhanced with more details)
            Console.WriteLine($"An error occurred: {ex.Message}");

            // Optionally, you can log the stack trace or other relevant information
            Console.WriteLine(ex.StackTrace);

            // Provide user feedback if necessary
            Console.WriteLine("Please try again or contact support if the issue persists.");
        }

    }
}

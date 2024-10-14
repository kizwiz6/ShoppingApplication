using System.Collections.Generic;

namespace ShoppingApplication
{
    public interface IProductManager
    {
        void AddProduct(); // Adds a new product; input is taken via Console
        void UpdateProduct(); // Updates an existing product; handled in method
        void RemoveProduct(); // Removes a product; handled in method
        IEnumerable<Product> DisplayAllProducts(string sortBy = "name", int pageNumber = 1, int pageSize = 5); // Displays products with sorting and pagination
        void SearchProducts(); // Searches for products based on a keyword
        void AddReviewToProduct(); // Adds a review to a specified product
        void DisplayProductReviews(); // Displays reviews for a specified product
        void DisplayProductCatalog(IEnumerable<Product> products); // Displays a list of products in the catalog
    }
}

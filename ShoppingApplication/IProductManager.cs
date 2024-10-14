using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingApplication
{
    public interface IProductManager
    {
        void AddProduct(); // No parameter since input is taken via Console
        void UpdateProduct(); // No parameter, handled in method
        void RemoveProduct(); // No parameter, handled in method
        void DisplayAllProducts(string sortBy = "name"); // Just displays products, no arguments needed
        void SearchProducts(); // Method for searching products
    }
}

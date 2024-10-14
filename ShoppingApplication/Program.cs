using System;
using System.Collections.Generic;

namespace ShoppingApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IProductRepository productRepository = new ProductRepository();
            IProductManager productManager = new ProductManager(productRepository);


            // Clear console and display welcome message
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Welcome to the Shopping Application!\n");
            Console.ResetColor();

            // Main loop to display the menu and handle user input
            while (true)
            {
                DisplayMenu();

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        productManager.DisplayAllProducts();
                        break;
                    case "2":
                        productManager.AddProduct();
                        break;
                    case "3":
                        productManager.UpdateProduct();
                        break;
                    case "4":
                        productManager.RemoveProduct();
                        break;
                    case "5":
                        // Exit the application
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Thank you for using the Shopping Application! Goodbye.");
                        Console.ResetColor();
                        return; // Exit the loop and terminate the program
                    default:
                        // Handle invalid input
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        // Display the menu options
        private static void DisplayMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n========= Main Menu =========");
            Console.ResetColor();
            Console.WriteLine("1. View all products");
            Console.WriteLine("2. Add a product");
            Console.WriteLine("3. Update a product");
            Console.WriteLine("4. Remove a product");
            Console.WriteLine("5. Exit");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=============================");
            Console.ResetColor();
            Console.Write("Please choose an option: ");
        }
    }
}
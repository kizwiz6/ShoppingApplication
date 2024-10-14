﻿using System;

namespace ShoppingApplication
{
    /// <summary>
    /// The main entry point for the Shopping Application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main method that initializes the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            IProductRepository productRepository = new ProductRepository();
            IProductManager productManager = new ProductManager(productRepository);

            // Clear console and display welcome message
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Welcome to the Shopping Application!\n");
            Console.ResetColor();

            ShowMainMenu(productManager);
        }

        /// <summary>
        /// Displays the main menu and handles user input for navigation.
        /// </summary>
        /// <param name="productManager">The product manager instance to handle product operations.</param>
        public static void ShowMainMenu(IProductManager productManager)
        {
            bool exit = false;

            while (!exit)
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

                string input = Console.ReadLine();

                // Validate user input
                if (int.TryParse(input, out int option))
                {
                    switch (option)
                    {
                        case 1:
                            productManager.DisplayAllProducts();
                            break;
                        case 2:
                            productManager.AddProduct();
                            break;
                        case 3:
                            productManager.UpdateProduct();
                            break;
                        case 4:
                            productManager.RemoveProduct();
                            break;
                        case 5:
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
                else
                {
                    // Handle invalid input
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid choice. Please try again.");
                    Console.ResetColor();
                }
            }
        }
    }
}

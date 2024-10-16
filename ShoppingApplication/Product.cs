﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingApplication
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public List<Review> Reviews { get; set; }


        public Product(string id, string name, decimal price, string description, string category)
        {
            Id = id;
            Name = name;
            Price = price;
            Description = description;
            Category = category;
            Reviews = new List<Review>();
        }

        public void AddReview(Review review)
        {
            Reviews.Add(review);
        }

        public double AverageRating()
        {
            return Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;
        }
    }
}

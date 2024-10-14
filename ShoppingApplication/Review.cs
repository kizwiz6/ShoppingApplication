using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingApplication
{
    public class Review
    {
        public string User { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }

        public Review(string user, int rating, string comment)
        {
            User = user;
            Rating = rating;
            Comment = comment;
            Date = DateTime.Now;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class Review
    {
        public String description;
        public int rating;
        public int bookingNumber;

        public Review(int bookingNumber)
        {
            description = "";
            rating = 0;
            this.bookingNumber = bookingNumber;
        }

        public Review(string description, int rating, int bookingNumber)
        {
            this.description = description;
            this.rating = rating;
            this.bookingNumber = bookingNumber;
        }
    }
}

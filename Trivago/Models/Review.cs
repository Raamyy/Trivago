using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class Review
    {
        public String Description;
        public int Rating;

        public Review(string description, int rating)
        {
            Description = description;
            Rating = rating;
        }
    }
}

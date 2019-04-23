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

        public Review(string description, int rating)
        {
            this.description = description;
            this.rating = rating;
        }
    }
}

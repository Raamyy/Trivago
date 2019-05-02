using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class Website
    {
        public String name;
        public int rating;

        public Website(string name, int rating)
        {
            this.name = name;
            this.rating = rating;
        }

        public override string ToString()
        {
            return name;
        }
    }
}

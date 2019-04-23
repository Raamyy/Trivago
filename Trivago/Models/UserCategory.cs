using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class UserCategory
    {
        public String name;
        public int discount;
        public int minimumThreshold; // Number of bookings needed to enter the category.

        public UserCategory(string name, int discount, int minimumThreshold)
        {
            this.name = name;
            this.discount = discount;
            this.minimumThreshold = minimumThreshold;
        }
    }
}

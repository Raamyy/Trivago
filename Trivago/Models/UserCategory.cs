using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class UserCategory
    {
        public String Name;
        public int Discount;
        public int MinimumThreshold; // Number of bookings needed to enter the category.
    }
}

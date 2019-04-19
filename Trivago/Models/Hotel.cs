using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    class Hotel
    {
        public int LicenceNumber;
        public String Name;
        public CustomImage Image;
        public Location Location;
        public List<HotelFacility> Facilities;
        public List<MealPlan> MealPlans;
    }
}

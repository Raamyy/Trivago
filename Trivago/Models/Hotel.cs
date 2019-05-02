using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivago.Front_End;

namespace Trivago.Models
{
    public class Hotel
    {
        public int licenseNumber;
        public String name;
        public CustomImage image;
        public Location location;
        public List<HotelFacility> facilities;
        public List<MealPlan> mealPlans;
        
        public Hotel(int licenseNumber, String name, CustomImage image, 
                     Location location, List<HotelFacility> facilities, List<MealPlan> mealPlans)
        {
            this.licenseNumber = licenseNumber;
            this.name = name;
            this.image = image;
            this.location = location;
            this.facilities = facilities;
            this.mealPlans = mealPlans;
        }

        public override string ToString()
        {
            return name + $"( {licenseNumber.ToString()} )";
        }
    }
}

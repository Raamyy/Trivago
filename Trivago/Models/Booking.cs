using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class Booking
    {
        public int Number;
        public DateTime StartDate;
        public DateTime EndDate;
        public int NumberOfGuests;
        User BookingUser;
        MealPlan BookingMealPlan;
        Room BookingRoom;
        Review BookingReview;
        Website BookingWebsite;
    }
}

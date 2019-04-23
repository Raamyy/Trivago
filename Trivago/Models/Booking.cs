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
        public User BookingUser;
        public MealPlan BookingMealPlan;
        public Room BookingRoom;
        public Review BookingReview;
        public Website BookingWebsite;

        public Booking(int number, DateTime startDate, DateTime endDate, int numberOfGuests, User bookingUser, MealPlan bookingMealPlan, Room bookingRoom, Review bookingReview, Website bookingWebsite)
        {
            Number = number;
            StartDate = startDate;
            EndDate = endDate;
            NumberOfGuests = numberOfGuests;
            BookingUser = bookingUser;
            BookingMealPlan = bookingMealPlan;
            BookingRoom = bookingRoom;
            BookingReview = bookingReview;
            BookingWebsite = bookingWebsite;
        }
    }
}

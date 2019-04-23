using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class Booking
    {
        public int number;
        public DateTime startDate;
        public DateTime endDate;
        public int numberOfGuests;
        public User bookingUser;
        public MealPlan bookingMealPlan;
        public Room bookingRoom;
        public Review bookingReview;
        public Website bookingWebsite;

        public Booking(int number, DateTime startDate, DateTime endDate, 
                       int numberOfGuests, User bookingUser, MealPlan bookingMealPlan, 
                       Room bookingRoom, Review bookingReview, Website bookingWebsite)
        {
            this.number = number;
            this.startDate = startDate;
            this.endDate = endDate;
            this.numberOfGuests = numberOfGuests;
            this.bookingUser = bookingUser;
            this.bookingMealPlan = bookingMealPlan;
            this.bookingRoom = bookingRoom;
            this.bookingReview = bookingReview;
            this.bookingWebsite = bookingWebsite;
        }
    }
}

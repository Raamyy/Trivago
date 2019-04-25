using System;
using System.Collections.Generic;

using Oracle.DataAccess.Client;
using System.Data;
using Trivago.Front_End;

namespace Trivago.Models
{
    public class DataModels
    {
        private static DataModels dataModels;
        private string oracleConnectionString;
        private OracleConnection connection;
        private OracleCommand command;

        public static DataModels GetInstance()
        {
            if (dataModels == null)
                dataModels = new DataModels();
            return dataModels;
        }

        private DataModels()
        {
            oracleConnectionString = "data source = orcl; user id = scott; password = tiger;";
            connection = new OracleConnection(oracleConnectionString);
            connection.Open();
        }

        public List<Location> GetAllLocations()
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT *
                                    FROM location";

            OracleDataReader reader = command.ExecuteReader();
            List<Location> locations = new List<Location>();
            while (reader.Read())
            {
                string country = reader["country"].ToString();
                string city = reader["city"].ToString();
                List<PlaceOfIntrest> placesOfIntrests = GetPlacesOfInterest(country, city);
                locations.Add(new Location(placesOfIntrests, country, city));
            }
            return locations;
        }

        public List<RoomType> GetAllRoomTypes()
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT *
                                    FROM room_type";

            OracleDataReader reader = command.ExecuteReader();
            List<RoomType> roomTypes= new List<RoomType>();
            while (reader.Read())
            {
                string name = reader["type_name"].ToString();
                int maxGuests =int.Parse(reader["maximum_guests"].ToString());
                
                roomTypes.Add(new RoomType(name,maxGuests));
            }
            return roomTypes;
        }

        public Hotel GetHotel(int licenseNumber)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT Hotel.*
                                    FROM Hotel
                                    WHERE License_number = :license";
            command.Parameters.Add("license", licenseNumber);

            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                String hotelName = reader["hotel_name"].ToString();
                byte[] image = (byte[])reader["hotel_image"];
                String country = reader["country"].ToString();
                String city = reader["city"].ToString();
                Location location = GetLocation(country, city);
                List<HotelFacility> facilities = GetFacilities(licenseNumber);
                List<MealPlan> meals = GetMealPlans(licenseNumber);

                Hotel hotel = new Hotel(licenseNumber, hotelName, new CustomImage(image),
                    location, facilities, meals);
                return hotel;
            }

            return null;
        }

        public Location GetLocation(String country, String city)
        {
            Location location = new Location(GetPlacesOfInterest(country, city), country, city);
            return location;
        }

        public Booking GetBooking(int bookingNumber)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT Booking.*, website_name, room_number
                                    FROM Booking, define_booking
                                    WHERE booking.booking_number = :bookingNumber";
            command.Parameters.Add("bookingNumber", bookingNumber);

            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                DateTime startDate = (DateTime) reader["start_date"];
                DateTime endDate = (DateTime) reader["end_date"];
                int numberOfGuests = int.Parse(reader["number_of_guests"].ToString());
                User bookingUser = GetUser(reader["user_name"].ToString());
                int hotelLicenceNumber = int.Parse(reader["licence_number"].ToString());
                MealPlan bookingMealPlan = GetMealPlan(hotelLicenceNumber, reader["meal_plan"].ToString()); //Hotel and plan name defines the meal plan
                Room room = GetRoom(hotelLicenceNumber, int.Parse(reader["room_number"].ToString()));
                Review review = GetReview(bookingNumber);
                Website website = GetWebsite(reader["website_name"].ToString());

                Booking booking = new Booking(
                        bookingNumber,
                        startDate,
                        endDate,
                        numberOfGuests,
                        bookingUser,
                        bookingMealPlan,
                        room,
                        review,
                        website
                    );
                return booking;
            }
            return null;
        }

        private Website GetWebsite(string websiteName)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT rating
                                    FROM website
                                    WHERE name = :websiteName";
            command.Parameters.Add("websiteName", websiteName);

            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                return new Website(websiteName, int.Parse(reader["rating"].ToString()));
            }
            return null;
        }

        private Review GetReview(int bookingNumber)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT *
                                    FROM review
                                    WHERE booking_number = :bookingNum";
            command.Parameters.Add("bookingNum", bookingNumber);

            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                return new Review(reader["description"].ToString(), int.Parse(reader["rating"].ToString()));
            }
            return null;
        }

        private Room GetRoom(int hotelLicenceNumber, int roomNumber)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT *
                                    FROM room
                                    WHERE room_number = :roomNumber
                                    AND hotel_license_number = :hotelNumber";

            command.Parameters.Add("roomNumber", roomNumber);
            command.Parameters.Add("hotelNumber", hotelLicenceNumber);

            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Build the room object with the dependant objects
                String s = reader["hotel_license_number"].ToString();
                Hotel hotel = GetHotel(Int32.Parse(s));
                List<RoomView> views = GetViews(roomNumber, hotelLicenceNumber);
                byte[] image = (byte[])reader["room_image"];
                string roomType = reader["room_type"].ToString();

                return new Room(roomNumber, hotel, GetRoomType(roomType),
                                new CustomImage(image), views);
            }
            return null;
        }

        private MealPlan GetMealPlan(int hotelLicenceNumber, string mealPlanName)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = $@"SELECT price
                                    FROM meal_plan
                                    WHERE name = :mealName
                                    AND hotel_license_number = :hotelNumber";
                                    
            command.Parameters.Add("mealName", mealPlanName);
            command.Parameters.Add("hotelNumber", hotelLicenceNumber);

            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int price = int.Parse(reader["price"].ToString());
                return new MealPlan(mealPlanName, price);
            }

            return null;

        }

        public User GetUser(string userName)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT *
                                    FROM website_user
                                    WHERE user_name = :userName";
            command.Parameters.Add("userName", userName);

            OracleDataReader reader = command.ExecuteReader();
            User user = null;
            while (reader.Read())
            {
                string email = reader["email"].ToString();
                string name = reader["name"].ToString();
                UserCategory category = GetUserCategory(reader["user_category"].ToString());
                CreditCard creditCard = GetCreditCard(reader["credit_card_number"].ToString());
                                        
                user = new User(userName, email, name, category, creditCard);
            }
            reader.Close();
            return user;
        }

        private CreditCard GetCreditCard(string creditCardNumber)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT *
                                    FROM credit_card
                                    WHERE credit_card_number = :creditNumber";
            command.Parameters.Add("creditNumber", creditCardNumber);

            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                return new CreditCard(creditCardNumber,
                                      int.Parse(reader["cvv"].ToString()),
                                      (DateTime)reader["expiration_date"]);
            }
            return null;
        }

        private UserCategory GetUserCategory(string categoryName)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT *
                                    FROM user_category
                                    WHERE name = :categoryName";
            command.Parameters.Add("categoryName", categoryName);
            
            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                UserCategory userCategory = new UserCategory(categoryName,
                    int.Parse(reader["discount"].ToString()),
                    int.Parse(reader["minimum_threshold"].ToString())
                    );
                return userCategory;
            }
            return null;
        }

        public RoomType GetRoomType(int numberOfGuest)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"select type_name
                                    FROM room_type
                                    Where maximum_guests = :guests";
            command.Parameters.Add("guests", numberOfGuest);

            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                return new RoomType(reader[0].ToString(), numberOfGuest);
            }
            return null;
        }

        public RoomType GetRoomType(string typeName)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"select maximum_guests
                                    FROM room_type
                                    Where type_name = :type";
            command.Parameters.Add("type", typeName);

            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                return new RoomType(typeName,int.Parse(reader[0].ToString()));
            }
            return null;
        }

        public List<HotelFacility> GetFacilities(int hotelLicense)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT Facility_name, Facility_image
                                    FROM Hotel_Facilities
                                    WHERE License_Number = :license";
            command.Parameters.Add("license", hotelLicense);

            List<HotelFacility> facilites = new List<HotelFacility>();
            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                String name = reader["facility_name"].ToString();
                byte[] image = (byte[])reader["facility_image"];
                HotelFacility facility = new HotelFacility(name, new CustomImage(image));
                facilites.Add(facility);
            }

            return facilites;
        }

        public List<MealPlan> GetMealPlans(int hotelLicense)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT Name, Price
                                    FROM Meal_Plan
                                    WHERE Hotel_License_Number = :license";
            command.Parameters.Add("license", hotelLicense);

            List<MealPlan> plans = new List<MealPlan>();
            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string name = reader["name"].ToString();
                String price =reader["price"].ToString();
                MealPlan plan = new MealPlan(name,(int.Parse(price)));
                plans.Add(plan);
            }
            return plans;
        }

        private List<PlaceOfIntrest> GetPlacesOfInterest(String country, String city)
        {
            /// <summary>
            /// Gets places of interest in a certain location defined by country and city.
            /// </summary>
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT place_of_intrest, place_image
                                    FROM Places_of_intrest
                                    WHERE country = :country
                                    AND city = :city";
            command.Parameters.Add("country", country);
            command.Parameters.Add("city", city);

            List<PlaceOfIntrest> places = new List<PlaceOfIntrest>(0);
            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                String name = reader["place_of_intrest"].ToString();
                byte[] placeImage = (byte[]) reader["place_image"];
                PlaceOfIntrest place = new PlaceOfIntrest(name, new CustomImage(placeImage));
                places.Add(place);
            }
            reader.Close();

            return places;
        }

        public List<RoomView> GetViews(int hotelLicense, int roomNumber)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT Room_view
                                    FROM Room_views
                                    WHERE Room_number = :room
                                    AND license_number = :hotel";
            command.Parameters.Add("room", roomNumber);
            command.Parameters.Add("hotel", hotelLicense);

            List<RoomView> views = new List<RoomView>();
            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                RoomView view = new RoomView(reader["Room_view"].ToString());
                views.Add(view);
            }

            return views;
        }

        public List<Room> GetRooms(List<Location> locations, int guestsCount, DateTime startDate, DateTime endDate)
        {
            /// <summary>
            /// Returns a list of rooms available in the given locations
            /// withing the given date and matching the number of guests.
            /// </summary>
            List<Room> rooms = new List<Room>();
            foreach (Location location in locations)
            {
                command = new OracleCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText = @"SELECT Room.*, Hotel.license_number
                                        FROM Room, Hotel, Room_Type
                                        WHERE Room.hotel_license_number = Hotel.license_number
                                        AND Room.room_type = room_type.type_name
                                        AND Hotel.country = :country AND Hotel.city = :city
                                        AND room_type.maximum_guests = :guests";
                command.Parameters.Add("country", location.country);
                command.Parameters.Add("city", location.city);
                command.Parameters.Add("guests", guestsCount.ToString());

                OracleDataReader reader = command.ExecuteReader();
                Room room;
                while (reader.Read())
                {
                    // Build the room object with the dependant objects
                    
                    Hotel hotel = GetHotel(Int32.Parse(reader["license_number"].ToString()));
                    int roomNumber = Int32.Parse(reader["room_number"].ToString());

                    room = GetRoom(hotel.licenseNumber, roomNumber);

                    // Check if the room is available in the given dates
                    if (IsRoomAvailable(room, startDate, endDate))
                        rooms.Add(room);
                }
                reader.Close();
            }
            return rooms;
        }

        public List<Booking> GetUserBookings(User user)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT booking_number
                                    FROM Booking
                                    WHERE user_name = :userName";
            command.Parameters.Add("userName", user.username);

            OracleDataReader reader = command.ExecuteReader();
            List<Booking> bookings = new List<Booking>();
            while (reader.Read())
            {
                Booking booking = GetBooking(int.Parse(reader["booking_number"].ToString()));
                bookings.Add(booking);
            }
            reader.Close();
            return bookings;
        }

        bool IsRoomAvailable(Room room, DateTime startDate, DateTime endDate)
        {
            /// <summary>
            /// Helper Function.
            /// Checks if the given room is free to book within the give dates.
            /// </summary>
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT Start_Date, End_Date
                                    FROM Booking
                                    WHERE Booking_Number = 
                                    (SELECT Booking_Number
                                    FROM Define_Booking
                                    WHERE Room_Number = :roomNumber
                                    AND Hotel_License_Number = :hotelNumber)";
            command.Parameters.Add("roomNumber", room.number);
            command.Parameters.Add("hotelNumber", room.hotel.licenseNumber);

            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                DateTime bookingStart = (DateTime)reader["start_date"];
                DateTime bookingEnd = (DateTime)reader["end_date"];

                if (startDate >= bookingStart && startDate <= bookingEnd)
                    return false;
                if (endDate >= bookingStart && endDate <= bookingEnd)
                    return false;
            }
            reader.Close();

            return true;
        }

        public List<Review> GetRoomReviews(Room room)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;

            int roomNumber = room.number;
            int hotelNumber = room.hotel.licenseNumber;
            command.CommandText = "Get_Room_Reviews";
            command.Parameters.Add("room_number", roomNumber);
            command.Parameters.Add("hotel_number", hotelNumber);
            command.Parameters.Add("reviews", OracleDbType.RefCursor, ParameterDirection.Output);
            OracleDataReader reader = command.ExecuteReader();

            List<Review> reviews = new List<Review>();
            while (reader.Read())
            {
                Review review = new Review(reader["description"].ToString(), 
                    Int32.Parse(reader["rating"].ToString()));
                reviews.Add(review);
            }

            return reviews;
        }


        /*
         * Insertion to database methods.
         */

        public void AddImage()
        {
            /// <summary>
            /// Arbitrary method used for populating the database.
            /// </summary>
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            CustomImage image = new CustomImage("C:\\Users\\ahmed\\Pictures\\Screenshots\\Screenshot (2).png");
            command.CommandText = $"UPDATE Room SET Room_Image = :image";
            command.Parameters.Add("image", image.GetByteImage());
            command.ExecuteNonQuery();
        }

        public void AddHotel(Hotel hotel)
        {
            /// <summary>
            /// Writes a Hotel object to the database,
            /// along with its associated facilities and meal plans.
            /// </summary>
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            // Add Hotel
            command.CommandText = @"INSERT INTO Hotel
                                    (License_Number, Hotel_Name, Hotel_Image, City, Country)
                                    VALUES (:hotelLicense, :hotelName, :hotelImage, :city, :country);";
            command.Parameters.Add("hotelLicense", hotel.licenseNumber);
            command.Parameters.Add("hotelName", hotel.name);
            command.Parameters.Add("hotelImage", hotel.image.GetByteImage());
            command.Parameters.Add("city", hotel.location.city);
            command.Parameters.Add("country", hotel.location.country);
            command.ExecuteNonQuery();

            // Add Hotel facilities and meal plans (referencing Hotel)
            AddFacilities(hotel.licenseNumber, hotel.facilities);
            AddMealPlans(hotel.licenseNumber, hotel.mealPlans);
        }

        private void AddFacilities(int hotelLicenseNumber, List<HotelFacility> facilities)
        {
            foreach (HotelFacility facility in facilities)
            {
                command = new OracleCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText = @"INSERT INTO Hotel_Facilities
                                        (License_Number, Facility_Name, Facility_Image)
                                        VALUES (:hotelNumber, :facilityName, :facilityImage);";
                command.Parameters.Add("hotelNumber", hotelLicenseNumber);
                command.Parameters.Add("facilityName", facility.name);
                command.Parameters.Add("facilityImage", facility.image.GetByteImage());
            }
        }

        private void AddMealPlans(int hotelLicenseNumber, List<MealPlan> meals)
        {
            foreach (MealPlan plan in meals)
            {
                command = new OracleCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                command.CommandText = @"INSERT INTO Meal_Plan
                                        (Name, Hotel_License_Number, Price)
                                        VALUES (:name, :hotelNumber, :price)";
                command.Parameters.Add("name", plan.name);
                command.Parameters.Add("hotelNumber", hotelLicenseNumber);
                command.Parameters.Add("price", plan.price);
                command.ExecuteNonQuery();
            }
        }

        public void AddLocation(Location location)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            // Add location object
            command.CommandText = @"INSERT INTO Location
                                    (Country, City)
                                    VALUES (:country, :city);";
            command.Parameters.Add("country", location.country);
            command.Parameters.Add("city", location.city);
            command.ExecuteNonQuery();

            // Add location's places of interest.
            AddPlacesOfInterest(location.placesOfIntrest);
        }

        private void AddPlacesOfInterest(List<PlaceOfIntrest> places)
        {
            foreach (PlaceOfIntrest place in places)
            {
                command = new OracleCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;

                // Add location object
                command.CommandText = @"INSERT INTO Places_Of_Intrest
                                        (country, city, place_of_intrest, place_image)
                                        VALUES (:country, :city, :place_of_interest, :image)";
                command.Parameters.Add("country", place.country);
                command.Parameters.Add("city", place.city);
                command.Parameters.Add("place_of_interest", place.name);
                command.Parameters.Add("image", place.image.GetByteImage());
                command.ExecuteNonQuery();
            }
        }
    }
}

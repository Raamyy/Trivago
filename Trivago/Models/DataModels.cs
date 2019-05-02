using System;
using System.Collections.Generic;

using Oracle.DataAccess.Client;
using System.Data;
using Trivago.Front_End;
using System.Windows;
using System.Globalization;
using Oracle.DataAccess.Types;

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
            /* Disconnected Mode */
            /// <summary>
            /// Gets a list of all room types in datbase.
            /// </summary>
            OracleDataAdapter adapter = new OracleDataAdapter("SELECT * FROM Room_Type", oracleConnectionString);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset);

            List<RoomType> roomTypes = new List<RoomType>();
            DataRow[] rows = dataset.Tables[0].Select();
            foreach (var row in rows)
            {
                string name = row["type_name"].ToString();
                int maxGuests = int.Parse(row["maximum_guests"].ToString());
                roomTypes.Add(new RoomType(name, maxGuests));
            }
            return roomTypes;
        }

        public List<Room> GetAllRooms()
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT *
                                    FROM room";

            OracleDataReader reader = command.ExecuteReader();
            List<Room> rooms = new List<Room>();
            while (reader.Read())
            {
                int hotelNumber = int.Parse(reader["hotel_license_number"].ToString());
                Hotel hotel = GetHotel(hotelNumber);
                int roomNumber = int.Parse(reader["room_number"].ToString());
                List<RoomView> views = GetViews(roomNumber, hotelNumber);
                byte[] image = (byte[])reader["room_image"];
                string _roomType = reader["room_type"].ToString();
                RoomType roomType = GetRoomType(_roomType);

                rooms.Add(new Room(roomNumber,
                                hotel,
                                roomType,
                                new CustomImage(image),
                                views));
            }
            return rooms;
        }

        public List<Hotel> GetAllHotels()
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT *
                                    FROM hotel";

            OracleDataReader reader = command.ExecuteReader();
            List<Hotel> hotels = new List<Hotel>();
            while (reader.Read())
            {
                int licenseNumber = int.Parse(reader["license_number"].ToString());
                String hotelName = reader["hotel_name"].ToString();
                byte[] image = (byte[])reader["hotel_image"];
                String country = reader["country"].ToString();
                String city = reader["city"].ToString();
                Location location = GetLocation(country, city);
                List<HotelFacility> facilities = GetFacilities(licenseNumber);
                List<MealPlan> meals = GetMealPlans(licenseNumber);

                Hotel hotel = new Hotel(licenseNumber, hotelName, new CustomImage(image),
                    location, facilities, meals);
                hotels.Add(hotel);
            }
            return hotels;
        }

        public List<Website> GetAllWebsites()
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT *
                                    FROM website";

            OracleDataReader reader = command.ExecuteReader();
            List<Website> websites = new List<Website>();
            while (reader.Read())
            {
                String name = reader["name"].ToString();
                int rating = int.Parse(reader["rating"].ToString());
                Website website = new Website(name, rating);
                websites.Add(website);
            }
            return websites;
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
                DateTime startDate = (DateTime)reader["start_date"];
                DateTime endDate = (DateTime)reader["end_date"];
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

        public Review GetReview(int bookingNumber)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;

            command.CommandText = "Get_Review";
            command.Parameters.Add("booking_number_in", bookingNumber);
            command.Parameters.Add("Description_out", OracleDbType.Varchar2, 100).Direction = ParameterDirection.Output;
            command.Parameters.Add("Rating_out", OracleDbType.Int32, ParameterDirection.Output);

            try
            {
                command.ExecuteNonQuery();
            }
            catch
            {
                return null;
            }

            return new Review(command.Parameters["Description_out"].Value.ToString(),
                              int.Parse(command.Parameters["Rating_out"].Value.ToString()),
                              bookingNumber);
        }

        public Room GetRoom(int hotelLicenceNumber, int roomNumber)
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

        public List<Room> GetHotelRooms(int hotelLicenceNumber)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT *
                                    FROM room
                                    where hotel_license_number = :hotelNumber";

            command.Parameters.Add("hotelNumber", hotelLicenceNumber);
            List<Room> rooms = new List<Room>();
            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Build the room object with the dependant objects
                String s = reader["hotel_license_number"].ToString();
                Hotel hotel = GetHotel(Int32.Parse(s));
                int roomNumber = int.Parse(reader["room_number"].ToString());
                List<RoomView> views = GetViews(roomNumber, hotelLicenceNumber);
                byte[] image = (byte[])reader["room_image"];
                string roomType = reader["room_type"].ToString();

               Room room = new Room(roomNumber, hotel, GetRoomType(roomType),
                                new CustomImage(image), views);
                rooms.Add(room);
            }
            return rooms;
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
                return new RoomType(typeName, int.Parse(reader[0].ToString()));
            }
            return null;
        }

        private RoomType GetRoomType(int licenseNumber, int roomNumber)
        {
            /// <summary>
            /// *** NOT USED ***
            /// Gets RoomType object based on hotel and room numbers.
            /// </summary>
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT Room.room_type, Room_Type.maximum_guests
                                    FROM Room, Room_Type
                                    WHERE Room.room_number = :roomNumber
                                    AND Room.hotel_license_number = :hotelNumber
                                    AND Room_Type.type_name = Room.room_type";
            command.Parameters.Add("roomNumber", roomNumber);
            command.Parameters.Add("hotelNumber", licenseNumber);

            OracleDataReader reader = command.ExecuteReader();
            RoomType type = null;
            while (reader.Read())
            {
                type = new RoomType(reader["room_type"].ToString(), 
                    Int32.Parse(reader["maximum_guests"].ToString()));
                return type;
            }
            return type;
        }

        public List<Booking> GetRoomBookings(Room room)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"select booking.*, website_name
                                    FROM booking, define_booking
                                    Where room_number = :room
                                    AND hotel_license_number = :hotel";

            command.Parameters.Add("room", room.number);
            command.Parameters.Add("hotel", room.hotel.licenseNumber);

            OracleDataReader reader = command.ExecuteReader();
            List<Booking> bookings = new List<Booking>();
            while (reader.Read())
            {
                DateTime startDate = (DateTime)reader["start_date"];
                DateTime endDate = (DateTime)reader["end_date"];
                int numberOfGuests = int.Parse(reader["number_of_guests"].ToString());
                User bookingUser = GetUser(reader["user_name"].ToString());
                int hotelLicenceNumber = int.Parse(reader["licence_number"].ToString());
                MealPlan bookingMealPlan = GetMealPlan(hotelLicenceNumber, reader["meal_plan"].ToString()); //Hotel and plan name defines the meal plan
                int bookingNumber = int.Parse(reader["booking_number"].ToString());
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
                bookings.Add(booking);
            }
            return bookings;
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
                String price = reader["price"].ToString();
                MealPlan plan = new MealPlan(name, (int.Parse(price)));
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
                byte[] placeImage = (byte[])reader["place_image"];
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

        public List<Offer> GetAllOffers()
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT website_name, price, license_number, room_number
                                    from room_price";

            OracleDataReader reader = command.ExecuteReader();
            List<Offer> offers = new List<Offer>();
            while (reader.Read())
            {
                string websiteName = reader["website_name"].ToString();
                int price = int.Parse(reader["price"].ToString());
                int licenceNumber = int.Parse(reader["license_number"].ToString());
                int roomNumber = int.Parse(reader["room_number"].ToString());

                Website website = GetWebsite(websiteName);
                Room room = GetRoom(licenceNumber, roomNumber);

                Offer offer = new Offer(website, room, price);
                offers.Add(offer);
            }
            return offers;
        }

        public List<Tuple<Website, int>> GetWebsitePricesForRoom(Room room)
        {
            /// <summary>
            /// Gets a pair list of website and it's price for a specific room,
            /// SORTED ascendingly according to price.
            /// </summary>
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT website_name, price
                                    from room_price
                                     where room_number = :room
                                     and license_number = :hotel";
            command.Parameters.Add("room", room.number);
            command.Parameters.Add("hotel", room.hotel.licenseNumber);

            List<Tuple<Website, int>> websitesPrices = new List<Tuple<Website, int>>();
            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Website website = GetWebsite(reader["website_name"].ToString());
                int price = int.Parse(reader["price"].ToString());
                websitesPrices.Add(new Tuple<Website, int>(website, price));
            }
            websitesPrices.Sort((x, y) => x.Item2.CompareTo(y.Item2));
            return websitesPrices;
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
                    Int32.Parse(reader["rating"].ToString()), Int32.Parse(reader["booking_number"].ToString()));
                reviews.Add(review);
            }

            return reviews;
        }

        public List<Review> GetHotelReviews(Hotel hotel)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"SELECT *
                                    FROM Review
                                    WHERE booking_number in (
                                      SELECT booking_number
                                      FROM Define_Booking
                                      WHERE hotel_license_number = :hotel);";
            command.Parameters.Add("hotelNumber", hotel.licenseNumber);

            List<Review> reviews = new List<Review>();
            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                reviews.Add(
                    new Review(reader["description"].ToString(),
                               int.Parse(reader["rating"].ToString()),
                               int.Parse(reader["booking_number"].ToString()))
                );
            }
            return reviews;
        }

        /* 
         * Disconnected Mode
         */
        public List<Room> GetWebsiteRooms(Website website)
        {
            /// <summary>
            /// Get all rooms linked to a website.
            /// </summary>
            OracleDataAdapter adapter = new OracleDataAdapter(
                $"SELECT * FROM Room_Price WHERE website_name = '{website.name}'", oracleConnectionString);
            DataSet dataset = new DataSet();
            adapter.Fill(dataset);

            List<Room> rooms = new List<Room>();
            DataRow[] rows = dataset.Tables[0].Select();
            foreach (DataRow row in rows)
            {
                Room newRoom = GetRoom(Int32.Parse(row["license_number"].ToString()), 
                    Int32.Parse(row["room_number"].ToString()));
                rooms.Add(newRoom);
            }
            return rooms;
        }

        /*
         * Insertion to database methods.
         */

        public bool AddOffer(Offer offer)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            // Add location object
            command.CommandText = $@"INSERT INTO room_price
                                    VALUES ('{offer.website.name}', {offer.room.hotel.licenseNumber},{offer.room.number},{offer.price})";

            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
            return true;
        }

        public bool AddImage()
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
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }
            return true;
        }

        public bool AddHotel(Hotel hotel)
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
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }

            // Add Hotel facilities and meal plans (referencing Hotel)
            if (AddFacilities(hotel.licenseNumber, hotel.facilities) &
                AddMealPlans(hotel.licenseNumber, hotel.mealPlans))
                return true;
            else
                return false;
        }

        private bool AddFacilities(int hotelLicenseNumber, List<HotelFacility> facilities)
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
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (OracleException)
                {
                    return false;
                }
            }
            return true;
        }

        private bool AddMealPlans(int hotelLicenseNumber, List<MealPlan> meals)
        {
            /// <summary>
            /// Adds hotels meals to database.
            /// </summary>
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
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (OracleException)
                {
                    return false;
                }
            }
            return true;
        }

        public bool AddLocation(Location location)
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
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }

            // Add location's places of interest.
            if (AddPlacesOfInterest(location.placesOfIntrest))
                return true;
            else
                return false;
        }

        private bool AddPlacesOfInterest(List<PlaceOfIntrest> places)
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
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (OracleException)
                {
                    return false;
                }
            }
            return true;
        }

        public bool AddReview(Review review)
        {
            /// <summary>
            /// Writes a Review object to the database.
            /// Done using an oracle stored procedure.
            /// </summary>
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;

            // Add location object
            command.CommandText = "Add_Review";
            command.Parameters.Add("description", review.description);
            command.Parameters.Add("rating", review.rating);
            command.Parameters.Add("booking_number", review.bookingNumber);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }
            return true;
        }

        public bool AddWebsite(Website website)
        {
            /* Disconnected Mode */
            /// <summary>
            /// Adds a new website to the database using Oracle Command Builder.
            /// </summary>
            OracleDataAdapter adapter = new OracleDataAdapter("SELECT * FROM Website", oracleConnectionString);
            DataTable datatable = new DataTable();
            adapter.Fill(datatable);
            DataRow row = datatable.NewRow();
            row["name"] = website.name;
            row["rating"] = website.rating;
            datatable.Rows.Add(row);

            OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
            try
            {
                adapter.Update(datatable);
            }
            catch (OracleException)
            {
                return false;
            }
            return true;
        }

        public bool AddRoom(Room room)
        {
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"INSERT INTO Room
                                    (room_number, hotel_license_number, room_type, room_image)
                                    VALUES (:roomNumber, :hoteNumber, :roomType, :roomImage)";
            command.Parameters.Add("roomNUmber", room.number);
            command.Parameters.Add("hotelNumber", room.hotel.licenseNumber);
            command.Parameters.Add("roomType", room.type.name);
            command.Parameters.Add("roomImage", room.image.GetByteImage());

            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
            return true;
        }

        public bool AddBooking(Booking booking)
        {
            /// <summary>
            /// Adds a booking to the database and adds the associating Define_Booking object.
            /// </summary>
            if (!IsRoomAvailable(booking.bookingRoom, booking.startDate, booking.endDate))
                return false;

            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = @"INSERT INTO Booking
                                    (booking_number, start_date, end_date, number_of_guests, user_name,
                                        meal_plan, LICENCE_NUMBER)
                                    VALUES (:bookingNumber, :sDate, :eDate, :guests, :userName,
                                    :mealPlan, :licenseNumber)";
            command.Parameters.Add("bookingNumber", booking.number);
            command.Parameters.Add("sDate", (OracleDate)booking.startDate);
            command.Parameters.Add("eDate", (OracleDate)booking.endDate);
            command.Parameters.Add("guests", booking.numberOfGuests);
            command.Parameters.Add("userName", booking.bookingUser.username);
            command.Parameters.Add("mealPlan", booking.bookingMealPlan.name);
            command.Parameters.Add("licenseNumber", booking.bookingRoom.hotel.licenseNumber);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }

            command.CommandText = @"INSERT INTO Define_Booking
                                    (website_name, hotel_license_number, room_number, booking_number)
                                    VALUES (:websiteName, :hn, :rn, :bn)";
            command.Parameters.Clear();
            command.Parameters.Add("websiteName", booking.bookingWebsite.name);
            command.Parameters.Add("hn", booking.bookingRoom.hotel.licenseNumber);
            command.Parameters.Add("rn", booking.bookingRoom.number);
            command.Parameters.Add("bn", booking.number);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            { 
                return false;
            }
            return true;
        }

        public bool AddRoomView(Room room, RoomView view)
        {
            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = $@"INSERT INTO Room_Views
                                     (license_number, room_number, room_view)
                                     VALUES ({room.hotel.licenseNumber}, {room.number}, '{view.view}')";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }
            return true;
        }

        public bool RegisterUser(User user, string password)
        {
            /// <summary>
            /// Adds a user object to the database.
            /// </summary>
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = $@"INSERT INTO Credit_Card
                                    (credit_card_number, cvv, expiration_date)
                                    VALUES (:serialN, :cvv, :eDate)";
            command.Parameters.Add("serialN", user.userCreditCard.cardSerial);
            command.Parameters.Add("cvv", user.userCreditCard.cvv);
            command.Parameters.Add("eDate", user.userCreditCard.expirationDate);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }
            command.Parameters.Clear();
            command.CommandText = @"INSERT INTO Website_User
                                    (user_name, email, name, password, user_category, credit_card_number)
                                    VALUES (:userName,
                                            :userEmail,
                                            :name,
                                            :pswd,
                                            :category,
                                            :serial)";
            command.Parameters.Add("userName", user.username);
            command.Parameters.Add("userEmail", user.email);
            command.Parameters.Add("name", user.name);
            command.Parameters.Add("pswd", PasswordHasher.Hash(password));
            command.Parameters.Add("category", "Basic");
            command.Parameters.Add("serial", user.userCreditCard.cardSerial);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }
            return true;
        }

        public User LogUser(string userName, string password)
        {
            /// <summary>
            /// Checks if user is registered and password is valid.
            /// <returns>
            /// User object if user is valid.
            /// </returns>
            /// <returns>
            /// null if user isn't found or password is invalid.
            /// </returns>
            /// </summary>
            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = $@"SELECT user_name, password
                                    FROM Website_User
                                    WHERE user_name = '{userName}'";
            OracleDataReader reader = command.ExecuteReader();
            if (reader.HasRows == false)
                return null;

            if (!PasswordHasher.Verify(password, reader["password"].ToString()))
                return null;

            return GetUser(userName);
        }

        public int GetBookingId()
        {
            /// <summary>
            /// Returns the next booking id available.
            /// </summary>
            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = @"SELECT Booking_Number
                                    FROM Booking
                                    ORDER BY booking_number DESC";
            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = int.Parse(reader["booking_number"].ToString());
                return id + 1;
            }
            return 1;
        }

        /*
         * Delete Methods
         */

        public bool DeleteBooking(int booking_number)
        {
            /// <summary>
            /// Deletes booking table and the corresponding Define_Booking and Review
            /// having the given booking number.
            /// </summary>
            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            // Delete the booking child records
            command.CommandText = $@"DELETE FROM Define_Booking
                                     WHERE Booking_Number = {booking_number}";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }

            command.CommandText = $@"DELETE FROM Review
                                     WHERE booking_number = {booking_number}";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }

            // Delete the booking record itself
            command.CommandText = $@"DELETE FROM Booking
                                     WHERE Booking_Number = {booking_number}";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }
            return true;
        }

        public bool DeleteUser(User user)
        {
            /// <summary>
            /// Deletes a user from the database and its corresponding credit card and bookings.
            /// </summary>
            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            // Delete child records
            command.CommandText = $@"SELECT booking_number
                                     FROM booking
                                     WHERE user_name = '{user.username}'";
            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int num = Int32.Parse(reader["booking_number"].ToString());
                if (!DeleteBooking(num))
                    return false;
            }

            // Delete the user records itself and the corresponding credit card
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Delete_User";    // Deletes user and credit card
            command.Parameters.Add("uName", user.username);
            command.Parameters.Add("creditNumber", user.userCreditCard.cardSerial);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }

            return true;
        }

        public bool DeleteReview(Review review)
        {
            /* Disconnected Mode */
            /// <summary>
            /// Removes a booking review according to the booking number.
            /// </summary>
            OracleDataAdapter adapter = new OracleDataAdapter(
                $@"SELECT * FROM Review WHERE booking_number = {review.bookingNumber}",
                oracleConnectionString);
            DataTable datatable = new DataTable();
            adapter.Fill(datatable);
            datatable.Rows[0].Delete();

            OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
            try
            {
                adapter.Update(datatable);
            }
            catch (OracleException)
            {
                return false;
            }
            return true;
        }

        public bool DeleteWebsite(Website website)
        {
            /// <summary>
            /// Deletes a website and its associated rooms and bookings.
            /// </summary>
            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            // Delete child records
            command.CommandText = $@"DELETE FROM Room_Price
                                     WHERE website_name = '{website.name}'";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }

            // Get associated bookings to invoke DeleteBooking
            command.CommandText = $@"SELECT Booking_Number
                                     FROM Define_Booking
                                     WHERE website_name = '{website.name}'";
            OracleDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int bookingNumber = int.Parse(reader["booking_number"].ToString());
                if (!DeleteBooking(bookingNumber))
                    return false;
            }

            command.CommandText = $@"DELETE FROM Website
                                     WHERE name = '{website.name}'";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }
            return true;
        }

        public bool DeleteRoom(Room room)
        {
            List<Booking> roomBookings = GetRoomBookings(room);
            foreach(Booking booking in roomBookings)
            {
                if (!DeleteBooking(booking.number))
                    return false;
            }

            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = $@"DELETE FROM room_views
                                     WHERE room_number = {room.number}
                                     AND license_number ={room.hotel.licenseNumber}";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                return false;
            }

            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = $@"DELETE FROM room_price
                                     WHERE room_number = {room.number}
                                     AND license_number ={room.hotel.licenseNumber}";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                return false;
            }

            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = $@"DELETE FROM room
                                     WHERE room_number = {room.number}
                                     AND hotel_license_number ={room.hotel.licenseNumber}";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException  e)
            {
                return false;
            }

            return true;

        }

        public bool DeleteHotel(Hotel hotel)
        {
            List<Room> hotelRooms = GetHotelRooms(hotel.licenseNumber);
            foreach(Room room in hotelRooms)
            {
                if (!DeleteRoom(room))
                    return false;
            }

            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = $@"DELETE FROM hotel_facilities
                                     WHERE license_number ={hotel.licenseNumber}";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                return false;
            }

            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = $@"DELETE FROM meal_plan
                                     WHERE hotel_license_number ={hotel.licenseNumber}";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                return false;
            }


            command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;

            command.CommandText = $@"DELETE FROM hotel
                                     WHERE license_number ={hotel.licenseNumber}";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                return false;
            }

            return true;
        }

        /*
         * Update Methods
         */

        public void UpdateHotel(Hotel hotel)
        {
            /// <summary>
            /// Updates Hotel's name, city and country.
            /// </summary>
            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Update_Hotel";

            command.Parameters.Add("lNumber", hotel.licenseNumber);
            command.Parameters.Add("name", hotel.name);
            command.Parameters.Add("city", hotel.location.city);
            command.Parameters.Add("city", hotel.location.country);

            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException e)
            {
                MessageBox.Show(e.ToString());
                return;
            }
        }

        public bool UpdateRoom(Room room)
        {
            /// <summary>
            /// Updates a room table based on given room and adds the given view
            /// to the room's views list.
            /// </summary>
            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = $@"UPDATE Room
                                     SET room_type = '{room.type.name}'
                                     WHERE room_number = {room.number}
                                     AND hotel_license_number = {room.hotel.licenseNumber}";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }
            return true;
        }

        public bool UpdateReview(Review review)
        {
            /// <summary>
            /// Updates an existing review based on the given object.
            /// </summary>
            OracleDataAdapter adapter = new OracleDataAdapter("SELECT * FROM Review", oracleConnectionString);
            DataTable datatable = new DataTable();
            adapter.Fill(datatable);
            DataRow[] rows = datatable.Select();

            OracleCommandBuilder builder = new OracleCommandBuilder(adapter);
            foreach(var row in rows)
            {
                if (row["booking_number"].ToString() == review.bookingNumber.ToString())
                {
                    row["description"] = review.description;
                    row["rating"] = review.rating;
                    adapter.Update(rows);
                    return true;
                }
            }
            return false;
        }

        public bool UpdateWebsite(Website website)
        {
            /// <summary>
            /// Updates a website rating.
            /// </summary>
            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = $@"UPDATE Website
                                     SET rating = {website.rating}
                                     WHERE name = '{website.name}'";
            try
            {
                command.ExecuteNonQuery();
            }
            catch(OracleException)
            {
                return false;
            }
            return true;
        }

        public bool UpdateRoomPrice(Room room, Website website, int price)
        {
            /// <summary>
            /// Updates a room price relative to the hotel and website.
            /// </summary>
            OracleCommand command = new OracleCommand();
            command.Connection = connection;
            command.CommandType = CommandType.Text;
            command.CommandText = $@"UPDATE Room_Price
                                     SET price = {price}
                                     WHERE website_name = '{website.name}'
                                     AND license_number = {room.hotel.licenseNumber}
                                     AND room_number = {room.number}";
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OracleException)
            {
                return false;
            }
            return true;
        }
        
        public bool isAdmin(string userName, string password)
        {
            command = new OracleCommand();
            command.CommandText = @"select *
                                   from admin
                                   where user_name = :username
                                    AND password = :password";
            command.Connection = connection;
            command.Parameters.Add("username", userName);
            command.Parameters.Add("password", password);

            OracleDataReader reader = command.ExecuteReader();
            return reader.HasRows == true;
        }
    }
}

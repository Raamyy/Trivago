using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data;
using System.Windows;


namespace Trivago.Models
{
    public class DataModels
    {
        private string oracleConnectionString;
        private OracleConnection connection;
        private OracleCommand command;
        public DataModels()
        {
            oracleConnectionString = "data source = orcl; user id = scott; password = tiger;";
            connection = new OracleConnection(oracleConnectionString);
            connection.Open();
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
            Location location = new Location(getPlacesOfInterest(country, city), country, city);
            return location;
        }

        public List<PlaceOfIntrest> getPlacesOfInterest(String country, String city)
        {
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

        public List<Room> GetRooms(List<Location> locations, int guestsCount,
            DateTime startDate, DateTime endDate)
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
                    String s = reader["license_number"].ToString();
                    Hotel hotel = GetHotel(Int32.Parse(reader["license_number"].ToString()));
                    int roomNumber = Int32.Parse(reader["room_number"].ToString());
                    int hotelNumber = Int32.Parse(reader["license_number"].ToString());
                    List<RoomView> views = GetViews(roomNumber, hotelNumber);
                    byte[] image = (byte[]) reader["room_image"];

                    room = new Room(roomNumber, hotel, GetRoomType(guestsCount),
                                    new CustomImage(image), views);

                    // Check if the room is available in the given dates
                    if (isRoomAvailable(room, startDate, endDate))
                        rooms.Add(room);
                }
                reader.Close();
            }
            return rooms;
        }

        bool isRoomAvailable(Room room, DateTime startDate, DateTime endDate)
        {
            /// <summary>
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

        public void addImage()
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

        public void addHotel(Hotel hotel)
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
            addFacilities(hotel.licenseNumber, hotel.facilities);
            addMealPlans(hotel.licenseNumber, hotel.mealPlans);
        }

        private void addFacilities(int hotelLicenseNumber, List<HotelFacility> facilities)
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

        private void addMealPlans(int hotelLicenseNumber, List<MealPlan> meals)
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

        public void addLocation(Location location)
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
            addPlacesOfInterest(location.placesOfIntrest);
        }

        private void addPlacesOfInterest(List<PlaceOfIntrest> places)
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

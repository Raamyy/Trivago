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
        public DataModels()
        {
            oracleConnectionString = "data source = orcl; user id = scott; password = tiger;";
        }

        public List<HotelFacility> GetFacilities(int hotelLicense)
        {
            connection = new OracleConnection(oracleConnectionString);
            connection.Open();
            OracleCommand command = new OracleCommand();
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
            connection = new OracleConnection(oracleConnectionString);
            connection.Open();
            OracleCommand command = new OracleCommand();
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
            connection = new OracleConnection(oracleConnectionString);
            connection.Open();
            OracleCommand command = new OracleCommand();
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
            connection = new OracleConnection(oracleConnectionString);
            connection.Open();
            OracleCommand command = new OracleCommand();
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
            connection = new OracleConnection(oracleConnectionString);
            connection.Open();

            OracleCommand command = new OracleCommand();
            command.Connection = connection;
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
            connection = new OracleConnection(oracleConnectionString);
            connection.Open();
            OracleCommand command = new OracleCommand();
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
            connection = new OracleConnection(oracleConnectionString);
            connection.Open();

            OracleCommand command = new OracleCommand();
            command.Connection = connection;

            List<Room> rooms = new List<Room>();
            foreach (Location location in locations)
            {
                // TODO : Add date based filtration
                command.CommandText = @"SELECT Room.*, Hotel.license_number,
                                        FROM Room, Hotel, Room_Type
                                        WHERE Room.hotel_license_number = Hotel.license_number
                                        AND Room.room_type = room_type.type_name
                                        AND Hotel.country = :country AND Hotel.city = :city
                                        AND room_type.number = :guests";

                command.CommandType = CommandType.Text;
                command.Parameters.Add("country", location.country);
                command.Parameters.Add("city", location.city);
                command.Parameters.Add("guests", guestsCount);

                OracleDataReader reader = command.ExecuteReader();
                Room room;
                while (reader.Read())
                { 
                    Hotel hotel = GetHotel((int) reader["license_number"]);
                    int roomNumber = (int) reader["room_number"];
                    int hotelNumber = (int)reader["license_number"];
                    List<RoomView> views = GetViews(roomNumber, hotelNumber);
                    byte[] image = (byte[]) reader["room_image"];
                    room = new Room(roomNumber, hotel, GetRoomType(guestsCount),
                        new CustomImage(image), views);
                    rooms.Add(room);
                }
                reader.Close();
            }
            return rooms;
        }
    }
}

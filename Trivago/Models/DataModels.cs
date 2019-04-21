using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data;

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
                command.CommandText = "SELECT Room.* " +
                "FROM Room, Hotel " +
                "WHERE Room.hotel_license_number = Hotel.license_number" +
                "AND Hotel.country = :country AND Hotel.city = :city";

                command.CommandType = CommandType.Text;
                command.Parameters.Add("country", location.Country);
                command.Parameters.Add("city", location.City);

                OracleDataReader reader = command.ExecuteReader();
                Room room = new Room();
                while(reader.Read())
                {
                    // TODO : Parse room objects and them to rooms
                    // TODO : Retreive room's Hotel data as well
                }
                reader.Close();
            }
            

            return new List<Room>();
        }
    }
}

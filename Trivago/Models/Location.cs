using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class Location
    {
        public List<PlaceOfIntrest> placesOfIntrest;
        public String country;
        public String city;

        public Location(List<PlaceOfIntrest> places, String country, String city)
        {
            this.city = city;
            this.country = country;
            this.placesOfIntrest = places;
        }
        public override string ToString()
        {
            return country + ", " + city;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivago.Front_End;

namespace Trivago.Models
{
    public class PlaceOfIntrest
    {
        public String name;
        public CustomImage image;
        public String country;
        public String city;
        public PlaceOfIntrest(String name, CustomImage image)
        {
            this.name = name;
            this.image = image;
        }
    }
}

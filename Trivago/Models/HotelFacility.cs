using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivago.Front_End;

namespace Trivago.Models
{
    public class HotelFacility
    {
        public String name;
        public CustomImage image;
        
        public HotelFacility(String name, CustomImage image)
        {
            this.name = name;
            this.image = image;
        }
    }
}

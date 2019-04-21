using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class RoomType
    {
        public String name;
        public int maxGuests;

        public RoomType(String name, int maxGuests)
        {
            this.name = name;
            this.maxGuests = maxGuests;
        }
    }
}

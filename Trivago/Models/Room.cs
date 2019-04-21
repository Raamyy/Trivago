using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class Room
    {
        public int Number;
        public Hotel RoomHotel;
        public RoomType Type;
        public CustomImage Image;
        public List<RoomView> Views;
    }
}

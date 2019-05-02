using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivago.Front_End;

namespace Trivago.Models
{
    public class Room
    {
        public int number;
        public Hotel hotel;
        public RoomType type;
        public CustomImage image;
        public List<RoomView> views;

        public Room(int number, Hotel hotel, RoomType type,
            CustomImage image, List<RoomView> views)
        {
            this.number = number;
            this.hotel = hotel;
            this.type = type;
            this.views = views;
            this.image = image;
        }
        public override string ToString()
        {
            return $"{hotel.name} (#{number})";
        }
    }
}

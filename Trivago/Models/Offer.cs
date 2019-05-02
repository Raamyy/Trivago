using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class Offer
    {
        public Website website;
        public Room room;
        public int price;

        public Offer()
        {
            website = null;
            room = null;
            price = 0;
        }
        public Offer(Website website, Room room, int price)
        {
            this.website = website;
            this.room = room;
            this.price = price;
        }
    }
}

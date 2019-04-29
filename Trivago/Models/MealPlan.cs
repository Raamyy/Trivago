using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class MealPlan
    {
        public String name;
        public int price;

        public MealPlan(String name, int price)
        {
            this.name = name;
            this.price = price;
        }

        public override string ToString()
        {
            return name + " , " + price;
        }
    }
}

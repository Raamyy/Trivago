using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class User
    {
        public String username;
        public String email;
        public String name;
        public UserCategory category;
        public CreditCard userCreditCard;

        public User(string username, string email, string name, UserCategory category, CreditCard userCreditCard)
        {
            this.username = username;
            this.email = email;
            this.name = name;
            this.category = category;
            this.userCreditCard = userCreditCard;
        }
    }
}

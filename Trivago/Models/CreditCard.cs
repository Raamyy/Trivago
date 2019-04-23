using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class CreditCard
    {
        public String CardSerial;
        public int CVV;
        public DateTime ExpirationDate;

        public CreditCard(string cardSerial, int cVV, DateTime expirationDate)
        {
            CardSerial = cardSerial;
            CVV = cVV;
            ExpirationDate = expirationDate;
        }
    }
}

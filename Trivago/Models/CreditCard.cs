using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public class CreditCard
    {
        public String cardSerial;
        public int cvv;
        public DateTime expirationDate;

        public CreditCard(string cardSerial, int cvv, DateTime expirationDate)
        {
            this.cardSerial = cardSerial;
            this.cvv = cvv;
            this.expirationDate = expirationDate;
        }
    }
}

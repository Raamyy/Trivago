using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivago.Models
{
    public static class BackEndHelper
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsNumber(string number)
        {
            for(int i = 0; i < number.Length; i++)
            {
                if (!(number[i] >= '0' && number[i] <= '9'))
                    return false;
            }
            return true;
        }
    }
}

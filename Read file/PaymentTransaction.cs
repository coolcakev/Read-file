using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_file
{
    public class PaymentTransaction
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public decimal Payment { get; set; }
        public DateTime Date { get; set; }
        public long AccountNumber { get; set; }
        public string Service { get; set; }

        public string TakeCity()
        {
            var city = Address.Split(",").FirstOrDefault();
            if (city == null)
            {
                return string.Empty;
            }
            city = city.Trim();
            return city;
        }
    }

}

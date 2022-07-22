using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_file
{
    public class City
    {
        public string Name { get; set; }
        public IEnumerable<Service> Services { get; set; }
        public decimal Total { get; set; }

    }

    public class Service
    {
        public string Name { get; set; }
        public IEnumerable<Payer> Payers { get; set; }
        public decimal Total { get; set; }
    }

    public class Payer
    {
        public string Name { get; set; }
        public decimal Payment { get; set; }
        public DateTime Date { get; set; }
        public long AccountNumber { get; set; }
    }

}

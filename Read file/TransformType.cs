using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_file
{
    public class TransformType
    {
        public string Name { get; set; }
        public List<Service> Services { get; set; }= new List<Service>(){ };
        public decimal Total { get; set; }

    }

    public class Service
    {
        public string Name { get; set; }
        public List<Payer> Payers { get; set; } = new List<Payer>() { };
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

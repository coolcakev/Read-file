using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_file
{
    public  class Meta
    {
        public int ParsedFiles { get; set; }
        public int ParsedLines { get; set; }
        public int FoundErrors { get; set; }
        public List<string> InvalidFiles { get; set; }=new List<string>();
    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_file
{
    internal class Configuration
    {
        public string PathPayment { get; set; }
        public string FolderOfTransformType { get; set; }
        public IEnumerable<string> AllowFormat { get; set; }
        
    }
}

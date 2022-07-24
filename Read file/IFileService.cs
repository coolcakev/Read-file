using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_file
{
    internal interface IFileService
    {  
        Meta Meta { get; }
        IEnumerable<PaymentTransaction> Read(string file);
        Dictionary<string, List<TransformType>> ReadFiles(IEnumerable<string> files);
        PaymentTransaction CreatePaymentTransaction(object obj);
    }
}

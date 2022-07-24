using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_file
{
    public class TransformTypeService
    {
        public List<TransformType> CreateTransformType(IEnumerable<PaymentTransaction> paymentTransactions)
        {
            List<TransformType> transformTypes = new List<TransformType>();
            var groupedByCity = paymentTransactions.GroupBy(x => x.TakeCity());
            foreach (var groupByCity in groupedByCity)
            {
                var transformType = new TransformType();
                transformType.Name = groupByCity.Key;

                var groupedByService = groupByCity.GroupBy(x => x.Service);
                foreach (var groupByService in groupedByService)
                {
                    var service = new Service();
                    service.Name = groupByService.Key;

                    foreach (var group in groupByService)
                    {
                        var payer = new Payer()
                        {
                            Name = $"{group.FirstName} {group.LastName}",
                            AccountNumber = group.AccountNumber,
                            Date = group.Date,
                            Payment = group.Payment,
                        };
                        service.Payers.Add(payer);
                        service.Total += group.Payment;
                    }
                    transformType.Total += service.Total;
                    transformType.Services.Add(service);
                }
                transformTypes.Add(transformType);
            }
            return transformTypes;
        }
    }
}

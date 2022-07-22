using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_file
{
    public class CSVService : IFileService
    {
        public Meta Meta { get; }
        public CSVService()
        {
            Meta = new Meta();
        }

        public PaymentTransaction CreatePaymentTransaction(object obj)
        {
            PaymentTransaction paymentTransaction = null;
            var csvReader = obj as CsvReader;
            if (csvReader == null)
            {
                return paymentTransaction;
            }
            try
            {
                var firstName = csvReader.GetField(0).Trim();
                if (string.IsNullOrWhiteSpace(firstName))
                {
                    return paymentTransaction;
                }
                var lastName = csvReader.GetField(1).Trim();
                if (string.IsNullOrWhiteSpace(lastName))
                {
                    return paymentTransaction;
                }
                var address = $"{csvReader.GetField(2)},{csvReader.GetField(3).Trim()},{csvReader.GetField(4)}".Trim();
                address = address.Substring(1, address.Length - 2);

                var paymentString = csvReader.GetField(5).Trim().Replace('.', ','); ;
                var isCorrectPayment = Decimal.TryParse(paymentString, NumberStyles.AllowDecimalPoint, null, out decimal payment);
                if (!isCorrectPayment)
                {
                    return paymentTransaction;
                }

                var dateString = csvReader.GetField(6).Trim();
                var isCorrectDate = DateTime.TryParseExact(dateString, "yyyy-dd-MM", null, DateTimeStyles.AllowWhiteSpaces, out DateTime date);
                if (!isCorrectDate)
                {
                    return paymentTransaction;
                }

                var accountNumberString = csvReader.GetField(7).Trim();
                if (!long.TryParse(accountNumberString, out long accountNumber))
                {
                    return paymentTransaction;
                }

                var service = csvReader.GetField(8).Trim();
                if (string.IsNullOrWhiteSpace(service))
                {
                    return paymentTransaction;
                }
                paymentTransaction = new PaymentTransaction();
                paymentTransaction.Service = service;
                paymentTransaction.Date = date;
                paymentTransaction.AccountNumber = accountNumber;
                paymentTransaction.LastName = lastName;
                paymentTransaction.FirstName = firstName;
                paymentTransaction.Address = address;
                paymentTransaction.Payment = payment;


            }
            catch (Exception)
            {
                return null;
            }


            return paymentTransaction;
        }

        public IEnumerable<PaymentTransaction> Read(string file)
        {
            Meta.ParsedFiles++;
            var paymentTransactions = new List<PaymentTransaction>();
            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
            };

            using var streamReader = File.OpenText(file);
            using var csvReader = new CsvReader(streamReader, csvConfig);
            var hasError = false;
            Meta.ParsedLines--;
            while (csvReader.Read())
            {
                if (csvReader.Context.Parser.Row == 1)
                {
                    continue;
                }
                Meta.ParsedLines++;

                var paymentTransactiot = CreatePaymentTransaction(csvReader);
                if (paymentTransactiot == null)
                {
                    Meta.FoundErrors++;
                    hasError = true;
                    continue;
                }
                paymentTransactions.Add(paymentTransactiot);
            }
            if (hasError)
            {
                Meta.InvalidFiles.Add(file);
            }
            return paymentTransactions;
        }

        public List<PaymentTransaction> ReadFiles(IEnumerable<string> files)
        {
            var paymentTransactions = new List<PaymentTransaction>();
            foreach (var file in files)
            {
                var partialPaymentTransactions = Read(file);
                paymentTransactions.AddRange(partialPaymentTransactions);
            }
            return paymentTransactions;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Read_file
{
    public class TxtService : IFileService
    {
        private readonly TransformTypeService _transformTypeService;
        public Meta Meta { get; }
        public TxtService()
        {
            Meta = new Meta();
            _transformTypeService = new TransformTypeService();
        }
        public PaymentTransaction CreatePaymentTransaction(object obj)
        {
            var line = obj as string;
            if (string.IsNullOrWhiteSpace(line))
            {
                return null;
            }
            var transaction = new PaymentTransaction();

            Regex valueForOnlyContainsLetter = new Regex(@"[A-Za-z]");

            Regex firstNameRegex = new Regex(@"[A-Za-z ]+,");
            var fistname = MatchString(firstNameRegex, line, valueForOnlyContainsLetter);
            if (string.IsNullOrWhiteSpace(fistname))
            {
                return null;
            }
            transaction.FirstName = fistname;

            Regex lastNameRegex = new Regex(@",[ A-Za-z]+");
            var lastname = MatchString(lastNameRegex, line, valueForOnlyContainsLetter);
            if (string.IsNullOrWhiteSpace(lastname))
            {
                return null;
            }
            transaction.LastName = lastname;

            Regex addressRegex = new Regex(@"“[A-Za-z ]+,[A-Za-z ]+ +[0-9 ]+,[0-9 ]+”");
            var match = addressRegex.Matches(line).FirstOrDefault();
            if (match == null)
            {
                return null;
            }
            var address = match.Value.Substring(1, match.Value.Length - 2);

            transaction.Address = address;

            Regex doubleValue = new Regex(@"[0-9]+(.[0-9]+|)");
            Regex paymentRegex = new Regex(@"”,[0-9 ]+(.[0-9]+|),");
            var paymentString = MatchString(paymentRegex, line, doubleValue)?.Replace('.', ',');
            var isDoubleParse = Decimal.TryParse(paymentString, NumberStyles.AllowDecimalPoint, null, out decimal result);
            if (!isDoubleParse)
            {
                return null;
            }
            transaction.Payment = result;

            Regex dateRegex = new Regex(@",[ 0-9]+-[ 0-9]+-[ 0-9]+,");
            var dateString = MatchString(dateRegex, line);
            var isCorrectDate = DateTime.TryParseExact(dateString, "yyyy-dd-MM", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out DateTime date);

            if (!isCorrectDate)
            {
                return null;
            }
            transaction.Date = date;

            Regex accountNumberRegex = new Regex(@",[ 0-9]+,");
            var accountNumberString = MatchString(accountNumberRegex, line);
            var isCorrectAccountNumber = long.TryParse(accountNumberString, out long resultNumber);

            if (!isCorrectAccountNumber)
            {
                return null;
            }
            transaction.AccountNumber = resultNumber;

            Regex serviceRegex = new Regex(@",[ A-Za-z]+$");
            var service = MatchString(serviceRegex, line);

            if (string.IsNullOrWhiteSpace(service))
            {
                return null;
            }
            transaction.Service = service;

            return transaction;
        }

        public IEnumerable<PaymentTransaction> Read(string file)
        {
            Meta.ParsedFiles++;
            var paymentTransactions = new List<PaymentTransaction>();
            var lines = File.ReadAllLines(file);
            var hasError = false;

            foreach (var line in lines)
            {
                Meta.ParsedLines++;
                var paymentTransaction = CreatePaymentTransaction(line);
                if (paymentTransaction == null)
                {
                    Meta.FoundErrors++;
                    hasError = true;
                    continue;
                }
                paymentTransactions.Add(paymentTransaction);
            }
            if (hasError)
            {
                Meta.InvalidFiles.Add(file);
            }
            return paymentTransactions;
        }


        public Dictionary<string, List<TransformType>> ReadFiles(IEnumerable<string> files)
        {
            var filesTransformTypes = new Dictionary<string, List<TransformType>>();

            foreach (var file in files)
            {
                var paymentTransactions = Read(file);
                var tranformTypes = _transformTypeService.CreateTransformType(paymentTransactions);
              
                filesTransformTypes.Add(file, tranformTypes);

            }
            return filesTransformTypes;
        }
        private string MatchString(Regex regex, string text, Regex value = null)
        {
            var match = regex.Matches(text).FirstOrDefault();
            if (match == null)
            {
                return null;
            };
            var trimedMatch = match.Value.Trim(',', ' ');
            if (value == null)
            {
                return trimedMatch;
            }
            var symbols = value.Matches(trimedMatch).Select(x => x.Value);
            var result = string.Join("", symbols);
            return result;
        }
    }
}

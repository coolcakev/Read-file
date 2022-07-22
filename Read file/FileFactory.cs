using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Read_file
{
    internal class FileFactory
    {
        private static readonly Configuration _configuration;

        static FileFactory()
        {
            var cache = GlobalProperty.MemoryCache;
            _configuration = cache.Get<Configuration>(CachType.Configuration);
        }
        public static FileFactory GetFileFactory()
        {
            if (!Directory.Exists(_configuration?.PathPayment))
            {
                Console.WriteLine("Invalid path payment");
                return null;
            }
            return new FileFactory();

        }
        public IFileService GetInstance(string fileType)
        {
            var isEnumFileType = Enum.TryParse(typeof(FileType), fileType, out object result);
            if (!isEnumFileType)
            {
                Console.WriteLine("Invalid file type");
                return null;
            }

            switch (result)
            {
                case FileType.Csv:
                    return new CSVService();
                case FileType.Txt:
                    return new TxtService();
                default:
                    return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Read_file
{
    public class FIleService
    {
        private readonly FileFactory _fileFactory;
        private IFileService _fileService;
        private readonly ConfigurationService _configurationService;
        private readonly Configuration _configuration;
        private readonly SimpleMemoryCache _simpleMemoryCache;

        public Meta Meta;

        public FIleService()
        {
            Meta = new Meta();
            var fileFactory = FileFactory.GetFileFactory();
            if (fileFactory == null)
            {
                Console.WriteLine("file factory is null in FIleService constructor");
                return;
            }
            _fileFactory = fileFactory;
            _simpleMemoryCache = GlobalProperty.MemoryCache;
            _configurationService = new ConfigurationService();
            _configuration = _configurationService.Configuration();
        }
        public async Task SaveTransFormType(Dictionary<string, List<TransformType>> fileTransformTypes)
        {
            var location = _configurationService.GetPathToFolderOfTransformType();
            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }
            location = _configurationService.GetCurrentSubFolder();
            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }
            var countfile = _simpleMemoryCache.Get<int>(CachType.CountFiles);

            foreach (var fileTransformType in fileTransformTypes)
            {
                var filename = $"output{++countfile}.json";
                var pathToFile = Path.Combine(location, filename);
                await using FileStream createStream = File.Create(pathToFile);
                await JsonSerializer.SerializeAsync(createStream, fileTransformType.Value);
            }

            _simpleMemoryCache.Set(CachType.CountFiles, countfile);
        }

        public Dictionary<string, List<TransformType>> ReadFiles()
        {
            var paymentTransactions = new Dictionary<string, List<TransformType>>();
            if (_fileFactory == null)
            {
                Console.WriteLine("file factory is null in ReadFiles Function");
                return new Dictionary<string, List<TransformType>>();
            }

            foreach (var allowFormat in _configuration.AllowFormat)
            {
                var filesByType = Directory.EnumerateFiles(_configuration.PathPayment, $"*.{allowFormat}");
                var partialPaymentTransactions = ReadFile(filesByType, allowFormat);

                paymentTransactions = paymentTransactions.Concat(partialPaymentTransactions).ToDictionary(x => x.Key, x => x.Value);
                       
            }
            return paymentTransactions;
        }
        private Dictionary<string, List<TransformType>> ReadFile(IEnumerable<string> files, string type)
        {

            _fileService = _fileFactory.GetInstance(type);
            if (_fileService == null)
            {
                Console.WriteLine("Cannot find file service");
                return null;
            }
            var paymentTransactions = _fileService.ReadFiles(files);

            Meta.ParsedFiles += _fileService.Meta.ParsedFiles;
            Meta.ParsedLines += _fileService.Meta.ParsedLines;
            Meta.FoundErrors += _fileService.Meta.FoundErrors;
            Meta.InvalidFiles.AddRange(_fileService.Meta.InvalidFiles);


            return paymentTransactions;
        }
    }
}

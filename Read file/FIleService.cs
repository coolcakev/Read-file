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
                Environment.Exit(0);
                return;
            }
            _fileFactory = fileFactory;
            _simpleMemoryCache = GlobalProperty.MemoryCache;
            _configurationService = new ConfigurationService();
            _configuration = _configurationService.Configuration();
        }
        private void CheckCurrentFolderExists()
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

        }
        public async Task SaveMetaInLogFile()
        {
            CheckCurrentFolderExists();
            var location = _configurationService.GetCurrentSubFolder();
            var fileName = "meta.log";
            var pathToFile = Path.Combine(location, fileName);

            var content = $"parsed_files: {Meta.ParsedFiles}\n";
            content += $"parsed_lines: {Meta.ParsedLines}\n";
            content += $"found_errors: {Meta.FoundErrors}\n";
            content += $"invalid_files: {JsonSerializer.Serialize(Meta.InvalidFiles)}\n";

            await using FileStream createStream = File.Create(pathToFile);
            var contentByte = Encoding.UTF8.GetBytes(content);
            await createStream.WriteAsync(contentByte, 0, contentByte.Length);
        }
        public async Task SaveTransFormType(Dictionary<string, List<TransformType>> fileTransformTypes)
        {
            CheckCurrentFolderExists();
            var location = _configurationService.GetCurrentSubFolder();

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
                var unreadFiles = GetUnReadFiles(filesByType);
                if (unreadFiles.Count() == 0)
                {
                    continue;
                }

                var partialPaymentTransactions = ReadFile(unreadFiles, allowFormat);

                var readFiles = GetReadFiles(partialPaymentTransactions.Select(x => x.Key));
                _simpleMemoryCache.Set(CachType.ReadFile, readFiles);

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
            Meta.InvalidFiles.AddRange(_fileService.Meta.InvalidFiles.Select(x => x.Split("\\").LastOrDefault()));


            return paymentTransactions;
        }
        private IEnumerable<string> GetReadFiles(IEnumerable<string> files)
        {
            var readFiles = _simpleMemoryCache.Get<List<string>>(CachType.ReadFile) ?? new List<string>();
            readFiles.AddRange(files);
            return readFiles;
        }
        private IEnumerable<string> GetUnReadFiles(IEnumerable<string> files)
        {
            var readFiles = _simpleMemoryCache.Get<IEnumerable<string>>(CachType.ReadFile);
            if (readFiles == null || readFiles.Count() == 0)
            {
                return files;
            }
            files = files.Where(x => !readFiles.Contains(x));
            return files;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_file
{
    public class FIleService
    {
        private readonly FileFactory _fileFactory;
        private IFileService _fileService;
        private readonly Configuration _configuration;

        public Meta Meta;

        public FIleService()
        {
            Meta= new Meta();
            var fileFactory = FileFactory.GetFileFactory();
            if (fileFactory == null)
            {
                Console.WriteLine("file factory is null in FIleService constructor");
                return;
            }
            _fileFactory = fileFactory;
            var cache = GlobalProperty.MemoryCache;
            _configuration = cache.Get<Configuration>(CachType.Configuration);
        }
        public void ReadFiles()
        {
            if (_fileFactory == null)
            {
                Console.WriteLine("file factory is null in ReadFiles Function");
                return;
            }
            foreach (var allowFormat in _configuration.AllowFormat)
            {
                var filesByType = Directory.EnumerateFiles(_configuration.PathPayment, $"*.{allowFormat}");
                ReadFile(filesByType, allowFormat);
            }
        }
        private void ReadFile(IEnumerable<string> files, string type)
        {
            _fileService = _fileFactory.GetInstance(type);
            if (_fileService == null)
            {
                Console.WriteLine("Cannot find file service");
                return;
            }
            _fileService.ReadFiles(files);

            Meta.ParsedFiles += _fileService.Meta.ParsedFiles;
            Meta.ParsedLines += _fileService.Meta.ParsedLines;
            Meta.FoundErrors += _fileService.Meta.FoundErrors;
            Meta.InvalidFiles.AddRange(_fileService.Meta.InvalidFiles);
        }
    }
}

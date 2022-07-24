using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Read_file
{
    internal class ConfigurationService
    {
        private readonly Configuration _configuration;

        public ConfigurationService()
        {
            var cache = GlobalProperty.MemoryCache;
            _configuration = cache.Get<Configuration>(CachType.Configuration);
        }
        public Configuration Configuration() => _configuration;
       
        public string GetPathToFolderOfTransformType()
        {
            var indexOfLastSlash = _configuration.PathPayment.LastIndexOf('\\');
            var location = _configuration.PathPayment.Substring(0, indexOfLastSlash);
            var locattionToTransformFolder = Path.Combine(location, _configuration.FolderOfTransformType);
            return locattionToTransformFolder;
        }
        public string GetCurrentSubFolder()
        {
            var location = GetPathToFolderOfTransformType();
            location = Path.Combine(location, DateTime.Now.ToString("MM-dd-yyyy"));
            return location;
        }
    }
}

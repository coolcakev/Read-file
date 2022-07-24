using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Read_file
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var cache = GlobalProperty.MemoryCache;

            string fileName = "config.json";
            string jsonString = File.ReadAllText(fileName);
            var configuration = JsonSerializer.Deserialize<Configuration>(jsonString);
            cache.Set(CachType.Configuration, configuration);

            Console.WriteLine("Start work");
            var fileService = new FIleService();

            var fileTransformTypes = fileService.ReadFiles();
            await fileService.SaveTransFormType(fileTransformTypes);

            var meta = fileService.Meta;
            Console.WriteLine("End work");

        }
    }
}

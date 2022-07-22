using System;
using System.IO;
using System.Text.Json;

namespace Read_file
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var cache = GlobalProperty.MemoryCache;
         
            string fileName = "config.json";
            string jsonString = File.ReadAllText(fileName);
            var configuration= JsonSerializer.Deserialize<Configuration>(jsonString);
            cache.Set(CachType.Configuration, configuration);

            Console.WriteLine("Start work");
            var fileService = new FIleService();
            fileService.ReadFiles();
            var meta = fileService.Meta;
            Console.WriteLine("End work");

        }
    }
}

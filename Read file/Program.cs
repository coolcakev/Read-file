using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Read_file
{
    internal class Program
    {
        static bool CanExit = true;
        static async Task Main(string[] args)
        {

            var input = "";
            var logicThread = new Thread(async () => await StartService(input));
            for (int i = 0; ; i++)
            {
                Console.WriteLine("Choose the options:");
                Console.WriteLine("1 - Start");
                Console.WriteLine("2 - Reset");
                Console.WriteLine("3 - Stop");
                input = Console.ReadLine();

                if (input == "1" && logicThread.ThreadState == ThreadState.Unstarted)
                {
                    logicThread.Start();
                }
                else if (input == "2")
                {

                }
                else if (input == "3")
                {

                    if (!CanExit)
                    {
                        Console.WriteLine("Program is busy");
                    }
                    while (!CanExit)
                    {

                    }
                    Console.WriteLine("End work");

                    return;
                }
                Console.Clear();
            }
        }
        static async Task StartService(string input)
        {
            var cache = GlobalProperty.MemoryCache;

            string fileName = "config.json";
            string jsonString = File.ReadAllText(fileName);
            var configuration = JsonSerializer.Deserialize<Configuration>(jsonString);
            cache.Set(CachType.Configuration, configuration);

            Console.WriteLine("Start work");
            var fileService = new FIleService();
            for (int i = 0; ; i++)
            {
                try
                {
                    var fileTransformTypes = fileService.ReadFiles();
                    if (fileTransformTypes.Count == 0)
                    {
                        CanExit = true;
                        continue;
                    }

                    CanExit = false;
                    await fileService.SaveTransFormType(fileTransformTypes);
                    await fileService.SaveMetaInLogFile();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}

using NasladdinPlace.Api.StressTesting.Args;
using NasladdinPlace.Api.StressTesting.Core;
using NasladdinPlace.Api.StressTesting.Models;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.StressTesting
{
    internal sealed class Program
    {
        private readonly IStressTester _stressTester;

        public Program(StressTestingConfig stressTestingConfig)
        {
            _stressTester = new StressTester(stressTestingConfig);
        }

        public void Run()
        {
            Task.Run(async () =>
            {
                var report = await _stressTester.RunAsync();
                Console.WriteLine(report.ToString());
            });
        }

        internal static void Main(string[] args)
        {
            var argsReader = new ArgsReader();
            if (!argsReader.TryReadConfig(args, out var config))
                Console.WriteLine("Config is incorrect.");

            var program = new Program(config);
            program.Run();

            Console.ReadKey();
        }
    }
}
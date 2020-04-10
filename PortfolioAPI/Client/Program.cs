using BenchmarkDotNet.Analysers;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using System.Diagnostics;

namespace Client
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ServiceBenchmark>();
        }
    }
}

namespace MassTransitBenchmark
{
    using System;
    using System.Collections.Generic;
    using NDesk.Options;

    class Program
    {
        static List<string> _remaining;

        static void Main(string[] args)
        {
            Console.WriteLine("MassTransit Benchmark");
            Console.WriteLine();

            var optionSet = new ProgramOptionSet();

            try
            {
                _remaining = optionSet.Parse(args);
            }
            catch (OptionException ex)
            {
                Console.Write("mtbench: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Use 'mtbench --help' for detailed usage information.");
                return;
            }

            if (optionSet.Help)
            {
                ShowHelp(optionSet);
                return;
            }

            optionSet.ShowOptions();


            RunLatencyBenchmark(optionSet);
        }

        static void RunLatencyBenchmark(ProgramOptionSet optionSet)
        {
            var messageLatencyOptionSet = new MessageLatencyOptionSet();
            try
            {
                messageLatencyOptionSet.Parse(_remaining);
            }
            catch (OptionException ex)
            {
                Console.Write("mtbench: ");
                Console.WriteLine(ex.Message);
                ShowHelp(messageLatencyOptionSet);
                return;
            }

            IMessageLatencySettings settings = messageLatencyOptionSet;

            IMessageLatencyTransport transport = new RabbitMqMessageLatencyTransport(optionSet, settings);
            var benchmark = new MessageLatencyBenchmark(transport, settings);

            benchmark.Run();
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: mtbench [OPTIONS]+");
            Console.WriteLine("Executes the benchmark using the specified transport with the specified options.");
            Console.WriteLine("If no benchmark is specified, all benchmarks are executed.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
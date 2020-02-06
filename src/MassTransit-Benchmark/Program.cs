namespace MassTransitBenchmark
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using Latency;
    using NDesk.Options;
    using RequestResponse;


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

                if (optionSet.Help)
                {
                    ShowHelp(optionSet);
                    return;
                }

                if (optionSet.Verbose)
                {
                }

                optionSet.ShowOptions();

                if (optionSet.Threads.HasValue)
                {
                    ThreadPool.GetMinThreads(out var workerThreads, out var completionPortThreads);
                    ThreadPool.SetMinThreads(Math.Max(workerThreads, optionSet.Threads.Value), completionPortThreads);
                }

                if (optionSet.Benchmark.HasFlag(ProgramOptionSet.BenchmarkOptions.Latency))
                {
                    RunLatencyBenchmark(optionSet);
                }

                if (optionSet.Benchmark.HasFlag(ProgramOptionSet.BenchmarkOptions.RPC))
                {
                    RunRequestResponseBenchmark(optionSet);
                }

                if (Debugger.IsAttached)
                {
                    Console.Write("Press any key to continue...");
                    Console.ReadKey();
                }
            }
            catch (OptionException ex)
            {
                Console.Write("mtbench: ");
                Console.WriteLine(ex.Message);
                Console.WriteLine("Use 'mtbench --help' for detailed usage information.");
            }
        }

        static void RunLatencyBenchmark(ProgramOptionSet optionSet)
        {
            var messageLatencyOptionSet = new MessageLatencyOptionSet();

            messageLatencyOptionSet.Parse(_remaining);

            IMessageLatencySettings settings = messageLatencyOptionSet;

            IMessageLatencyTransport transport;
            if (optionSet.Transport == ProgramOptionSet.TransportOptions.AzureServiceBus)
            {
                var serviceBusOptionSet = new ServiceBusOptionSet();

                serviceBusOptionSet.Parse(_remaining);

                serviceBusOptionSet.ShowOptions();

                ServicePointManager.Expect100Continue = false;
                ServicePointManager.UseNagleAlgorithm = false;

                transport = new ServiceBusMessageLatencyTransport(serviceBusOptionSet, settings);
            }
            else
            {
                var rabbitMqOptionSet = new RabbitMqOptionSet();
                rabbitMqOptionSet.Parse(_remaining);

                rabbitMqOptionSet.ShowOptions();

                transport = new RabbitMqMessageLatencyTransport(rabbitMqOptionSet, settings);
            }

            var benchmark = new MessageLatencyBenchmark(transport, settings);

            benchmark.Run();
        }

        static void RunRequestResponseBenchmark(ProgramOptionSet optionSet)
        {
            var requestResponseOptionSet = new RequestResponseOptionSet();

            requestResponseOptionSet.Parse(_remaining);

            IRequestResponseSettings settings = requestResponseOptionSet;

            IRequestResponseTransport transport;
            if (optionSet.Transport == ProgramOptionSet.TransportOptions.AzureServiceBus)
            {
                var serviceBusOptionSet = new ServiceBusOptionSet();

                serviceBusOptionSet.Parse(_remaining);

                serviceBusOptionSet.ShowOptions();

                ServicePointManager.Expect100Continue = false;
                ServicePointManager.UseNagleAlgorithm = false;

                transport = new ServiceBusRequestResponseTransport(serviceBusOptionSet, settings);
            }
            else
            {
                var rabbitMqOptionSet = new RabbitMqOptionSet();
                rabbitMqOptionSet.Parse(_remaining);

                rabbitMqOptionSet.ShowOptions();

                transport = new RabbitMqRequestResponseTransport(rabbitMqOptionSet, settings);
            }

            var benchmark = new RequestResponseBenchmark(transport, settings);

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

            Console.WriteLine();
            Console.WriteLine("RabbitMQ Options:");
            new RabbitMqOptionSet().WriteOptionDescriptions(Console.Out);

            Console.WriteLine();
            Console.WriteLine("Azure Service Bus Options:");
            new ServiceBusOptionSet().WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
            Console.WriteLine("Benchmark Options:");
            new MessageLatencyOptionSet().WriteOptionDescriptions(Console.Out);
        }
    }
}
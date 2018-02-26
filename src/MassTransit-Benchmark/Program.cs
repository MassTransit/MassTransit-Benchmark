using log4net;

namespace MassTransitBenchmark
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using log4net.Config;
    using Latency;
    using MassTransit.Log4NetIntegration.Logging;
#if !NETCOREAPP2_0
    using Microsoft.ServiceBus;
    #endif
    using NDesk.Options;
    using RequestResponse;


    class Program
    {
        static List<string> _remaining;

        static void Main(string[] args)
        {
            Console.WriteLine("MassTransit Benchmark");
            Console.WriteLine();

#if !NETCOREAPP2_0
            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Https;
#endif
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
                    ConfigureLogger();
                }

                optionSet.ShowOptions();

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
#if !NETCOREAPP2_0
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
    #endif
            var rabbitMqOptionSet = new RabbitMqOptionSet();
            rabbitMqOptionSet.Parse(_remaining);

            rabbitMqOptionSet.ShowOptions();

            transport = new RabbitMqMessageLatencyTransport(rabbitMqOptionSet, settings);
#if !NETCOREAPP2_0
            }
#endif

            var benchmark = new MessageLatencyBenchmark(transport, settings);

            benchmark.Run();
        }

        static void RunRequestResponseBenchmark(ProgramOptionSet optionSet)
        {
            var requestResponseOptionSet = new RequestResponseOptionSet();

            requestResponseOptionSet.Parse(_remaining);

            IRequestResponseSettings settings = requestResponseOptionSet;

            IRequestResponseTransport transport;
#if !NETCOREAPP2_0
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
    #endif
            var rabbitMqOptionSet = new RabbitMqOptionSet();
            rabbitMqOptionSet.Parse(_remaining);

            rabbitMqOptionSet.ShowOptions();

            transport = new RabbitMqRequestResponseTransport(rabbitMqOptionSet, settings);
#if !NETCOREAPP2_0
            }
#endif
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
#if !NETCOREAPP2_0
            new ServiceBusOptionSet().WriteOptionDescriptions(Console.Out);
#endif
            Console.WriteLine();
            Console.WriteLine("Benchmark Options:");
            new MessageLatencyOptionSet().WriteOptionDescriptions(Console.Out);
        }

        static void ConfigureLogger()
        {
            const string logConfig = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<log4net>
  <root>
    <level value=""INFO"" />
    <appender-ref ref=""console"" />
  </root>
  <appender name=""console"" type=""log4net.Appender.ColoredConsoleAppender"">
    <layout type=""log4net.Layout.PatternLayout"">
      <conversionPattern value=""%m%n"" />
    </layout>
  </appender>
</log4net>";


            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(logConfig)))
            {
#if NETCOREAPP2_0
                var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
                XmlConfigurator.Configure(logRepository, stream);
#else
                XmlConfigurator.Configure(stream);
#endif
            }

            Log4NetLogger.Use();
        }
    }
}
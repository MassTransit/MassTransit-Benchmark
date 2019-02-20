namespace MassTransitBenchmark
{
    using System;
    using NDesk.Options;


    class ProgramOptionSet :
        OptionSet
    {
        [Flags]
        public enum BenchmarkOptions
        {
            Latency = 1,
            RPC = 2,
        }


        public enum TransportOptions
        {
            RabbitMQ,
            AzureServiceBus
        }


        public ProgramOptionSet()
        {
            Add<string>("v|verbose", "Verbose output", x => Verbose = x != null);
            Add<string>("?|help", "Display this help and exit", x => Help = x != null);
            Add<int>("threads:", "The number of sending message clients", value => Threads = value);
            Add<TransportOptions>("t|transport:", "Transport (RabbitMQ, AzureServiceBus)",
                value => Transport = value);
            Add("rabbitmq", "Use RabbitMQ", x => Transport = TransportOptions.RabbitMQ);
            Add("servicebus", "Use Azure Service Bus", x => Transport = TransportOptions.AzureServiceBus);

            Add<BenchmarkOptions>("run:", "Run benchmark (All, Latency, RPC)", value => Benchmark = value);
            Add("rpc", "Run the RPC benchmark", x => Benchmark = BenchmarkOptions.RPC);
            Add("latency", "Run the Latency benchmark", x => Benchmark = BenchmarkOptions.Latency);

            Benchmark = BenchmarkOptions.Latency | BenchmarkOptions.RPC;
        }

        public BenchmarkOptions Benchmark { get; private set; }

        public int? Threads { get; set; }
        public bool Verbose { get; set; }
        public bool Help { get; set; }

        public TransportOptions Transport { get; private set; }

        public void ShowOptions()
        {
            Console.WriteLine("Transport: {0}", Transport);

            if (Transport == TransportOptions.RabbitMQ)
            {
            }
            else
            {
                Console.WriteLine("Transport");
            }

            if (Threads.HasValue)
                Console.WriteLine("Threads: {0}", Threads.Value);
        }
    }
}
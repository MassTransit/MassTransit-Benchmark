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
            Rpc = 2,
        }


        public enum TransportOptions
        {
            RabbitMq,
            AzureServiceBus,
            Mediator,
            AmazonSqs,
            ActiveMq
        }


        public ProgramOptionSet()
        {
            Add<string>("v|verbose", "Verbose output", x => Verbose = x != null);
            Add<string>("?|help", "Display this help and exit", x => Help = x != null);
            Add<int>("threads:", "The minimum number of thread pool threads", value => Threads = value);
            Add<TransportOptions>("t|transport:", "Transport (RabbitMQ, AzureServiceBus, Mediator, AmazonSqs)",
                value => Transport = value);
            Add("rabbitmq", "Use RabbitMQ", x => Transport = TransportOptions.RabbitMq);
            Add("mediator", "Use Mediator", x => Transport = TransportOptions.Mediator);
            Add("sqs", "Use Amazon SQS", x => Transport = TransportOptions.AmazonSqs);
            Add("servicebus", "Use Azure Service Bus", x => Transport = TransportOptions.AzureServiceBus);
            Add("activemq", "Use ActiveMQ", x => Transport = TransportOptions.ActiveMq);

            Add<BenchmarkOptions>("run:", "Run benchmark (All, Latency, RPC)", value => Benchmark = value);
            Add("rpc", "Run the RPC benchmark", x => Benchmark = BenchmarkOptions.Rpc);
            Add("latency", "Run the Latency benchmark", x => Benchmark = BenchmarkOptions.Latency);

            Benchmark = BenchmarkOptions.Latency | BenchmarkOptions.Rpc;
        }

        public BenchmarkOptions Benchmark { get; private set; }

        public int? Threads { get; set; }
        public bool Verbose { get; set; }
        public bool Help { get; set; }

        public TransportOptions Transport { get; private set; }

        public void ShowOptions()
        {
            Console.WriteLine("Transport: {0}", Transport);

            if (Threads.HasValue)
                Console.WriteLine("Threads: {0}", Threads.Value);
        }
    }
}
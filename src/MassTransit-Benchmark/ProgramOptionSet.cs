namespace MassTransitBenchmark
{
    using System;
    using NDesk.Options;

    class ProgramOptionSet :
        OptionSet
    {
        public enum TransportOptions
        {
            RabbitMQ,
            AzureServiceBus
        }

        public ProgramOptionSet()
        {
            Add<string>("v|verbose", "Verbose output", x => Verbose = x != null);
            Add<string>("?|help", "Display this help and exit", x => Help = x != null);
            Add<string, TransportOptions>("t|transport=", "Transport (RabbitMQ, AzureServiceBus)",
                (x, value) => Transport = value);
            Add("rabbitmq", "Use RabbitMQ", x => Transport = TransportOptions.RabbitMQ);
            Add("servicebus", "Use Azure Service Bus", x => Transport = TransportOptions.AzureServiceBus);
        }

        public bool Verbose { get; set; }
        public bool Help { get; set; }

        public TransportOptions Transport { get; set; }
        public string MessageSize { get; set; }


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

            //            _log.InfoFormat("Message Size: {0} {1}", _messageSize, _mixed ? "(mixed)" : "(fixed)");
            //            _log.InfoFormat("Iterations: {0}", _iterations);
            //            _log.InfoFormat("Clients: {0}", _instances);
            //            _log.InfoFormat("Requests Per Client: {0}", _requestsPerInstance);
            //            _log.InfoFormat("Consumer Limit: {0}", _consumerLimit);
        }
    }
}
namespace MassTransitBenchmark
{
    using System;
    using System.Net;
    using MassTransit.AzureServiceBusTransport;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Microsoft.ServiceBus.Messaging.Amqp;
    using NDesk.Options;

    internal class ServiceBusOptionSet :
        OptionSet,
        ServiceBusHostSettings
    {
        private readonly TokenScope _tokenScope;
        private readonly TimeSpan _tokenTimeToLive;
        private string _accessKey;
        private string _keyName;

        public ServiceBusOptionSet()
        {
            Add<string>("ns=", "The service bus namespace",
                x => ServiceUri = ServiceBusEnvironment.CreateServiceUri("sb", x, "Benchmark"));
            Add<string>("keyname=", "The access key name", x => _keyName = x);
            Add<string>("key=", "The access key", x => _accessKey = x);
            Add<int>("connections=", "The number of connections to configure for the service point manager",
                x => DefaultConnections = x);

            _tokenTimeToLive = TimeSpan.FromDays(1);
            _tokenScope = TokenScope.Namespace;

            OperationTimeout = TimeSpan.FromSeconds(60.0);
            RetryMinBackoff = TimeSpan.FromMilliseconds(100.0);
            RetryMaxBackoff = TimeSpan.FromSeconds(20.0);
            RetryLimit = 10;
            TransportType = Microsoft.ServiceBus.Messaging.TransportType.Amqp;
            AmqpTransportSettings = new AmqpTransportSettings
            {
                BatchFlushInterval = TimeSpan.FromMilliseconds(50.0)
            };
            NetMessagingTransportSettings = new NetMessagingTransportSettings
            {
                BatchFlushInterval = TimeSpan.FromMilliseconds(50.0)
            };

            DefaultConnections = ServicePointManager.DefaultConnectionLimit;
        }

        public int DefaultConnections { get; set; }

        public Uri ServiceUri { get; private set; }

        public TokenProvider TokenProvider =>
            TokenProvider.CreateSharedAccessSignatureTokenProvider(_keyName, _accessKey, _tokenTimeToLive, _tokenScope);

        public TimeSpan OperationTimeout { get; }

        public TimeSpan RetryMinBackoff { get; }

        public TimeSpan RetryMaxBackoff { get; }

        public int RetryLimit { get; }

        public Microsoft.ServiceBus.Messaging.TransportType TransportType { get; }

        public AmqpTransportSettings AmqpTransportSettings { get; }

        public NetMessagingTransportSettings NetMessagingTransportSettings { get; }

        public void ShowOptions()
        {
            Console.WriteLine("Service URI: {0}", ServiceUri);
            Console.WriteLine("Key Name: {0}", _keyName);
            Console.WriteLine("Access Key: {0}", new string('*', (_accessKey ?? "default").Length));
            Console.WriteLine("Service Point Manager.Default Connections: {0}", DefaultConnections);
        }
    }
}
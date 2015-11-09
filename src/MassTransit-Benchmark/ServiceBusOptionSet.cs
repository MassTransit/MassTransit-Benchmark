namespace MassTransitBenchmark
{
    using System;
    using System.Net;
    using MassTransit.AzureServiceBusTransport;
    using Microsoft.ServiceBus;
    using NDesk.Options;

    class ServiceBusOptionSet :
        OptionSet,
        ServiceBusHostSettings
    {
        readonly TokenScope _tokenScope;
        readonly TimeSpan _tokenTimeToLive;
        string _accessKey;
        string _keyName;

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

            DefaultConnections = ServicePointManager.DefaultConnectionLimit;


            OperationTimeout = TimeSpan.FromSeconds(10);
        }

        public int DefaultConnections { get; set; }

        public Uri ServiceUri { get; set; }

        public TokenProvider TokenProvider =>
            TokenProvider.CreateSharedAccessSignatureTokenProvider(_keyName, _accessKey, _tokenTimeToLive, _tokenScope);

        public TimeSpan OperationTimeout { get; }

        public void ShowOptions()
        {
            Console.WriteLine("Service URI: {0}", ServiceUri);
            Console.WriteLine("Key Name: {0}", _keyName);
            Console.WriteLine("Access Key: {0}", new string('*', (_accessKey ?? "default").Length));
            Console.WriteLine("Service Point Manager.Default Connections: {0}", DefaultConnections);
        }
    }
}
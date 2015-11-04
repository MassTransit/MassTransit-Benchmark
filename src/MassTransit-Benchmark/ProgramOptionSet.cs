namespace MassTransitBenchmark
{
    using System;
    using System.Net.Security;
    using MassTransit.RabbitMqTransport;
    using NDesk.Options;

    class ProgramOptionSet :
        OptionSet,
        RabbitMqHostSettings
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
            Add<string>("h|host=", "The host name of the broker", x => Host = x);
            Add<string, string>("vhost=", "The virtual host to use", (x, value) => VirtualHost = value);
            Add<string, string>("u|username=", "Username (if using basic credentials)", (x, value) => Username = value);
            Add<string, string>("p|password=", "Password (if using basic credentials)", (x, value) => Password = value);
            Add<string, ushort>("heartbeat=", "Heartbeat (for RabbitMQ)", (x, value) => Heartbeat = value);
            Add<string, TransportOptions>("t|transport=", "Transport (RabbitMQ, AzureServiceBus)",
                (x, value) => Transport = value);

            Host = "localhost";
            Username = "guest";
            Password = "guest";
            Heartbeat = 0;
            VirtualHost = "/";
            Port = 5672;
        }

        public bool Verbose { get; set; }
        public bool Help { get; set; }

        public TransportOptions Transport { get; set; }
        public string MessageSize { get; set; }


        public string Host { get; set; }

        public int Port { get; }

        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ushort Heartbeat { get; set; }

        public bool Ssl { get; }
        public string SslServerName { get; }
        public SslPolicyErrors AcceptablePolicyErrors { get; }
        public string ClientCertificatePath { get; }
        public string ClientCertificatePassphrase { get; }


        public void ShowOptions()
        {
            Console.WriteLine("Transport: {0}", Transport);

            if (Transport == TransportOptions.RabbitMQ)
            {
                Console.WriteLine("Host: {0}", Host);
                Console.WriteLine("Virtual Host: {0}", VirtualHost);
                Console.WriteLine("Username: {0}", Username);
                Console.WriteLine("Password: {0}", new string('*', (Password ?? "default").Length));
                Console.WriteLine("Heartbeat: {0}", Heartbeat);
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
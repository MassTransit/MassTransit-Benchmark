namespace MassTransitBenchmark
{
    using System;
    using MassTransit.ActiveMqTransport;
    using MassTransit.ActiveMqTransport.Configurators;
    using NDesk.Options;


    class ActiveMqOptionSet :
        OptionSet,
        ActiveMqHostSettings
    {
        readonly ConfigurationHostSettings _hostSettings;

        public ActiveMqOptionSet()
        {
            _hostSettings = new ConfigurationHostSettings(new Uri("activemq://localhost"));

            Add<string>("h|host:", "The host name of the broker", x => _hostSettings.Host = x);
            Add<int>("port:", "The virtual host to use", value => _hostSettings.Port = value);
            Add<string>("u|username:", "Username (if using basic credentials)", value => _hostSettings.Username = value);
            Add<string>("p|password:", "Password (if using basic credentials)", value => _hostSettings.Password = value);
        }

        public override string ToString()
        {
            return new UriBuilder
            {
                Scheme = UseSsl ? "ssl" : "tcp",
                Host = Host
            }.Uri.ToString();
        }

        public void ShowOptions()
        {
            Console.WriteLine("Host: {0}", HostAddress);
        }

        public string Host => _hostSettings.Host;

        public int Port => _hostSettings.Port;

        public string Username => _hostSettings.Username;

        public string Password => _hostSettings.Password;

        public Uri HostAddress => _hostSettings.HostAddress;

        public bool UseSsl => _hostSettings.UseSsl;

        public Uri BrokerAddress => _hostSettings.BrokerAddress;

        public Apache.NMS.IConnection CreateConnection()
        {
            return _hostSettings.CreateConnection();
        }
    }
}
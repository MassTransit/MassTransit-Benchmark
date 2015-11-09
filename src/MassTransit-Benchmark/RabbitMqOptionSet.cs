namespace MassTransitBenchmark
{
    using System;
    using System.Net.Security;
    using MassTransit.RabbitMqTransport;
    using NDesk.Options;

    class RabbitMqOptionSet :
        OptionSet,
        RabbitMqHostSettings
    {
        public RabbitMqOptionSet()
        {
            Add<string>("h|host:", "The host name of the broker", x => Host = x);
            Add<string>("vhost:", "The virtual host to use", value => VirtualHost = value);
            Add<string>("u|username:", "Username (if using basic credentials)", value => Username = value);
            Add<string>("p|password:", "Password (if using basic credentials)", value => Password = value);
            Add<ushort>("heartbeat:", "Heartbeat (for RabbitMQ)", value => Heartbeat = value);

            Host = "localhost";
            Username = "guest";
            Password = "guest";
            Heartbeat = 0;
            VirtualHost = "/";
            Port = 5672;

            Ssl = false;
            SslServerName = Host;
            AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch;
            ClientCertificatePath = "";
            ClientCertificatePassphrase = "";
        }

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
            Console.WriteLine("Host: {0}", Host);
            Console.WriteLine("Virtual Host: {0}", VirtualHost);
            Console.WriteLine("Username: {0}", Username);
            Console.WriteLine("Password: {0}", new string('*', (Password ?? "default").Length));
            Console.WriteLine("Heartbeat: {0}", Heartbeat);
        }
    }
}
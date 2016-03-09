// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransitBenchmark
{
    using System;
    using System.Net.Security;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
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
            SslProtocol = SslProtocols.Tls12;
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

        public SslProtocols SslProtocol { get; }
        public string SslServerName { get; }
        public SslPolicyErrors AcceptablePolicyErrors { get; }
        public string ClientCertificatePath { get; }
        public string ClientCertificatePassphrase { get; }

        public X509Certificate ClientCertificate => null;
        public bool UseClientCertificateAsAuthenticationIdentity => false;

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
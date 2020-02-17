namespace MassTransitBenchmark.RequestResponse
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.RabbitMqTransport;
    using MassTransit.Util;


    public class RabbitMqRequestResponseTransport :
        IRequestResponseTransport
    {
        readonly RabbitMqHostSettings _hostSettings;
        readonly IRequestResponseSettings _settings;
        Uri _targetEndpointAddress;
        Task<IClientFactory> _clientFactory;

        public RabbitMqRequestResponseTransport(RabbitMqHostSettings hostSettings, IRequestResponseSettings settings)
        {
            _hostSettings = hostSettings;
            _settings = settings;
        }

        public Task<IClientFactory> ClientFactory => _clientFactory;

        public Uri TargetEndpointAddress => _targetEndpointAddress;

        public IBusControl GetBusControl(Action<IReceiveEndpointConfigurator> callback)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(_hostSettings);

                x.ReceiveEndpoint("rpc_consumer" + (_settings.Durable ? "" : "_express"), e =>
                {
                    e.PurgeOnStartup = true;
                    e.Durable = _settings.Durable;
                    e.PrefetchCount = _settings.PrefetchCount;

                    callback(e);

                    _targetEndpointAddress = e.InputAddress;
                });
            });

            TaskUtil.Await(() => busControl.StartAsync());

            _clientFactory = busControl.CreateReplyToClientFactory();

            return busControl;
        }
    }
}
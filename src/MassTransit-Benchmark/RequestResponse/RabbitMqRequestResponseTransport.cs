namespace MassTransitBenchmark.RequestResponse
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.RabbitMqTransport;


    public class RabbitMqRequestResponseTransport :
        IRequestResponseTransport
    {
        readonly RabbitMqHostSettings _hostSettings;
        readonly IRequestResponseSettings _settings;
        Uri _targetEndpointAddress;
        Task<IClientFactory> _clientFactory;
        IBusControl _busControl;

        public RabbitMqRequestResponseTransport(RabbitMqHostSettings hostSettings, IRequestResponseSettings settings)
        {
            _hostSettings = hostSettings;
            _settings = settings;
        }

        public async Task<IRequestClient<T>> GetRequestClient<T>(TimeSpan settingsRequestTimeout)
            where T : class
        {
            var clientFactory = await _clientFactory;

            return clientFactory.CreateRequestClient<T>(_targetEndpointAddress, settingsRequestTimeout);
        }

        public void GetBusControl(Action<IReceiveEndpointConfigurator> callback)
        {
            _busControl = Bus.Factory.CreateUsingRabbitMq(x =>
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

            _busControl.Start();

            _clientFactory = _busControl.CreateReplyToClientFactory();
        }

        public void Dispose()
        {
            _busControl.Stop();
        }
    }
}
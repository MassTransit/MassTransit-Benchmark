namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.RabbitMqTransport;


    public class RabbitMqMessageLatencyTransport :
        IMessageLatencyTransport
    {
        readonly RabbitMqHostSettings _hostSettings;
        readonly IMessageLatencySettings _settings;
        Uri _targetAddress;
        IBusControl _busControl;
        Task<ISendEndpoint> _targetEndpoint;

        public RabbitMqMessageLatencyTransport(RabbitMqHostSettings hostSettings, IMessageLatencySettings settings)
        {
            _hostSettings = hostSettings;
            _settings = settings;
        }

        public Task<ISendEndpoint> TargetEndpoint => _targetEndpoint;

        public async Task Start(Action<IReceiveEndpointConfigurator> callback)
        {
            _busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                x.Host(_hostSettings);

                x.ReceiveEndpoint("latency_consumer" + (_settings.Durable ? "" : "_express"), e =>
                {
                    e.PurgeOnStartup = true;
                    e.Durable = _settings.Durable;
                    e.PrefetchCount = _settings.PrefetchCount;

                    callback(e);

                    _targetAddress = e.InputAddress;
                });
            });

            await _busControl.StartAsync();

            _targetEndpoint = _busControl.GetSendEndpoint(_targetAddress);
        }

        public async ValueTask DisposeAsync()
        {
            await _busControl.StopAsync();
        }
    }
}
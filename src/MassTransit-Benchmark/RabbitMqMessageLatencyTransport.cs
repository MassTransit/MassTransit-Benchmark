namespace MassTransitBenchmark
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
        Task<ISendEndpoint> _targetEndpoint;

        public RabbitMqMessageLatencyTransport(RabbitMqHostSettings hostSettings, IMessageLatencySettings settings)
        {
            _hostSettings = hostSettings;
            _settings = settings;
        }

        public Task<ISendEndpoint> TargetEndpoint => _targetEndpoint;

        public IBusControl GetBusControl(Action<IReceiveEndpointConfigurator> callback)
        {
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                IRabbitMqHost host = x.Host(_hostSettings);

                x.ReceiveEndpoint(host, "latency_consumer" + (_settings.Durable ? "" : "_express"), e =>
                {
                    e.PurgeOnStartup = true;
                    e.Durable = _settings.Durable;
                    e.PrefetchCount = _settings.PrefetchCount;

                    callback(e);

                    _targetAddress = e.InputAddress;
                });
            });

            busControl.Start();

            _targetEndpoint = busControl.GetSendEndpoint(_targetAddress);

            return busControl;
        }
    }
}
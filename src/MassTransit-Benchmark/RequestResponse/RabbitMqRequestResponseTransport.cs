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
        Task<ISendEndpoint> _targetEndpoint;
        Uri _targetEndpointAddress;

        public RabbitMqRequestResponseTransport(RabbitMqHostSettings hostSettings, IRequestResponseSettings settings)
        {
            _hostSettings = hostSettings;
            _settings = settings;
        }

        public Task<ISendEndpoint> TargetEndpoint => _targetEndpoint;
        public Uri TargetEndpointAddress => _targetEndpointAddress;

        public IBusControl GetBusControl(Action<IReceiveEndpointConfigurator> callback)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(x =>
            {
                var host = x.Host(_hostSettings);

                x.ReceiveEndpoint(host, "rpc_consumer" + (_settings.Durable ? "" : "_express"), e =>
                {
                    e.PurgeOnStartup = true;
                    e.Durable = _settings.Durable;
                    e.PrefetchCount = _settings.PrefetchCount;

                    callback(e);

                    _targetEndpointAddress = e.InputAddress;
                });
            });

            busControl.Start();

            _targetEndpoint = busControl.GetSendEndpoint(_targetEndpointAddress);

            return busControl;
        }
    }
}
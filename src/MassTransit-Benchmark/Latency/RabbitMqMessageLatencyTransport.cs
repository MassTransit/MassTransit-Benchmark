namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.RabbitMqTransport;
    using MassTransit.Util;


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

        public Task<ISendEndpoint> TargetEndpoint
        {
            get
            {
                return _targetEndpoint;

                // async Task<ISendEndpoint> GetAsync()
                // {
                //     var responseEndpointHandle = _busControl.ConnectResponseEndpoint();
                //
                //     await responseEndpointHandle.Ready;
                //
                //     return await responseEndpointHandle.ReceiveEndpoint.GetSendEndpoint(_targetAddress);
                // }
                //
                // return GetAsync();
            }
        }

        public IBusControl GetBusControl(Action<IReceiveEndpointConfigurator> callback)
        {
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(x =>
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

            TaskUtil.Await(() => busControl.StartAsync());

            _busControl = busControl;

            _targetEndpoint = busControl.GetSendEndpoint(_targetAddress);


            return busControl;
        }
    }
}
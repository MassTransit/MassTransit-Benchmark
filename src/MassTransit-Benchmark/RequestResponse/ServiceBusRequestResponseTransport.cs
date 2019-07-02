namespace MassTransitBenchmark.RequestResponse
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Azure.ServiceBus.Core;
    using MassTransit.Util;


    public class ServiceBusRequestResponseTransport :
        IRequestResponseTransport
    {
        readonly ServiceBusHostSettings _hostSettings;
        readonly IRequestResponseSettings _settings;

        Task<ISendEndpoint> _targetEndpoint;
        Uri _targetEndpointAddress;

        public ServiceBusRequestResponseTransport(ServiceBusHostSettings hostSettings, IRequestResponseSettings settings)
        {
            _hostSettings = hostSettings;
            _settings = settings;
        }

        public Task<ISendEndpoint> TargetEndpoint => _targetEndpoint;
        public Uri TargetEndpointAddress => _targetEndpointAddress;

        public IBusControl GetBusControl(Action<IReceiveEndpointConfigurator> callback)
        {
            var busControl = Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                var host = x.Host(_hostSettings);

                x.ReceiveEndpoint(host, "rpc_consumer" + (_settings.Durable ? "" : "_express"), e =>
                {
                    e.PrefetchCount = _settings.PrefetchCount;
                    if (_settings.ConcurrencyLimit > 0)
                        e.MaxConcurrentCalls = _settings.ConcurrencyLimit;

                    callback(e);

                    _targetEndpointAddress = e.InputAddress;
                });
            });

            TaskUtil.Await(() => busControl.StartAsync());

            _targetEndpoint = busControl.GetSendEndpoint(_targetEndpointAddress);

            return busControl;
        }
    }
}
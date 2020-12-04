using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassTransitBenchmark.Latency
{
    class InMemoryMessageLatencyTransport : IMessageLatencyTransport
    {
        readonly IMessageLatencySettings _settings;
        Task<ISendEndpoint> _targetEndpoint;
        IBusControl _busControl;
        Uri _targetAddress;

        public InMemoryMessageLatencyTransport(IMessageLatencySettings settings)
        {
            _settings = settings;
        }

        public Task<ISendEndpoint> TargetEndpoint => _targetEndpoint;

        public async ValueTask DisposeAsync()
        {
            await _busControl.StopAsync();
        }

        public async Task Start(Action<IReceiveEndpointConfigurator> callback)
        {
            _busControl = Bus.Factory.CreateUsingInMemory(x =>
            {
                x.ReceiveEndpoint("latency_consumer", e =>
                {
                    callback(e);
                    _targetAddress = e.InputAddress;
                });
            });

            await _busControl.StartAsync();

            _targetEndpoint = _busControl.GetSendEndpoint(_targetAddress);
        }
    }
}

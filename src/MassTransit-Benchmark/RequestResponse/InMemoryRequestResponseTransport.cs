using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassTransitBenchmark.RequestResponse
{
    public class InMemoryRequestResponseTransport : IRequestResponseTransport
    {
        IBusControl _busControl;
        IRequestResponseSettings _settings;

        Uri _targetEndpointAddress;

        Task<IClientFactory> _clientFactory;

        public InMemoryRequestResponseTransport(IRequestResponseSettings settings)
        {
            _settings = settings;
        }

              

        public void GetBusControl(Action<IReceiveEndpointConfigurator> callback)
        {
            _busControl = Bus.Factory.CreateUsingInMemory(x =>
            {
                x.ReceiveEndpoint("rpc_consumer" , e =>
                {
                    callback(e);
                    _targetEndpointAddress = e.InputAddress;
                });
            });

            _busControl.Start();

            _clientFactory = _busControl.CreateReplyToClientFactory();
        }

        public async Task<IRequestClient<T>> GetRequestClient<T>(TimeSpan settingsRequestTimeout) where T : class
        {
            var clientFactory = await _clientFactory;

            return clientFactory.CreateRequestClient<T>(_targetEndpointAddress, settingsRequestTimeout);
        }

        public void Dispose()
        {
            _busControl.Stop();
        }
    }
}

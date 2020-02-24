namespace MassTransitBenchmark.Latency
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Util;


    public class MediatorMessageLatencyTransport :
        IMessageLatencyTransport
    {
        readonly IMessageLatencySettings _settings;
        Task<ISendEndpoint> _targetEndpoint;
        IMediator _mediator;

        public MediatorMessageLatencyTransport(IMessageLatencySettings settings)
        {
            _settings = settings;
        }

        public Task<ISendEndpoint> TargetEndpoint => _targetEndpoint;

        public void Start(Action<IReceiveEndpointConfigurator> callback)
        {
            _mediator = Bus.Factory.CreateMediator(callback);

            _targetEndpoint = Task.FromResult<ISendEndpoint>(_mediator);
        }

        public void Dispose()
        {
            TaskUtil.Await(() => _mediator.DisposeAsync());
        }
    }
}
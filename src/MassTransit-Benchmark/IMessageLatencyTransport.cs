namespace MassTransitBenchmark
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;

    public interface IMessageLatencyTransport
    {
        /// <summary>
        /// The target endpoint where messages are to be sent
        /// </summary>
        Task<ISendEndpoint> TargetEndpoint { get; }

        /// <summary>
        /// The bus control
        /// </summary>
        /// <param name="callback"></param>
        IBusControl GetBusControl(Action<IReceiveEndpointConfigurator> callback);
    }
}
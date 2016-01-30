namespace MassTransitBenchmark.RequestResponse
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;


    public interface IRequestResponseTransport
    {
        /// <summary>
        /// The target endpoint where messages are to be sent
        /// </summary>
        Task<ISendEndpoint> TargetEndpoint { get; }

        /// <summary>
        /// The address of the target endpoint
        /// </summary>
        Uri TargetEndpointAddress { get; }

        /// <summary>
        /// The bus control
        /// </summary>
        /// <param name="callback"></param>
        IBusControl GetBusControl(Action<IReceiveEndpointConfigurator> callback);
    }
}
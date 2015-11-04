namespace MassTransitBenchmark
{
    using System;

    public class LatencyTestMessage
    {
        public LatencyTestMessage()
        {
        }

        public LatencyTestMessage(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; set; }
    }
}
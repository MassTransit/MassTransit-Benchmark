namespace MassTransitBenchmark.Latency
{
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;


    public class MessageLatencyConsumer :
        IConsumer<LatencyTestMessage>
    {
        public static int CurrentConsumerCount;
        public static int MaxConsumerCount;
        readonly IReportConsumerMetric _report;

        public MessageLatencyConsumer(IReportConsumerMetric report)
        {
            _report = report;
        }

        public async Task Consume(ConsumeContext<LatencyTestMessage> context)
        {
            var current = Interlocked.Increment(ref CurrentConsumerCount);
            var maxConsumercount = MaxConsumerCount;
            if (current > maxConsumercount)
                Interlocked.CompareExchange(ref MaxConsumerCount, current, maxConsumercount);

            try
            {
                await _report.Consumed<LatencyTestMessage>(context.Message.CorrelationId);
            }
            finally
            {
                Interlocked.Decrement(ref CurrentConsumerCount);
            }
        }
    }
}
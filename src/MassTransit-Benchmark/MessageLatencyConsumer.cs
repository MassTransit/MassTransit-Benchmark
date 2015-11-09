namespace MassTransitBenchmark
{
    using System.Threading.Tasks;
    using MassTransit;

    public class MessageLatencyConsumer :
        IConsumer<LatencyTestMessage>
    {
        readonly IReportConsumerMetric _report;

        public MessageLatencyConsumer(IReportConsumerMetric report)
        {
            _report = report;
        }

        public Task Consume(ConsumeContext<LatencyTestMessage> context)
        {
            return _report.Consumed<LatencyTestMessage>(context.Message.CorrelationId);
        }
    }
}
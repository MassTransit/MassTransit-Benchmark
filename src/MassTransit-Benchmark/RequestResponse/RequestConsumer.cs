namespace MassTransitBenchmark.RequestResponse
{
    using System.Threading.Tasks;
    using MassTransit;


    public class RequestConsumer :
        IConsumer<RequestMessage>
    {
        readonly IReportConsumerMetric _report;

        public RequestConsumer(IReportConsumerMetric report)
        {
            _report = report;
        }

        public Task Consume(ConsumeContext<RequestMessage> context)
        {
            context.Respond(new ResponseMessage(context.Message.CorrelationId));

            return _report.Consumed<RequestMessage>(context.Message.CorrelationId);
        }
    }
}
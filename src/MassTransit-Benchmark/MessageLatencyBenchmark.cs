namespace MassTransitBenchmark
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Util;

    /// <summary>
    /// Benchmark that determines the latency of messages between the time the message is published
    /// to the broker until it is acked by RabbitMQ. And then consumed by the message consumer.
    /// </summary>
    public class MessageLatencyBenchmark
    {
        readonly IMessageLatencySettings _settings;
        readonly IMessageLatencyTransport _transport;
        MessageMetricCapture _capture;
        TimeSpan _consumeDuration;
        TimeSpan _sendDuration;

        public MessageLatencyBenchmark(IMessageLatencyTransport transport, IMessageLatencySettings settings)
        {
            _transport = transport;
            _settings = settings;

            if (settings.MessageCount/settings.Clients*settings.Clients != settings.MessageCount)
            {
                throw new ArgumentException("The clients must be a factor of message count");
            }
        }

        public void Run(CancellationToken cancellationToken = default(CancellationToken))
        {
            _capture = new MessageMetricCapture(_settings.MessageCount);

            IBusControl busControl = _transport.GetBusControl(ConfigureReceiveEndpoint);
            try
            {
                Console.WriteLine("Running Message Latency Benchmark");

                TaskUtil.Await(() => RunBenchmark(busControl), cancellationToken);

                Console.WriteLine("Message Count: {0}", _settings.MessageCount);
                Console.WriteLine("Clients: {0}", _settings.Clients);
                Console.WriteLine("Durable: {0}", _settings.Durable);
                Console.WriteLine("Prefetch Count: {0}", _settings.PrefetchCount);
                Console.WriteLine("Concurrency Limit: {0}", _settings.ConcurrencyLimit);

                Console.WriteLine("Total send duration: {0:g}", _sendDuration);
                Console.WriteLine("Send message rate: {0:F2} (msg/s)",
                    _settings.MessageCount*1000/_sendDuration.TotalMilliseconds);
                Console.WriteLine("Total consume duration: {0:g}", _consumeDuration);
                Console.WriteLine("Consume message rate: {0:F2} (msg/s)",
                    _settings.MessageCount*1000/_consumeDuration.TotalMilliseconds);

                MessageMetric[] messageMetrics = _capture.GetMessageMetrics();

                Console.WriteLine("Avg Ack Time: {0:F0}ms",
                    messageMetrics.Average(x => x.AckLatency)*1000/Stopwatch.Frequency);
                Console.WriteLine("Min Ack Time: {0:F0}ms",
                    messageMetrics.Min(x => x.AckLatency)*1000/Stopwatch.Frequency);
                Console.WriteLine("Max Ack Time: {0:F0}ms",
                    messageMetrics.Max(x => x.AckLatency)*1000/Stopwatch.Frequency);
                Console.WriteLine("Med Ack Time: {0:F0}ms",
                    messageMetrics.Median(x => x.AckLatency)*1000/Stopwatch.Frequency);
                Console.WriteLine("95t Ack Time: {0:F0}ms",
                    messageMetrics.Percentile(x => x.AckLatency)*1000/Stopwatch.Frequency);

                Console.WriteLine("Avg Consume Time: {0:F0}ms",
                    messageMetrics.Average(x => x.ConsumeLatency)*1000/Stopwatch.Frequency);
                Console.WriteLine("Min Consume Time: {0:F0}ms",
                    messageMetrics.Min(x => x.ConsumeLatency)*1000/Stopwatch.Frequency);
                Console.WriteLine("Max Consume Time: {0:F0}ms",
                    messageMetrics.Max(x => x.ConsumeLatency)*1000/Stopwatch.Frequency);
                Console.WriteLine("Med Consume Time: {0:F0}ms",
                    messageMetrics.Median(x => x.ConsumeLatency)*1000/Stopwatch.Frequency);
                Console.WriteLine("95t Consume Time: {0:F0}ms",
                    messageMetrics.Percentile(x => x.ConsumeLatency)*1000/Stopwatch.Frequency);

                Console.WriteLine();
                DrawResponseTimeGraph(messageMetrics, x => x.ConsumeLatency);
            }
            finally
            {
                busControl.Stop(cancellationToken);
            }
        }


        void DrawResponseTimeGraph(MessageMetric[] metrics, Func<MessageMetric, long> selector)
        {
            long maxTime = metrics.Max(selector);
            long minTime = metrics.Min(selector);

            const int segments = 10;

            long span = maxTime - minTime;
            long increment = span / segments;

            var histogram = (from x in metrics.Select(selector)
                             let key = ((x - minTime) * segments / span)
                             where key >= 0 && key < segments
                             let groupKey = key
                             group x by groupKey
                into segment
                             orderby segment.Key
                             select new { Value = segment.Key, Count = segment.Count() }).ToList();

            int maxCount = histogram.Max(x => x.Count);

            foreach (var item in histogram)
            {
                int barLength = item.Count * 60 / maxCount;
                Console.WriteLine("{0,5}ms {2,-60} ({1,7})", (minTime + increment * item.Value)*1000/Stopwatch.Frequency, item.Count,
                    new string('*', barLength));
            }
        }

        async Task RunBenchmark(IBusControl busControl)
        {
            await Task.Yield();

            ISendEndpoint targetEndpoint = await _transport.TargetEndpoint;

            var stripes = new Task[_settings.Clients];

            for (var i = 0; i < _settings.Clients; i++)
            {
                stripes[i] = RunStripe(targetEndpoint, _settings.MessageCount/_settings.Clients);
            }

            await Task.WhenAll(stripes);

            _sendDuration = await _capture.SendCompleted;
            _consumeDuration = await _capture.ConsumeCompleted;
        }

        async Task RunStripe(ISendEndpoint targetEndpoint, long messageCount)
        {
            await Task.Yield();

            for (long i = 0; i < messageCount; i++)
            {
                Guid messageId = NewId.NextGuid();
                Task task = targetEndpoint.Send(new LatencyTestMessage(messageId));

                await _capture.Sent(messageId, task);
            }
        }

        void ConfigureReceiveEndpoint(IReceiveEndpointConfigurator configurator)
        {
            if (_settings.ConcurrencyLimit > 0)
                configurator.UseConcurrencyLimit(_settings.ConcurrencyLimit);

            configurator.Consumer(() => new MessageLatencyConsumer(_capture));
        }
    }

    public class MessageMetric
    {
        public MessageMetric(Guid messageId, long ackLatency, long consumeLatency)
        {
            MessageId = messageId;
            AckLatency = ackLatency;
            ConsumeLatency = consumeLatency;
        }

        public Guid MessageId { get; }
        public long AckLatency { get; set; }
        public long ConsumeLatency { get; set; }
    }
}
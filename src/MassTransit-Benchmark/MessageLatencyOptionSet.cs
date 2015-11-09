namespace MassTransitBenchmark
{
    using NDesk.Options;

    public class MessageLatencyOptionSet :
        OptionSet,
        IMessageLatencySettings
    {
        public MessageLatencyOptionSet()
        {
            Add<long>("count=", "The number of messages to send", value => MessageCount = value);
            Add<ushort>("prefetch=", "The prefetch count for the broker", value => PrefetchCount = value);
            Add<int>("concurrency=", "The number of concurrent consumers", value => ConcurrencyLimit = value);
            Add<int>("clients=", "The number of sending message clients", value => Clients = value);
            Add<bool>("durable", "The number of concurrent consumers", value => Durable = value);

            MessageCount = 10000;
            PrefetchCount = 16;
            ConcurrencyLimit = 4;
            Clients = 1;
            Durable = true;
        }

        public bool Durable { get; set; }
        public long MessageCount { get; set; }
        public ushort PrefetchCount { get; set; }
        public int Clients { get; set; }
        public int ConcurrencyLimit { get; set; }
    }
}
# MassTransit Benchmark

A set of benchmarks for measuring the performance of MassTransit with the supported transports.


## Message Latency 

Measures the throughput (send, consume) and latency (time from send to receive) of messages. The number of clients can be scaled to simulate multiple concurrent messages being written to the queue, and the concurrency, prefetch counts, and other settings can also be adjusted.

## Usage

To see the usage, enter:

`dotnet run -f netcoreapp2.2 -c Release -- -?`

That will show all the details of using the benchmark.

## RabbitMQ

A good example that really hits RabbitMQ pretty hard.

`dotnet run -f netcoreapp2.2 -c Release -- --count=100000 --prefetch=1000 --clients=100`

### Output

```
MassTransit Benchmark

Transport: RabbitMQ
Host: localhost
Virtual Host: /
Username: guest
Password: *****
Heartbeat: 0
Publisher Confirmation: False
Running Message Latency Benchmark
Message Count: 100000
Clients: 100
Durable: False
Payload Length: 0
Prefetch Count: 1000
Concurrency Limit: 0
Total send duration: 0:00:09.1151465
Send message rate: 10970.75 (msg/s)
Total consume duration: 0:00:09.765957
Consume message rate: 10239.65 (msg/s)
Concurrent Consumer Count: 10
Avg Ack Time: 8ms
Min Ack Time: 0ms
Max Ack Time: 214ms
Med Ack Time: 5ms
95t Ack Time: 29ms
Avg Consume Time: 714ms
Min Consume Time: 246ms
Max Consume Time: 963ms
Med Consume Time: 770ms
95t Consume Time: 908ms

  246ms ****                                                         (   2272)
  318ms ****                                                         (   2219)
  389ms ******                                                       (   3235)
  461ms ***********                                                  (   5605)
  533ms ******************************                               (  14327)
  604ms *************************                                    (  11927)
  676ms ***************                                              (   7403)
  748ms **********************************                           (  16409)
  819ms ************************************************************ (  28472)
  891ms *****************                                            (   8130)
Host: localhost
Virtual Host: /
Username: guest
Password: *****
Heartbeat: 0
Publisher Confirmation: False
Running Request Response Benchmark
Message Count: 100000
Clients: 100
Durable: False
Prefetch Count: 1000
Concurrency Limit: 0
Total consume duration: 0:00:20.6300097
Consume message rate: 4847.31 (msg/s)
Total request duration: 0:00:20.6330248
Request rate: 4846.60 (msg/s)
Concurrent Consumer Count: 25
Avg Request Time: 20ms
Min Request Time: 4ms
Max Request Time: 104ms
Med Request Time: 20ms
95t Request Time: 25ms
Avg Consume Time: 5ms
Min Consume Time: 0ms
Max Consume Time: 54ms
Med Consume Time: 4ms
95t Consume Time: 9ms

Request duration distribution
    4ms                                                              (    527)
   14ms ************************************************************ (  92393)
   24ms ****                                                         (   6736)
   34ms                                                              (    227)
   44ms                                                              (     17)
   84ms                                                              (     16)
   94ms                                                              (     83)
```

# MassTransit Benchmark

A set of benchmarks for measuring the performance of MassTransit with the supported transports.


## Message Latency 

Measures the throughput (send, consume) and latency (time from send to receive) of messages. The number of clients can be scaled to simulate multiple concurrent messages being written to the queue, and the concurrency, prefetch counts, and other settings can also be adjusted.

## Usage

To see the usage, enter:

    mtbench --help

That will show all the details of using the benchmark.
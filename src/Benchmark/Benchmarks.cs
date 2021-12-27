using System;
using BenchmarkDotNet.Attributes;
using NLog;

namespace Benchmark;

/// <summary>
/// Launch loki on http://localhost:3100 prior to running the benchmark.
/// </summary>
[MemoryDiagnoser]
public class Benchmarks
{
    // Create an nlog logger with loki as a target
    private readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly string[] users = new[] { "Corentin Altepe", "A", "B", "C", "D", "E", "F", "G", "H", "I" };
    private readonly string[] destinations = new[] { "127.0.0.1:8475", "A", "B", "C", "D", "E", "F", "G", "H", "I" };
    private readonly Exception exception = new("Ooops. Something went wrong!");

    [Benchmark]
    public void LogInfoWith2Variables()
    {
        logger.Info("Receive message from {User} with destination {Destination}.", users[0], destinations[0]);
    }

    [Benchmark]
    public void LogException()
    {
        logger.Error(exception, "Could not proceed to operation X.");
    }

    [Benchmark]
    public void LogMany()
    {
        for(var i = 0; i < 250; i++)
            logger.Info("Receive message from {User} with destination {Destination}.", users[i % 10], destinations[i % 10]);
    }
}

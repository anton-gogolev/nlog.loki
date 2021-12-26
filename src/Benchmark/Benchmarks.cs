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

    private readonly string user = "Corentin Altepe";
    private readonly string destination = "127.0.0.1:8475";
    private readonly Exception exception = new("Ooops. Something went wrong!");

    [Benchmark]
    public void LogInfoWith2Variables()
    {
        logger.Info("Receive message from {User} with destination {Destination}.", user, destination);
    }

    [Benchmark]
    public void LogException()
    {
        logger.Error(exception, "Could not proceed to operation X.");
    }
}

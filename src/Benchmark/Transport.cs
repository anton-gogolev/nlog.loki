using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using NLog.Loki;
using NLog.Loki.Model;

namespace Benchmark;

[MemoryDiagnoser]
public class Transport
{
    [Params(CompressionLevel.NoCompression, CompressionLevel.Fastest, CompressionLevel.Optimal,
#if NET6_0_OR_GREATER
                CompressionLevel.SmallestSize
#endif
        )]
    public CompressionLevel CompressionLevel { get; set; }

    private readonly IList<LokiEvent> _manyLokiEvents;
    private readonly IList<LokiEvent> _lokiEvents = new List<LokiEvent> {
        new(
            new LokiLabels(new LokiLabel("env", "benchmark"), new LokiLabel("job", "WriteLogEventsAsync")),
            DateTime.Now,
            "Info|Receive message from \"A\" with destination \"B\".")};

    public Transport()
    {
        _manyLokiEvents = new List<LokiEvent>(100);
        for(var i = 0; i < 100; i++)
            _manyLokiEvents.Add(new LokiEvent(_lokiEvents[0].Labels, DateTime.Now, _lokiEvents[0].Line));
    }

    private HttpLokiTransport _transport;
    [GlobalSetup]
    public void GlobalSetup()
    {
        _transport = new(new LokiHttpClient(new HttpClient { BaseAddress = new Uri("http://localhost:3100") }), false, CompressionLevel);
    }

    [Benchmark]
    public async Task WriteLogEventsAsync()
    {
        await _transport.WriteLogEventsAsync(_lokiEvents).ConfigureAwait(false);
    }

    [Benchmark]
    public async Task ManyWriteLogEventsAsync()
    {
        await _transport.WriteLogEventsAsync(_manyLokiEvents).ConfigureAwait(false);
    }
}

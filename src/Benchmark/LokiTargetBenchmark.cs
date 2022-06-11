using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using NLog;
using NLog.Loki;
using NLog.Loki.Model;

namespace Benchmark;

[MemoryDiagnoser]
public class LokiTargetBenchmark
{
    private readonly LokiTarget _target = new();

    private readonly IList<LokiTargetLabel> _labels = new List<LokiTargetLabel> {
        new() { Name = "Label1", Layout = "MyLabel1Value" },
        new() { Name = "Label2", Layout = "MyLabel2Value" },
        new() { Name = "Label3", Layout = "MyLabel3Value" },
    };

    private List<LogEventInfo> _logs;

    [GlobalSetup]
    public void GlobalSetup()
    {
        // Generate up to N messages for logevent infos
        _logs = new(N);
        for(var i = 0; i < N; i++)
            _logs.Add(new LogEventInfo(LogLevel.Info, "MyLogger", RandomString(55)));
    }

    private static readonly Random Random = new();
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    [Params(1, 10, 100, 1000)]
    public int N { get; set; }

    [Benchmark]
    public async Task WriteAsyncTaskList_Old()
    {
        var lokiEvents = GetLokiEvents(_logs);
        using var jsonStreamContent = JsonContent.Create(lokiEvents);
        _ = await jsonStreamContent.ReadAsByteArrayAsync();
    }

    private IEnumerable<LokiEvent> GetLokiEvents(IEnumerable<LogEventInfo> logEvents)
    {
        foreach(var e in logEvents)
            yield return GetLokiEvent(e);
    }

    private LokiEvent GetLokiEvent(LogEventInfo logEvent)
    {
        var labels = new LokiLabels(_labels.Select(lbl => new LokiLabel(lbl.Name, lbl.Layout.Render(logEvent))));
        return new LokiEvent(labels, logEvent.TimeStamp, logEvent.ToString());
    }
}


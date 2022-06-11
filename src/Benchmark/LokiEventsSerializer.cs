using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using NLog.Loki.Model;

namespace Benchmark;

[MemoryDiagnoser]
public class LokiEventsSerializer
{
    private readonly MemoryStream _stream = new();
    private readonly JsonSerializerOptions _serializerOptionsWithoutOrdering = new();
    private readonly JsonSerializerOptions _serializerOptionsWithOrdering = new();

    private readonly LokiEvent _event;
    private IEnumerable<LokiEvent> _manyLokiEvents;

    public LokiEventsSerializer()
    {
        _event = new LokiEvent(
            new LokiLabels(new HashSet<LokiLabel> { new LokiLabel("env", "benchmark"), new LokiLabel("job", "WriteLogEventsAsync") }),
            DateTime.Now,
            "Info|Receive message from \"A\" with destination \"B\".");


        _serializerOptionsWithoutOrdering = new JsonSerializerOptions();
        _serializerOptionsWithoutOrdering.Converters.Add(new NLog.Loki.LokiEventsSerializer(orderWrites: false));
        _serializerOptionsWithoutOrdering.Converters.Add(new NLog.Loki.LokiEventSerializer());

        _serializerOptionsWithOrdering = new JsonSerializerOptions();
        _serializerOptionsWithOrdering.Converters.Add(new NLog.Loki.LokiEventsSerializer(orderWrites: true));
        _serializerOptionsWithOrdering.Converters.Add(new NLog.Loki.LokiEventSerializer());
    }

    [GlobalSetup]
    public void GlobalSetup()
    {
        var events = new List<LokiEvent>(N);
        for(var i = 0; i < N; i++)
            events.Add(new LokiEvent(_event.Labels, DateTime.Now, _event.Line));
        _manyLokiEvents = events;
    }

    [Params(/*1, 10,*/ 100, 1000)]
    public int N { get; set; }

    [Benchmark]
    public void SerializeManyEventsWithoutOrdering()
    {
        _stream.Position = 0;
        JsonSerializer.Serialize(_stream, _manyLokiEvents, _serializerOptionsWithoutOrdering);
    }

    //[Benchmark]
    //public void SerializeManyEventsWithOrdering()
    //{
    //    _stream.Position = 0;
    //    JsonSerializer.Serialize(_stream, _manyLokiEvents, _serializerOptionsWithOrdering);
    //}

    //[Benchmark]
    //public void SerializeSingleEvent()
    //{
    //    _stream.Position = 0;
    //    JsonSerializer.Serialize(_stream, _event, _serializerOptionsWithoutOrdering);
    //}
}

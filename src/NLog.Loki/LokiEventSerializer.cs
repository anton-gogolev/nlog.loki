using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using NLog.Loki.Model;

namespace NLog.Loki;

/// <summary>
/// Alternative to LokiStreamsJsonSerializer. Used to serialize log events to json
/// for loki before sending them to the push HTTP API. This implementation aims
/// at being more efficient than the currently used LokiStreamsJsonSerializer.
/// </summary>
internal class LokiEventSerializer : JsonConverter<IEnumerable<LokiEvent>>
{
    public override IEnumerable<LokiEvent> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotSupportedException("This converter only supports serializing to JSON.");
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<LokiEvent> value, JsonSerializerOptions options)
    {
        var streams = value.GroupBy(le => le.Labels);

        writer.WriteStartObject();
        writer.WriteStartArray("streams");

        foreach(var stream in streams)
        {
            writer.WriteStartObject();

            writer.WriteStartObject("stream");
            foreach(var label in stream.Key.Labels)
            {
                writer.WritePropertyName(label.Label);
                writer.WriteStringValue(label.Value);
            }
            writer.WriteEndObject();

            writer.WriteStartArray("values");
            foreach(var @event in stream.OrderBy(le => le.Timestamp))
            {
                writer.WriteStartArray();
                var timestamp = ToUnixTimeNs(@event.Timestamp).ToString("g", CultureInfo.InvariantCulture);
                writer.WriteStringValue(timestamp);
                writer.WriteStringValue(@event.Line);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static long ToUnixTimeNs(DateTime dateTime)
            => (dateTime.ToUniversalTime() - UnixEpoch).Ticks * 100;
}

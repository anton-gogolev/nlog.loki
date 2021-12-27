using System.Collections.Generic;
using System.Threading.Tasks;
using NLog.Loki.Model;

namespace NLog.Loki;

internal class NullLokiTransport : ILokiTransport
{
    public void WriteLogEvents(IEnumerable<LokiEvent> lokiEvents) { }
    public Task WriteLogEventsAsync(IEnumerable<LokiEvent> lokiEvents) => Task.CompletedTask;
}

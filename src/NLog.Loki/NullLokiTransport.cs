using System.Collections.Generic;
using System.Threading.Tasks;
using NLog.Loki.Model;

namespace NLog.Loki;

internal class NullLokiTransport : ILokiTransport
{
    public Task WriteLogEventsAsync(IEnumerable<LokiEvent> lokiEvents) => Task.CompletedTask;
    public Task WriteLogEventsAsync(LokiEvent lokiEvent) => Task.CompletedTask;
    public void Dispose()
    {
        // Nothing to dispose in this null implementation.
    }
}

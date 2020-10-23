using System.Collections.Generic;
using System.Threading.Tasks;
using NLog.Loki.Model;

namespace NLog.Loki
{
    public class NullLokiTransport : ILokiTransport
    {
        public void WriteLogEvents(IEnumerable<LokiEvent> lokiEvents)
        {
        }

        public Task WriteLogEventsAsync(IEnumerable<LokiEvent> lokiEvents)
        {
            return Task.CompletedTask;
        }
    }
}

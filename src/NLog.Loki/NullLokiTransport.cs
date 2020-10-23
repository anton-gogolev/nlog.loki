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
#if NETSTANDARD
            return Task.CompletedTask;
#else
            return Task.FromResult(0);
#endif

        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using NLog.Loki.Model;

namespace NLog.Loki
{
    public interface ILokiTransport
    {
        void WriteLogEvents(IEnumerable<LokiEvent> lokiEvents);

        Task WriteLogEventsAsync(IEnumerable<LokiEvent> lokiEvents);
    }
}

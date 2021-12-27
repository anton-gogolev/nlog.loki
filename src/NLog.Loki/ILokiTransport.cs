using System.Collections.Generic;
using System.Threading.Tasks;
using NLog.Loki.Model;

namespace NLog.Loki
{
    public interface ILokiTransport
    {
        Task WriteLogEventsAsync(IEnumerable<LokiEvent> lokiEvents);
        Task WriteLogEventsAsync(LokiEvent lokiEvent);
    }
}

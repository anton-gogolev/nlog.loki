using System.Collections.Generic;
using System.Threading.Tasks;
using NLog.Loki.Model;

namespace NLog.Loki.Impl
{
    internal interface IJsonSerializer
    {
        Task SerializeAsync(IEnumerable<LokiEvent> instance, JsonTextWriter jsonTextWriter);
    }
}

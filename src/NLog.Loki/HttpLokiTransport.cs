using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using NLog.Loki.Model;

namespace NLog.Loki
{
    /// <remarks>
    /// See https://grafana.com/docs/loki/latest/api/#examples-4
    /// </remarks>
    internal class HttpLokiTransport : ILokiTransport
    {
        private static readonly JsonSerializerOptions JsonOptions;

        static HttpLokiTransport()
        {
            JsonOptions = new JsonSerializerOptions();
            JsonOptions.Converters.Add(new LokiEventsSerializer());
            JsonOptions.Converters.Add(new LokiEventSerializer());
        }

        private readonly ILokiHttpClient lokiHttpClient;

        public HttpLokiTransport(ILokiHttpClient lokiHttpClient)
        {
            this.lokiHttpClient = lokiHttpClient;
        }

        public async Task WriteLogEventsAsync(IEnumerable<LokiEvent> lokiEvents)
        {
            using var jsonStreamContent = JsonContent.Create(lokiEvents, options: JsonOptions);
            _ = await lokiHttpClient.PostAsync("loki/api/v1/push", jsonStreamContent).ConfigureAwait(false);
        }

        public async Task WriteLogEventsAsync(LokiEvent lokiEvent)
        {
            using var jsonStreamContent = JsonContent.Create(lokiEvent, options: JsonOptions);
            _ = await lokiHttpClient.PostAsync("loki/api/v1/push", jsonStreamContent).ConfigureAwait(false);
        }
    }
}

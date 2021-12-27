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
            JsonOptions.Converters.Add(new LokiEventSerializer());
        }

        private readonly ILokiHttpClient lokiHttpClient;

        public HttpLokiTransport(ILokiHttpClient lokiHttpClient)
        {
            this.lokiHttpClient = lokiHttpClient;
        }

        public void WriteLogEvents(IEnumerable<LokiEvent> lokiEvents)
        {
            WriteLogEventsAsync(lokiEvents).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task WriteLogEventsAsync(IEnumerable<LokiEvent> lokiEvents)
        {
            using var jsonStreamContent = JsonContent.Create(lokiEvents, options: JsonOptions);
            _ = await lokiHttpClient.PostAsync("loki/api/v1/push", jsonStreamContent).ConfigureAwait(false);
        }
    }
}

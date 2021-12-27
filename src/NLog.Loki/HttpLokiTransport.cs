using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NLog.Loki.Impl;
using NLog.Loki.Model;

namespace NLog.Loki
{
    /// <remarks>
    /// See https://grafana.com/docs/loki/latest/api/#examples-4
    /// </remarks>
    internal class HttpLokiTransport : ILokiTransport
    {
        private readonly LokiStreamsJsonSerializer lokiStreamsJsonSerializer = new();
        private readonly MediaTypeHeaderValue contentType = new("application/json");
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
            using var jsonStreamContent = new JsonStreamContent(contentType, lokiEvents, lokiStreamsJsonSerializer);
            _ = await lokiHttpClient.PostAsync("loki/api/v1/push", jsonStreamContent).ConfigureAwait(false);
        }
    }
}

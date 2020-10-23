using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NLog.Loki.Impl;
using NLog.Loki.Model;

namespace NLog.Loki
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// See https://grafana.com/docs/loki/latest/api/#examples-4
    /// </remarks>
    public class HttpLokiTransport : ILokiTransport
    {
        private readonly LokiStreamsJsonSerializer lokiStreamsJsonSerializer = new LokiStreamsJsonSerializer();
        private readonly MediaTypeHeaderValue contentType = new MediaTypeHeaderValue("application/json");
        private readonly ILokiHttpClient lokiHttpClient;

        public HttpLokiTransport(Uri uri, ILokiHttpClient lokiHttpClient)
        {
            this.lokiHttpClient = lokiHttpClient;
        }

        public void WriteLogEvents(IEnumerable<LokiEvent> lokiEvents)
        {
            WriteLogEventsAsync(lokiEvents).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task WriteLogEventsAsync(IEnumerable<LokiEvent> lokiEvents)
        {
            using(var jsonStreamContent = new JsonStreamContent(contentType, lokiEvents, lokiStreamsJsonSerializer))
            {
                var httpResponse = await lokiHttpClient.PostAsync("loki/api/v1/push", jsonStreamContent);
            }
        }
    }
}

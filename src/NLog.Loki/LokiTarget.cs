using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NLog.Common;
using NLog.Config;
using NLog.Layouts;
using NLog.Loki.Model;
using NLog.Targets;

namespace NLog.Loki
{
    [Target("loki")]
    public class LokiTarget : AsyncTaskTarget
    {
        private readonly Lazy<ILokiTransport> lazyLokiTransport;

        [RequiredParameter]
        public Layout Endpoint { get; init; }

        [ArrayParameter(typeof(LokiTargetLabel), "label")]
        public IList<LokiTargetLabel> Labels { get; }

        private static Func<Uri, ILokiHttpClient> LokiHttpClientFactory { get; } = GetLokiHttpClient;

        public LokiTarget()
        {
            Labels = new List<LokiTargetLabel>();

            lazyLokiTransport =
                new Lazy<ILokiTransport>(
                    () => GetLokiTransport(Endpoint),
                    LazyThreadSafetyMode.ExecutionAndPublication);
        }

        protected override void Write(IList<AsyncLogEventInfo> logEvents)
        {
            var events = GetLokiEvents(logEvents.Select(alei => alei.LogEvent));
            lazyLokiTransport.Value.WriteLogEventsAsync(events).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        protected override Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken)
        {
            var @event = GetLokiEvent(logEvent);
            return lazyLokiTransport.Value.WriteLogEventsAsync(@event);
        }

        protected override Task WriteAsyncTask(IList<LogEventInfo> logEvents, CancellationToken cancellationToken)
        {
            var events = GetLokiEvents(logEvents);
            return lazyLokiTransport.Value.WriteLogEventsAsync(events);
        }

        private IEnumerable<LokiEvent> GetLokiEvents(IEnumerable<LogEventInfo> logEvents)
        {
            return logEvents.Select(GetLokiEvent);
        }

        private LokiEvent GetLokiEvent(LogEventInfo logEvent)
        {
            var labels =
                new LokiLabels(
                    Labels.Select(
                        ltl => new LokiLabel(ltl.Name, ltl.Layout.Render(logEvent))));

            var line = RenderLogEvent(Layout, logEvent);

            var @event = new LokiEvent(labels, logEvent.TimeStamp, line);

            return @event;
        }

        internal ILokiTransport GetLokiTransport(Layout endpoint)
        {
            var endpointUri = RenderLogEvent(endpoint, LogEventInfo.CreateNullEvent());
            if(Uri.TryCreate(endpointUri, UriKind.Absolute, out var uri))
            {
                if(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                {
                    var lokiHttpClient = LokiHttpClientFactory(uri);
                    var httpLokiTransport = new HttpLokiTransport(lokiHttpClient);

                    return httpLokiTransport;
                }
            }

            InternalLogger.Warn("Unable to create a valid Loki Endpoint URI from '{0}'", endpoint);

            var nullLokiTransport = new NullLokiTransport();

            return nullLokiTransport;
        }

        internal static ILokiHttpClient GetLokiHttpClient(Uri uri)
        {
            var httpClient = new HttpClient { BaseAddress = uri };
            var lokiHttpClient = new LokiHttpClient(httpClient);

            return lokiHttpClient;
        }
    }
}

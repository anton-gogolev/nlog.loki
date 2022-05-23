using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        private readonly Lazy<ILokiTransport> _lazyLokiTransport;

        [RequiredParameter]
        public Layout Endpoint { get; set; }

        public Layout Username { get; set; }

        public Layout Password { get; set; }

        /// <summary>
        /// Orders the logs by timestamp before sending them to Loki.
        /// Required as <see langword="true"/> before Loki v2.4. Leave as <see langword="false"/> if you are running Loki v2.4 or above.
        /// See <see href="https://grafana.com/docs/loki/latest/configuration/#accept-out-of-order-writes"/>.
        /// </summary>
        public bool OrderWrites { get; set; } = true;

        /// <summary>
        /// Defines if the HTTP messages sent to Loki must be gzip compressed, and with which compression level.
        /// Possible values: NoCompression (default), Optimal, Fastest and SmallestSize (.NET 6 support only).
        /// </summary>
        public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.NoCompression;

        [ArrayParameter(typeof(LokiTargetLabel), "label")]
        public IList<LokiTargetLabel> Labels { get; }

        private static Func<Uri, string, string, ILokiHttpClient> LokiHttpClientFactory { get; } = GetLokiHttpClient;

        public LokiTarget()
        {
            Labels = new List<LokiTargetLabel>();

            _lazyLokiTransport = new Lazy<ILokiTransport>(
                () => GetLokiTransport(Endpoint, Username, Password, OrderWrites),
                LazyThreadSafetyMode.ExecutionAndPublication);
            InitializeTarget();
        }

        protected override Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken)
        {
            var @event = GetLokiEvent(logEvent);
            return _lazyLokiTransport.Value.WriteLogEventsAsync(@event);
        }

        protected override Task WriteAsyncTask(IList<LogEventInfo> logEvents, CancellationToken cancellationToken)
        {
            var events = GetLokiEvents(logEvents);
            return _lazyLokiTransport.Value.WriteLogEventsAsync(events);
        }

        private IEnumerable<LokiEvent> GetLokiEvents(IEnumerable<LogEventInfo> logEvents)
        {
            return logEvents.Select(GetLokiEvent);
        }

        private LokiEvent GetLokiEvent(LogEventInfo logEvent)
        {
            var labels = new LokiLabels(
                Labels.Select(ltl => new LokiLabel(ltl.Name, ltl.Layout.Render(logEvent))));

            var line = RenderLogEvent(Layout, logEvent);

            var @event = new LokiEvent(labels, logEvent.TimeStamp, line);

            return @event;
        }

        internal ILokiTransport GetLokiTransport(
            Layout endpoint, Layout username, Layout password, bool orderWrites)
        {
            var endpointUri = RenderLogEvent(endpoint, LogEventInfo.CreateNullEvent());
            var usr = RenderLogEvent(username, LogEventInfo.CreateNullEvent());
            var pwd = RenderLogEvent(password, LogEventInfo.CreateNullEvent());

            if(Uri.TryCreate(endpointUri, UriKind.Absolute, out var uri))
            {
                if(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                    return new HttpLokiTransport(LokiHttpClientFactory(uri, usr, pwd), orderWrites, CompressionLevel);
            }

            InternalLogger.Warn("Unable to create a valid Loki Endpoint URI from '{0}'", endpoint);
            return new NullLokiTransport();
        }

        internal static ILokiHttpClient GetLokiHttpClient(Uri uri, string username, string password)
        {
            var httpClient = new HttpClient { BaseAddress = uri };
            if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
            return new LokiHttpClient(httpClient);
        }

        private bool _isDisposed;
        protected override void Dispose(bool isDisposing)
        {
            if(!_isDisposed)
            {
                if(isDisposing)
                {
                    if(_lazyLokiTransport.IsValueCreated)
                        _lazyLokiTransport.Value.Dispose();
                }
                _isDisposed = true;
            }
            base.Dispose(isDisposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    internal class LokiStreamsJsonSerializer : IJsonSerializer
    {
        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public async Task SerializeAsync(object instance, JsonTextWriter jsonTextWriter)
        {
            var j = jsonTextWriter;
            var events = (IEnumerable<LokiEvent>)instance;

            var streams =
                events.GroupBy(le => le.Labels);

            using(await j.WriteObjectAsync())
            {
                using(await j.WriteArrayAsync("streams"))
                {
                    foreach(var stream in streams)
                    {
                        using(await j.WriteObjectAsync())
                        {
                            using(await j.WriteObjectAsync("stream"))
                            {
                                foreach(var label in stream.Key.Labels)
                                    await j.WritePropertyAsync(label.Label, label.Value);
                            }

                            using(await j.WriteArrayAsync("values"))
                            {
                                foreach(var @event in stream.OrderBy(le => le.Timestamp))
                                {
                                    using(await j.WriteArrayAsync())
                                    {
                                        var timestamp =
                                            ToUnixTimeNs(@event.Timestamp).
                                                ToString("g", CultureInfo.InvariantCulture);

                                        await j.WriteValueAsync(timestamp);
                                        await j.WriteValueAsync(@event.Line);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static long ToUnixTimeNs(DateTime dateTime)
        {
            var unixTimeNs = (long)(dateTime.ToUniversalTime() - UnixEpoch).Ticks * 100;
            return unixTimeNs;
        }
    }
}

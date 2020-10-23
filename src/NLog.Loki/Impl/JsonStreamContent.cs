using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NLog.Loki.Impl
{
    internal class JsonStreamContent : HttpContent
    {
        private readonly UTF8Encoding utf8Encoding = new UTF8Encoding(false);
        private readonly object instance;
        private readonly IJsonSerializer jsonSerializer;

        public JsonStreamContent(MediaTypeHeaderValue contentType, object instance, IJsonSerializer jsonSerializer)
        {
            this.instance = instance;
            this.jsonSerializer = jsonSerializer;

            Headers.ContentType = contentType;
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            using(var streamWriter = new StreamWriter(stream, utf8Encoding, 4096, true))
            using(var jsonWriter = new JsonTextWriter(streamWriter))
            {
                await jsonSerializer.SerializeAsync(instance, jsonWriter);
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;

            return false;
        }
    }
}

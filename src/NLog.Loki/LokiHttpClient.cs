using System.Net.Http;
using System.Threading.Tasks;

namespace NLog.Loki
{
    internal class LokiHttpClient : ILokiHttpClient
    {
        private readonly HttpClient httpClient;

        public LokiHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent httpContent)
        {
            return httpClient.PostAsync(requestUri, httpContent);
        }
    }
}

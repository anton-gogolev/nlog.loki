using System.Net.Http;
using System.Threading.Tasks;

namespace NLog.Loki
{
    public class LokiHttpClient : ILokiHttpClient
    {
        private readonly HttpClient httpClient;

        public LokiHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent httpContent)
        {
            return await httpClient.PostAsync(requestUri, httpContent);
        }
    }
}

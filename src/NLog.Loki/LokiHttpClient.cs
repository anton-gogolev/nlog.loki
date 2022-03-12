using System.Net.Http;
using System.Threading.Tasks;

namespace NLog.Loki;

internal sealed class LokiHttpClient : ILokiHttpClient
{
    private readonly HttpClient _httpClient;

    public LokiHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent httpContent)
    {
        return _httpClient.PostAsync(requestUri, httpContent);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}

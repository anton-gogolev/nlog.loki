using System.Net.Http;
using System.Threading.Tasks;

namespace NLog.Loki
{
    public interface ILokiHttpClient
    {
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent httpContent);
    }
}

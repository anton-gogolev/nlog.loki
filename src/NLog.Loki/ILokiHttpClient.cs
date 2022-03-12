using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NLog.Loki
{
    internal interface ILokiHttpClient : IDisposable
    {
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent httpContent);
    }
}

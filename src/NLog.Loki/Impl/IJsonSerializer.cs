using System.Threading.Tasks;

namespace NLog.Loki.Impl
{
    internal interface IJsonSerializer
    {
        Task SerializeAsync(object instance, JsonTextWriter jsonTextWriter);
    }
}

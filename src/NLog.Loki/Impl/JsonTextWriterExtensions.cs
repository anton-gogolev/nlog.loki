using System;
using System.Threading.Tasks;

namespace NLog.Loki.Impl
{
    internal static class JsonTextWriterExtensions
    {
        internal class JsonBlock : IDisposable
        {
            private readonly Action close;

            public JsonBlock(Action close)
            {
                this.close = close;
            }

            void IDisposable.Dispose()
            {
                close();
            }
        }

        public static async Task WritePropertyAsync(this JsonTextWriter jsonTextWriter, string name, object value)
        {
            await jsonTextWriter.WritePropertyNameAsync(name);
            await jsonTextWriter.WriteValueAsync(value);
        }

        public static async Task WritePropertyAsync(this JsonTextWriter jsonTextWriter, string name, string value)
        {
            await jsonTextWriter.WritePropertyNameAsync(name);
            await jsonTextWriter.WriteValueAsync(value);
        }

        public static async Task WritePropertyAsync(this JsonTextWriter jsonTextWriter, string name, int value)
        {
            await jsonTextWriter.WritePropertyNameAsync(name);
            await jsonTextWriter.WriteValueAsync(value);
        }

        public static async Task WritePropertyAsync(this JsonTextWriter jsonTextWriter, string name, float value)
        {
            await jsonTextWriter.WritePropertyNameAsync(name);
            await jsonTextWriter.WriteValueAsync(value);
        }

        public static async Task WritePropertyAsync(this JsonTextWriter jsonTextWriter, string name, double value)
        {
            await jsonTextWriter.WritePropertyNameAsync(name);
            await jsonTextWriter.WriteValueAsync(value);
        }

        public static async Task<IDisposable> WriteObjectAsync(this JsonTextWriter jsonTextWriter, string name = null)
        {
            if(!string.IsNullOrWhiteSpace(name))
                await jsonTextWriter.WritePropertyNameAsync(name);

            await jsonTextWriter.WriteStartObjectAsync();
            return new JsonBlock(jsonTextWriter.WriteEndObject);
        }

        public static async Task<IDisposable> WriteArrayAsync(this JsonTextWriter jsonTextWriter, string name = null)
        {
            if(!string.IsNullOrWhiteSpace(name))
                await jsonTextWriter.WritePropertyNameAsync(name);

            await jsonTextWriter.WriteStartArrayAsync();
            return new JsonBlock(jsonTextWriter.WriteEndArray);
        }
    }
}

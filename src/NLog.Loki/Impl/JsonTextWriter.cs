using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace NLog.Loki.Impl
{
    internal class JsonTextWriter : IDisposable
    {
        private readonly TextWriter textWriter;
        
        private bool pendingComma;

        public JsonTextWriter(TextWriter textWriter)
        {
            this.textWriter = textWriter;
        }

        public void Dispose()
        {
            textWriter?.Dispose();
        }

        public void WriteStartObject()
        {
            WritePendingComma();

            textWriter.Write("{");
        }

        public async Task WriteStartObjectAsync()
        {
            await WritePendingCommaAsync();

            await textWriter.WriteAsync("{");
        }

        public void WriteEndObject()
        {
            textWriter.Write("}");

            pendingComma = true;
        }

        public async Task WriteEndObjectAsync()
        {
            await textWriter.WriteAsync("}");

            pendingComma = true;
        }

        public async Task WriteStartArrayAsync()
        {
            await WritePendingCommaAsync();

            await textWriter.WriteAsync("[");

            pendingComma = false;
        }

        public void WriteEndArray()
        {
            textWriter.Write("]");

            pendingComma = true;
        }

        public async Task WriteEndArrayAsync()
        {
            await textWriter.WriteAsync("]");

            pendingComma = true;
        }

        public async Task WritePropertyNameAsync(string name)
        {
            await WritePendingCommaAsync();

            textWriter.Write("\"{0}\":", name);
        }

        public async Task WriteValueAsync(string value)
        {
            await WritePendingCommaAsync();

            if(value == null)
            {
                await WriteNullValueAsync();

                pendingComma = true;

                return;
            }

            await textWriter.WriteAsync("\"");

            //
            // See http://www.ecma-international.org/publications/files/ECMA-ST/ECMA-404.pdf
            foreach(var c in value)
            {
                var i = "\\\"/\b\f\n\r\t".IndexOf(c);
                if(i > -1)
                {
                    await textWriter.WriteAsync($"\\{"\\\"/bfnrt"[i]}");
                }
                else if(c <= 0x1f || c > 255)
                {
                    await textWriter.WriteAsync($"\\u{(uint)c:x4}");
                }
                else
                    await textWriter.WriteAsync(c);
            }

            await textWriter.WriteAsync("\"");

            pendingComma = true;
        }

        public async Task WriteValueAsync(int value)
        {
            await WriteRawValueAsync(value.ToString(CultureInfo.InvariantCulture));
        }

        public async Task WriteValueAsync(float value)
        {
            await WriteRawValueAsync(value.ToString(CultureInfo.InvariantCulture));
        }

        public async Task WriteValueAsync(double value)
        {
            await WriteRawValueAsync(value.ToString(CultureInfo.InvariantCulture));
        }

        public async Task WriteValueAsync(object value)
        {
            await WritePendingCommaAsync();

            if(value == null)
            {
                await WriteNullValueAsync();
                return;
            }

            if(value is string s)
            {
                await WriteValueAsync(s);
                return;
            }

            if(value is bool b)
            {
                await WriteValueAsync(b.ToString().ToLowerInvariant());
                return;
            }

            if(value.GetType().IsNumericType() && value is IFormattable formattable)
            {
                await WriteRawValueAsync(formattable.ToString(null, CultureInfo.InvariantCulture));
                return;
            }

            await WriteNullValueAsync();
        }

        public async Task WriteNullValueAsync()
        {
            await WriteRawValueAsync("null");
        }

        internal async Task WriteRawValueAsync(string value)
        {
            await WritePendingCommaAsync();

            await textWriter.WriteAsync(value);

            pendingComma = true;
        }

        internal void WritePendingComma()
        {
            if(!pendingComma)
                return;

            textWriter.Write(",");
            pendingComma = false;
        }

        internal async Task WritePendingCommaAsync()
        {
            if(!pendingComma)
                return;

            await textWriter.WriteAsync(",");
            pendingComma = false;
        }
    }
}

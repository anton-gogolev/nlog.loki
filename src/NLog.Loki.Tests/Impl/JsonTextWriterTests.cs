using System.IO;
using System.Text;
using System.Threading.Tasks;
using NLog.Loki.Impl;
using NUnit.Framework;

namespace NLog.Loki.Tests.Impl
{
    [TestFixture]
    public class JsonTextWriterTests
    {
        [Test]
        public async Task WriteJson()
        {
            var stringBuilder = new StringBuilder();

            using(var stringWriter = new StringWriter(stringBuilder))
            using(var jsonTextWriter = new JsonTextWriter(stringWriter))
            {
                await jsonTextWriter.WriteStartObjectAsync();

                await jsonTextWriter.WritePropertyNameAsync("hello");
                await jsonTextWriter.WriteValueAsync("\r\n\t\"\r\" hello/utf8\\=✓\U0001D11E\U0001F602\U0001D505");

                await jsonTextWriter.WritePropertyNameAsync("answer");
                await jsonTextWriter.WriteValueAsync(42);

                await jsonTextWriter.WritePropertyNameAsync("long_answer");
                await jsonTextWriter.WriteValueAsync(42424242L);

                await jsonTextWriter.WritePropertyNameAsync("precise_answer");
                await jsonTextWriter.WriteValueAsync(12234.543d);

                await jsonTextWriter.WritePropertyNameAsync("null_answer");
                await jsonTextWriter.WriteNullValueAsync();

                using(await jsonTextWriter.WriteObjectAsync("nested"))
                {
                    await jsonTextWriter.WritePropertyAsync("x", 1);
                }

                await jsonTextWriter.WriteEndObjectAsync();
            }

            Assert.AreEqual(
                @"{""hello"":""\r\n\t\""\r\"" hello\/utf8\\=\u2713\ud834\udd1e\ud83d\ude02\ud835\udd05"",""answer"":42,""long_answer"":4.242424E+07,""precise_answer"":12234.543,""null_answer"":null,""nested"":{""x"":1}}",
                stringBuilder.ToString());
        }
    }
}

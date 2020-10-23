using System.IO;
using System.Text;
using System.Threading.Tasks;
using NLog.Loki.Impl;
using NUnit.Framework;

namespace NLog.Loki.Tests.Impl
{
    [TestFixture]
    public class JsonTextWriterExtensionsTests
    {
        [Test]
        public async Task WriteAsync()
        {
            var stringBuilder = new StringBuilder();
            using(var textWriter = new StringWriter(stringBuilder))
            using(var j = new JsonTextWriter(textWriter))
            {
                using(await j.WriteObjectAsync())
                {
                    using(await j.WriteObjectAsync("loki"))
                    {
                        using(await j.WriteArrayAsync("streams"))
                        {
                            await j.WriteValueAsync("a");
                            await j.WriteValueAsync(1);
                            await j.WriteValueAsync(1.23);
                            await j.WriteValueAsync(1L);

                            using(await j.WriteObjectAsync())
                            {
                                await j.WritePropertyAsync("v", 1);
                            }

                            using(await j.WriteArrayAsync())
                            {
                                await j.WriteNullValueAsync();
                            }
                        }
                    }
                }
            }

            Assert.AreEqual(
                "{\"loki\":{\"streams\":[\"a\",1,1.23,1,{\"v\":1},[null]]}}",
                stringBuilder.ToString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using NLog.Loki.Impl;
using NLog.Loki.Model;
using NUnit.Framework;

namespace NLog.Loki.Tests;

public class HttpLokiTransportTests
{
    [Test]
    public async Task SerializeMessageToHttpLoki()
    {
        // Prepare the events to be sent to loki
        var date = new DateTime(2021, 12, 27, 9, 48, 26, DateTimeKind.Utc);
        var events = new List<LokiEvent> {
            new(new LokiLabels(new LokiLabel("env", "unittest"), new LokiLabel("job", "Job1")),
                date, "Info|Receive message from A with destination B."),
            new(new LokiLabels(new LokiLabel("env", "unittest"), new LokiLabel("job", "Job1")),
                date, "Info|Another event has occured here."),
            new(new LokiLabels(new LokiLabel("env", "unittest"), new LokiLabel("job", "Job1")),
                date, "Info|Event from another stream."),
        };

        // Configure the ILokiHttpClient such that we intercept the JSON content and simulate an OK response from Loki.
        string serializedJsonMessage = null;
        var httpClient = new Mock<ILokiHttpClient>();
        _ = httpClient.Setup(c => c.PostAsync("loki/api/v1/push", It.IsAny<JsonStreamContent>()))
            .Returns<string, JsonStreamContent>(async (s, json) =>
            {
                // Intercept the json content so that we can verify it.
                serializedJsonMessage = await json.ReadAsStringAsync().ConfigureAwait(false);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        // Send the logging request
        var transport = new HttpLokiTransport(httpClient.Object);
        await transport.WriteLogEventsAsync(events).ConfigureAwait(false);

        // Verify the json message format
        Assert.AreEqual(
            "{\"streams\":[{\"stream\":{\"env\":\"unittest\",\"job\":\"Job1\"},\"values\":[[\"1640598506000000000\",\"Info|Receive message from A with destination B.\"],[\"1640598506000000000\",\"Info|Another event has occured here.\"],[\"1640598506000000000\",\"Info|Event from another stream.\"]]}]}",
            serializedJsonMessage);
    }

    [Test]
    public async Task SerializeMessageToHttpLokiNewSerializer()
    {
        // Prepare the events to be sent to loki
        var date = new DateTime(2021, 12, 27, 9, 48, 26, DateTimeKind.Utc);
        var events = new List<LokiEvent> {
            new(new LokiLabels(new LokiLabel("env", "unittest"), new LokiLabel("job", "Job1")),
                date, "Info|Receive message from A with destination B."),
            new(new LokiLabels(new LokiLabel("env", "unittest"), new LokiLabel("job", "Job1")),
                date, "Info|Another event has occured here."),
            new(new LokiLabels(new LokiLabel("env", "unittest"), new LokiLabel("job", "Job1")),
                date, "Info|Event from another stream."),
        };

        // Configure the ILokiHttpClient such that we intercept the JSON content and simulate an OK response from Loki.
        string serializedJsonMessage = null;
        var httpClient = new Mock<ILokiHttpClient>();
        _ = httpClient.Setup(c => c.PostAsync("loki/api/v1/push", It.IsAny<HttpContent>()))
            .Returns<string, HttpContent>(async (s, json) =>
            {
                // Intercept the json content so that we can verify it.
                serializedJsonMessage = await json.ReadAsStringAsync().ConfigureAwait(false);
                return new HttpResponseMessage(HttpStatusCode.OK);
            });

        // Send the logging request
        var transport = new HttpLokiTransport(httpClient.Object);
        await transport.WriteLogEventsWitNewSerializerAsync(events).ConfigureAwait(false);

        // Verify the json message format
        Assert.AreEqual(
            "{\"streams\":[{\"stream\":{\"env\":\"unittest\",\"job\":\"Job1\"},\"values\":[[\"1640598506000000000\",\"Info|Receive message from A with destination B.\"],[\"1640598506000000000\",\"Info|Another event has occured here.\"],[\"1640598506000000000\",\"Info|Event from another stream.\"]]}]}",
            serializedJsonMessage);
    }
}

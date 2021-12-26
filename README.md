# NLog Loki Target

![CI](https://github.com/anton-gogolev/nlog.loki/workflows/CI/badge.svg?branch=master)

This is an [NLog](https://nlog-project.org/) target that sends messages to [Loki](https://grafana.com/oss/loki/) using Loki's HTTP Push API.

> Loki is a horizontally-scalable, highly-available, multi-tenant log aggregation system inspired by Prometheus. It is designed to be very cost effective and easy to operate.

## Installation

The NLog.Loki NuGet package can be found [here](https://www.nuget.org/packages/NLog.Loki). You can install it via one of the following commands below:

NuGet command:

    Install-Package NLog.Loki

.NET Core CLI command:

    dotnet add package NLog.Loki

## Usage

Under .NET Core, [remember to register](https://github.com/nlog/nlog/wiki/Register-your-custom-component) `NLog.Loki` as an extension assembly with NLog:

```xml
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

  <extensions>
    <add assembly="NLog.Loki" />
  </extensions>

</nlog>
```

You can now add a Loki target [to your configuration file](https://github.com/nlog/nlog/wiki/Tutorial#Configure-NLog-Targets-for-output):

```xml
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

  <target name="loki" xsi:type="loki" endpoint="http://localhost:3100">
    <!-- Loki requires at least one label associated with the log stream. 
    Make sure you specify at least one label here. -->
    <label name="level" layout="${level:lowercase=true}" />
    <label name="server" layout="${hostname:lowercase=true}" />
  </target>

  <rules>
    <logger name="*" minlevel="Info" writeTo="loki" />
  </rules>

</nlog>
```

The `@endpoint` attribute is a [Layout](https://github.com/NLog/NLog/wiki/Layouts) that must ultimately resolve to a fully-qualified absolute URL of the Loki Server when running in a [Single Proccess Mode](https://grafana.com/docs/loki/latest/overview/#modes-of-operation) or of the Loki Distributor when running in [Microservices Mode](https://grafana.com/docs/loki/latest/overview/#distributor). When an invalid URI is encountered, all log messages are silently discarded.

`label` elements can be used to enrich messages with additional [labels](https://grafana.com/docs/loki/latest/design-documents/labels/). `label/@layout` support usual NLog layout renderers.

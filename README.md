# NLog Loki Target

This is an [NLog](https://nlog-project.org/) target that sends messages to [Loki](https://grafana.com/oss/loki/).

## Using NLog.Loki.LokiTarget

Under .NET Core, [remember to register](https://github.com/nlog/nlog/wiki/Register-your-custom-component) `NLog.Loki` as an extension assembly with NLog:

```
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <extensions>
    <add assembly="NLog.Loki" />
  </extensions>
</nlog>
```

You can now add a Loki target [to your configuration file](https://github.com/nlog/nlog/wiki/Tutorial#Configure-NLog-Targets-for-output):

```
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <target name="loki" xsi:type="loki" endpoint="%LOKI_ENDPOINT_URI%">
    <label name="level" layout="${level:lowercase=true}" />
    <label name="server" layout="${hostname:lowercase=true}" />
  </target>

  <rules>
    <logger name="*" minlevel="Info" writeTo="loki" />
  </rules>
</nlog>
```

The `@endpoint` attribute must contain a fully-qualified absolute URL of the Loki server. Environment variables are fully supported. `label` elements can be used to enrich messages with additional labels. `label/@layout` support usual NLog layout renderers.

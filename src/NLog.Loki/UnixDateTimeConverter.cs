using System;

namespace NLog.Loki;
internal static class UnixDateTimeConverter
{
    private static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public static long ToUnixTimeNs(DateTime dateTime) => (dateTime.ToUniversalTime() - UnixEpoch).Ticks * 100;
}

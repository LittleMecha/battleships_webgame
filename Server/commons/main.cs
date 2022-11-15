using System;

public class Commons
{
    public static long CurrentTimestamp()
    {
        DateTimeOffset dto = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
        return dto.ToUnixTimeSeconds();
    }
    public static string GetRandomID()
    {
        long ts = Commons.CurrentTimestamp();
        Random rnd = new Random();
        return String.Format("{0}{1}", ts, rnd.Next(1000, 9999));
    }
}
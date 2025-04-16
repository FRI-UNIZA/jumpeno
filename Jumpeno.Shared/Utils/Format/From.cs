namespace Jumpeno.Shared.Utils;

public static class From {
    // Time units -------------------------------------------------------------------------------------------------------------------------
    public static int SToMS(int value) => value * 1000;
    public static double SToMS(double value) => value * 1000;

    public static int MinToMS(int value) => SToMS(value) * 60;
    public static double MinToMS(double value) => SToMS(value) * 60;

    public static int HourToMS(int value) => MinToMS(value) * 60;
    public static double HourToMS(double value) => MinToMS(value) * 60;

    public static int DayToMS(int value) => HourToMS(value) * 24;
    public static double DayToMS(double value) => HourToMS(value) * 24;

    // DateTime ---------------------------------------------------------------------------------------------------------------------------
    public static DateTime UnixToDateTime(long value) => DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime;
    public static long DateTimeToUnix(DateTime value) => ((DateTimeOffset) value).ToUnixTimeSeconds();
}

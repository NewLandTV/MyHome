public static class Logger
{
    public static void Log(string message)
    {
        string dateTime = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]";

        Console.WriteLine($"{dateTime}[Log] {message}");
    }
}

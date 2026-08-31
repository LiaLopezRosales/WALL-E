using Avalonia;
using System;
using System.IO;

namespace Wall_E.UI.Avalonia;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            try
            {
                string st = e.ExceptionObject is Exception ex ? (ex.StackTrace ?? "") : "";
                File.AppendAllText("/tmp/wall-e-crash.log",
                    $"[{DateTime.Now:HH:mm:ss}] UNHANDLED: {e.ExceptionObject}\n{st}\n\n");
            }
            catch { }
        };

        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            try { File.AppendAllText("/tmp/wall-e-crash.log", $"[{DateTime.Now:HH:mm:ss}] TOP-LEVEL:\n{ex}\n\n"); } catch { }
            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}

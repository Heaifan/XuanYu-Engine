using Avalonia;

namespace XYUI.Avalonia.Gallery;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Console.WriteLine($"[Gallery] Main started, args: {string.Join(" ", args)}");
        InitialComponentId = args.FirstOrDefault(x => x.StartsWith("--component=", StringComparison.Ordinal))?
            .Split('=', 2)[1];
        ScreenshotPath = args.FirstOrDefault(x => x.StartsWith("--screenshot=", StringComparison.Ordinal))?
            .Split('=', 2)[1];
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    internal static string? InitialComponentId { get; private set; }
    internal static string? ScreenshotPath { get; private set; }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect();
}

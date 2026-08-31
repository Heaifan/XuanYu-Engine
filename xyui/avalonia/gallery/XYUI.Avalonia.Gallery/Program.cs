using Avalonia;

namespace XYUI.Avalonia.Gallery;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        InitialComponentId = args.FirstOrDefault(x => x.StartsWith("--component=", StringComparison.Ordinal))?
            .Split('=', 2)[1];
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    internal static string? InitialComponentId { get; private set; }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect();
}

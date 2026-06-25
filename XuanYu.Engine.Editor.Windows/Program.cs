using Avalonia;
using Avalonia.Fonts.Inter;

namespace XuanYu.Engine.Editor.Windows;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        EditorConsoleOutput.AttachParentConsole();
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}

using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Fonts.Inter;
using EditorUiApp = XuanYu.Editor.UI.App;
using XuanYu.Editor.UI;

namespace XuanYu.Editor.App;

internal static class Program
{
    [DllImport("kernel32", SetLastError = true)]
    static extern bool AttachConsole(int dwProcessId);

    [STAThread]
    public static void Main(string[] args)
    {
        AttachConsole(-1);
        F1ForensicTrace.Version(typeof(Program).Assembly, typeof(EditorUiApp).Assembly);
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        var factory = EditorCompositionRoot.CreateSurfaceBridgeFactory();
        return AppBuilder.Configure(() => new EditorUiApp(factory))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}

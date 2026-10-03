using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Fonts.Inter;
using XuanYu.Core.Diagnostics;
using XuanYu.Editor.UI;
using EditorUiApp = XuanYu.Editor.UI.App;

namespace XuanYu.Editor.App;

internal static class Program
{
    [DllImport("kernel32", SetLastError = true)]
    static extern bool AttachConsole(int dwProcessId);

    [STAThread]
    public static void Main(string[] args)
    {
        ViewportProbe.Initialize();
        AttachConsole(-1);
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        var factory = EditorCompositionRoot.CreateSurfaceBridgeFactory();
        return AppBuilder.Configure(() => new EditorUiApp(factory,
                EditorCompositionRoot.CreateEditorState))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}

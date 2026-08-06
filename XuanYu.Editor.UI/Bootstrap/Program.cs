using System.Runtime.InteropServices;
using Avalonia;

namespace XuanYu.Editor.UI;

internal static class Program
{
    // WinExe 进程默认无控制台；AttachConsole(-1) 继承父终端（dotnet run 控制台），
    // 使所有 Console.WriteLine（VulkanBridge/Device/Swapchain/Capabilities/Builder/Selector 日志）可见。
    [DllImport("kernel32", SetLastError = true)]
    static extern bool AttachConsole(int dwProcessId);

    [STAThread]
    public static void Main(string[] args)
    {
        AttachConsole(-1); // ATTACH_PARENT_PROCESS
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }
}

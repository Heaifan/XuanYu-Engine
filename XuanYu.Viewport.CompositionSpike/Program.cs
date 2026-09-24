using Avalonia;
using Avalonia.Fonts.Inter;
using Avalonia.Logging;
using Avalonia.Vulkan;
using Avalonia.Win32;

namespace XuanYu.Viewport.CompositionSpike;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .With(new Win32PlatformOptions { RenderingMode = [Win32RenderingMode.Vulkan] })
        .With(new VulkanOptions { VulkanInstanceCreationOptions = new VulkanInstanceCreationOptions { UseDebug = true } })
        .WithInterFont().AfterPlatformServicesSetup(b =>
            Console.WriteLine($"[A1.5] Avalonia={typeof(AvaloniaObject).Assembly.GetName().Version}; RenderingSubsystem={b.RenderingSubsystemName}"))
        .LogToTrace(LogEventLevel.Debug, "Vulkan");
}

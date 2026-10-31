using Avalonia;
using Avalonia.Headless;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

// Headless App 构建器：复用 Gallery App（资源合并逻辑同步被测试）
public static class XyuiTestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}

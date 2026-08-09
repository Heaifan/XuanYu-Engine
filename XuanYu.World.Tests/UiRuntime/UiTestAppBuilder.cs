using Avalonia;
using Avalonia.Headless;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public static class UiTestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Xunit;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

public sealed class GalleryRuntimeTests
{
    [Fact]
    public void AppThemeAndMainWindowCanInitializeTogether()
    {
        var lifetime = new ClassicDesktopStyleApplicationLifetime();
        AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions())
            .SetupWithLifetime(lifetime);
        Assert.NotNull(lifetime.MainWindow);
        Assert.Contains("XYUI.Avalonia Gallery", lifetime.MainWindow.Title);
        lifetime.MainWindow.Close();
    }
}

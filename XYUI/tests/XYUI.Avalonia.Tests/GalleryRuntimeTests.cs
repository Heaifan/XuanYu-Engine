using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Headless;
using Xunit;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

public sealed class GalleryRuntimeTests
{
    [Fact]
    public async Task AppThemeAndMainWindowCanInitializeTogether()
    {
        using var session = HeadlessUnitTestSession.StartNew(typeof(TestAppBuilder));
        await session.Dispatch(() =>
        {
            var app = new App();
            app.Initialize();
            var window = new MainWindow();
            Assert.NotNull(app.Styles);
            Assert.Contains("XYUI.Avalonia Gallery", window.Title);
            window.Close();
        }, CancellationToken.None);
    }

    private static class TestAppBuilder
    {
        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder.Configure<App>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
}

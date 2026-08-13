using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Xunit;
using XYUI.Avalonia.Controls;
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
        var button = new XYButton { Content = "运行" };
        var field = new XYTextField { Text = "内容" };
        var badge = new XYBadge { Content = "Accent" };
        lifetime.MainWindow.Content = new StackPanel { Children = { button, field, badge } };
        lifetime.MainWindow.Show();
        lifetime.MainWindow.UpdateLayout();
        Assert.NotNull(button.Background);
        Assert.True(button.Padding.Left + button.Padding.Right > 0);
        Assert.True(button.MinHeight > 0);
        Assert.True(field.BorderThickness.Left > 0);
        Assert.True(field.Padding.Left + field.Padding.Right > 0);
        Assert.NotNull(badge.Background);
        lifetime.MainWindow.Close();
    }
}

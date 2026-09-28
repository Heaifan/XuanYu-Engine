using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class HeadlessInputInfrastructureTests
{
    readonly UiHeadlessFixture _fixture;
    public HeadlessInputInfrastructureTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Official_headless_button_click_executes()
    {
        var count = _fixture.Run(() => Click(false));
        Assert.Equal(1, count);
    }

    [Fact]
    public void Harness_headless_button_click_executes()
    {
        var count = _fixture.Run(() => Click(true));
        Assert.Equal(1, count);
    }

    static int Click(bool useHarness)
    {
        var count = 0; var button = new Button { Content = "Click", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch };
        button.Click += (_, _) => count++;
        var window = new Window { Width = 100, Height = 100, Content = button };
        window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
        if (useHarness) UiHeadlessInputHarness.Click(window, button);
        else { window.MouseDown(new Point(50, 50), MouseButton.Left); window.MouseUp(new Point(50, 50), MouseButton.Left); }
        Dispatcher.UIThread.RunJobs(); window.Close(); return count;
    }
}

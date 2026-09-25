using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticTrackedHideTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticTrackedHideTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Theory]
    [InlineData("escape")]
    [InlineData("user close")]
    [InlineData("minimize")]
    [InlineData("owner close")]
    [InlineData("unload")]
    public void Explicit_lifecycle_exit_hides_tool_window(string reason)
    {
        _fixture.Run(() =>
        {
            var target = new Button { Name = "Target" };
            var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 480, Height = 300,
                Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            Assert.Equal(1, host.ActiveProbeCardCount);
            switch (reason)
            {
                case "escape": host.ExitProbe(); break;
                case "user close": Invoke(host, "CloseToolWindow"); break;
                case "minimize": window.WindowState = WindowState.Minimized; break;
                case "owner close": window.Close(); break;
                case "unload": ((Panel)window.Content!).Children.Remove(host); break;
            }
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(0, host.ActiveProbeCardCount);
            if (reason != "owner close") window.Close();
        });
    }

    [Fact]
    public void Detached_popup_uses_last_bounds_and_frozen_identity_after_activation()
    {
        _fixture.Run(() =>
        {
            var item = new MenuItem { Header = "点标记" };
            var popup = new Popup { Child = item };
            var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 480, Height = 300,
                Content = new Grid { Children = { host, popup } } };
            window.Show(); window.UpdateLayout(); popup.IsOpen = true;
            Dispatcher.UIThread.RunJobs(); host.TrackProbe(DiagnosticProbeResolver.Resolve(item));
            var last = host.LastKnownBounds;
            Assert.NotNull(last);
            popup.IsOpen = false;
            popup.Child = null;
            item.Header = "Changed after closing";
            Invoke(host, "OnWindowActivated", window, EventArgs.Empty);
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(last, host.LastKnownBounds);
            Assert.Equal("点标记", host.TrackedSnapshot?.TargetDisplayName);
            Assert.Equal(1, host.ActiveProbeCardCount);
            window.Close();
        });
    }

    static void Invoke(DiagnosticOverlayHost host, string name, params object?[] args) =>
        typeof(DiagnosticOverlayHost).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(host, args);
}

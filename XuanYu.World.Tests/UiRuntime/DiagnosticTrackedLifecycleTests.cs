using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticTrackedLifecycleTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticTrackedLifecycleTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Owner_deactivation_keeps_locked_card_visible()
    {
        _fixture.Run(() =>
        {
            var (window, host, target) = Open();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            Invoke(host, "OnWindowDeactivated", window, EventArgs.Empty);
            Assert.Equal(1, host.ActiveProbeCardCount);
            Assert.True(host.IsProbeLocked);
            Assert.Equal(0, host.ActiveProbeHighlightCount);
            window.Close();
        });
    }

    [Fact]
    public void Closing_diagnostic_mode_hides_card()
    {
        _fixture.Run(() =>
        {
            var (window, host, target) = Open();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(target));
            host.DataContext = new UiVm(null, seedInitialScene: false);
            Dispatcher.UIThread.RunJobs();
            Assert.Equal(0, host.ActiveProbeCardCount);
            window.Close();
        });
    }

    [Fact]
    public void Popup_removal_keeps_snapshot_until_another_click()
    {
        _fixture.Run(() =>
        {
            var first = new MenuItem { Header = "点标记" };
            var popup = new Popup { Child = first };
            var host = new DiagnosticOverlayHost();
            var second = new Button { Name = "Next" };
            var window = new Window { Width = 480, Height = 300,
                Content = new Grid { Children = { host, popup, second } } };
            window.Show(); window.UpdateLayout(); popup.IsOpen = true;
            Dispatcher.UIThread.RunJobs();
            host.TrackProbe(DiagnosticProbeResolver.Resolve(first));
            popup.IsOpen = false; popup.Child = null; Dispatcher.UIThread.RunJobs();
            Assert.Equal("点标记", DisplayName(Snapshot(host)));
            Assert.Equal(1, host.ActiveProbeCardCount);
            host.TrackProbe(DiagnosticProbeResolver.Resolve(second));
            Assert.Equal("Next", DisplayName(Snapshot(host)));
            window.Close();
        });
    }

    static (Window, DiagnosticOverlayHost, Button) Open()
    {
        var target = new Button { Name = "Target" };
        var host = new DiagnosticOverlayHost();
        var window = new Window { Width = 480, Height = 300,
            Content = new Grid { Children = { target, host } } };
        window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
        return (window, host, target);
    }

    static DiagnosticElementSnapshot? Snapshot(DiagnosticOverlayHost host) =>
        (DiagnosticElementSnapshot?)typeof(DiagnosticOverlayHost).GetProperty("TrackedSnapshot")?.GetValue(host);

    static string? DisplayName(DiagnosticElementSnapshot? snapshot) =>
        snapshot?.GetType().GetProperty("TargetDisplayName")?.GetValue(snapshot) as string;

    static void Invoke(DiagnosticOverlayHost host, string name, params object?[] args) =>
        typeof(DiagnosticOverlayHost).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(host, args);
}

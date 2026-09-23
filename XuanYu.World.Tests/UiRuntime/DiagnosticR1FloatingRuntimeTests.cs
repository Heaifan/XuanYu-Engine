using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticR1FloatingRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticR1FloatingRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Locked_probe_ignores_later_hover_until_escape()
    {
        var first = new Button { Name = "First" };
        var second = new Button { Name = "Second" };
        var host = new DiagnosticOverlayHost();
        host.SetProbeResult(DiagnosticProbeResolver.Resolve(first));
        host.LockProbe();
        host.SetProbeResult(DiagnosticProbeResolver.Resolve(second));
        Assert.Same(second, host.CurrentProbeResult?.DeepVisual);
        Assert.Same(first, host.LockedProbeResult?.DeepVisual);
        Assert.True(host.IsProbeLocked);
        host.UnlockProbe();
        Assert.False(host.IsProbeLocked);
    }

    [Fact]
    public void Small_window_contains_the_entire_probe_card()
    {
        _fixture.Run(() =>
        {
            var target = new Button { Width = 300, Height = 30 }; var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 180, Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            host.SetProbeResult(DiagnosticProbeResolver.Resolve(target)); Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
            var field = typeof(DiagnosticOverlayHost).GetField("_probeCard", BindingFlags.Instance | BindingFlags.NonPublic);
            var card = (Control)field!.GetValue(host)!; var left = Canvas.GetLeft(card); var top = Canvas.GetTop(card);
            Assert.True(left >= 0 && top >= 0 && left + card.Bounds.Width <= window.ClientSize.Width &&
                top + card.Bounds.Height <= window.ClientSize.Height); window.Close();
        });
    }

    [Fact]
    public void Open_popup_root_is_registered_for_probe_input()
    {
        _fixture.Run(() =>
        {
            var popupHost = new DiagnosticPopupHost(); var popup = new Popup { Child = popupHost };
            var host = new DiagnosticOverlayHost(); var window = new Window { Width = 320, Height = 180,
                Content = new Grid { Children = { host, popup } } };
            window.Show(); window.UpdateLayout(); popup.IsOpen = true; popupHost.SetPopupOpen(true);
            Dispatcher.UIThread.RunJobs();
            var root = TopLevel.GetTopLevel(popupHost);
            if (root is not null)
            {
                var field = typeof(DiagnosticOverlayHost).GetField("_probeRoots", BindingFlags.Instance | BindingFlags.NonPublic);
                var roots = (IEnumerable<TopLevel>)field!.GetValue(host)!; Assert.Contains(root, roots);
            }
            popup.IsOpen = false; window.Close();
        });
    }
}

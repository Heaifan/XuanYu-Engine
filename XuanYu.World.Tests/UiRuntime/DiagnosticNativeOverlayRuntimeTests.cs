using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Reflection;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticNativeOverlayRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticNativeOverlayRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Probe_visuals_are_hosted_by_popup_roots()
    {
        _fixture.Run(() =>
        {
            var target = new Button { Width = 120, Height = 30 };
            var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 200, Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            host.SetProbeResult(new DiagnosticProbeResult(target, target, "N/A", "N/A",
                "Button", "N/A", "N/A", "N/A", true, true, target.Bounds, DiagnosticProbeMode.Semantic));
            var cardField = typeof(DiagnosticOverlayHost).GetField("_nativeCardPopup", BindingFlags.Instance | BindingFlags.NonPublic);
            var highlightField = typeof(DiagnosticOverlayHost).GetField("_nativeHighlightPopup", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.True(((Avalonia.Controls.Primitives.Popup)cardField!.GetValue(host)!).IsOpen);
            Assert.True(((Avalonia.Controls.Primitives.Popup)highlightField!.GetValue(host)!).IsOpen);
            var popup = (Avalonia.Controls.Primitives.Popup)cardField.GetValue(host)!;
            var placement = popup.PlacementRect!.Value;
            Assert.Equal(Avalonia.Controls.PlacementMode.AnchorAndGravity, popup.Placement);
            Assert.True(placement.Left >= 12 && placement.Top >= 12);
            Assert.True(placement.Right <= window.ClientSize.Width - 12);
            Assert.True(placement.Bottom <= window.ClientSize.Height - 12);
            window.Close();
        });
    }

    [Fact]
    public void Popup_probe_records_native_window_policy_facts()
    {
        _fixture.Run(() =>
        {
            var target = new Button { Width = 120, Height = 30 };
            var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 200,
                Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            host.SetProbeResult(new DiagnosticProbeResult(target, target, "N/A", "N/A",
                "Button", "N/A", "N/A", "N/A", true, true, target.Bounds,
                DiagnosticProbeMode.Semantic));
            var field = typeof(DiagnosticOverlayHost).GetField("_lastNativeWindowProbe",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var snapshot = field!.GetValue(host)!;
            var text = snapshot.GetType().GetMethod("Format")!.Invoke(snapshot, null) as string;
            Assert.Contains("PopupHwnd=", text);
            Assert.Contains("OwnerHwnd=", text);
            Assert.Contains("TopMost=", text);
            Assert.Contains("ForegroundHwnd=", text);
            window.Close();
        });
    }

}

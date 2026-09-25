using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.Platform;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticCardPlacementTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticCardPlacementTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Manual_card_position_survives_probe_content_changes()
    {
        _fixture.Run(() =>
        {
            var first = new Button { Width = 80, Height = 30 };
            var second = new Button { Width = 120, Height = 30 };
            var host = new DiagnosticOverlayHost();
            var window = Show(host, first, second);
            host.SetProbeResult(DiagnosticProbeResolver.Resolve(first));
            Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
            var tool = Tool(host); tool.Position = new PixelPoint(90, 70);
            Invoke(host, "BeginToolDrag"); var position = tool.Position;
            host.SetProbeResult(DiagnosticProbeResolver.Resolve(second));
            Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
            Assert.Equal(position, tool.Position); window.Close();
        });
    }

    [Fact]
    public void Diagnostic_mode_off_restores_auto_card_placement()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var host = new DiagnosticOverlayHost { DataContext = vm };
            var target = new Button { Width = 80, Height = 30 };
            var window = new Window { Width = 320, Height = 180,
                DataContext = vm, Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); vm.RunCommand.Execute("诊断模式");
            host.SetProbeResult(DiagnosticProbeResolver.Resolve(target));
            Invoke(host, "BeginToolDrag");
            vm.RunCommand.Execute("诊断模式"); Dispatcher.UIThread.RunJobs();
            Assert.Equal("Auto", PlacementMode(host).ToString()); window.Close();
        });
    }

    static Window Show(DiagnosticOverlayHost host, params Control[] controls)
    {
        var content = new Grid(); foreach (var control in controls) content.Children.Add(control);
        content.Children.Add(host); var window = new Window { Width = 320, Height = 180, Content = content };
        window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs(); return window;
    }

    static DiagnosticFloatingToolWindow Tool(DiagnosticOverlayHost host) =>
        (DiagnosticFloatingToolWindow)Field(host, "_toolWindow")!.GetValue(host)!;
    static object PlacementMode(DiagnosticOverlayHost host) => Field(host, "_cardPlacementMode")!.GetValue(host)!;
    static void Invoke(object target, string name, object value) => target.GetType().GetMethod(name,
        BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(target, [value]);
    static void Invoke(object target, string name) => target.GetType().GetMethod(name,
        BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(target, null);
    static FieldInfo? Field(object target, string name) => target.GetType().GetField(name,
        BindingFlags.Instance | BindingFlags.NonPublic);
}

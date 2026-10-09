using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using System.Reflection;
using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Tests.UiRuntime;
using Xunit.Abstractions;

namespace XuanYu.World.Tests.Scene;

[Collection("UiRuntime")]
public sealed class CAuditR1FlowAInputProbeTests(
    UiHeadlessFixture fixture, ITestOutputHelper output)
{
    [Fact]
    public void Observe_headless_pointer_route_and_region_pick_boundary()
    {
        var vm = RegionDrawingTestVm.Create();
        var host = new UiRuntimeTestHost(fixture);
        var evidence = new List<string>();
        try
        {
            fixture.Run(() =>
            {
                var main = new Main { DataContext = vm };
                var window = host.Show(main, 800, 600);
                window.UpdateLayout();
                vm.UpdateViewportFrame(800, 600);
                vm.SelectToolCommand.Execute("区域绘制");
                var native = UiRuntimeTestHost.Descendants<VulkanNativeHost>(main).Single();
                var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
                var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
                var local = Enumerable.Range(1, 15).SelectMany(x => Enumerable.Range(1, 11)
                    .Select(y => new Point(x * 50, y * 50)))
                    .First(p => MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, p.X, p.Y, out _));
                var windowPoint = native.TranslatePoint(local, window)!.Value;
                var hwnd = typeof(VulkanNativeHost).GetField("_hwnd",
                    BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(native);
                evidence.Add($"host={native.Bounds}; visible={native.IsEffectivelyVisible}; hwnd={hwnd}");
                evidence.Add($"local={local}; window={windowPoint}; hit={window.InputHitTest(windowPoint)?.GetType().FullName ?? "null"}");
                var viewportView = UiRuntimeTestHost.Descendants<VulkanViewport>(main).Single();
                evidence.Add($"fallback-visible={viewportView.FindControl<Border>("FallbackLayer")!.IsVisible}");
                window.PointerMoved += (_, e) => evidence.Add($"window-move={e.GetCurrentPoint(window).Position}; source={e.Source?.GetType().Name}");
                window.PointerPressed += (_, e) => evidence.Add($"window-down={e.GetCurrentPoint(window).Position}; source={e.Source?.GetType().Name}; handled={e.Handled}");
                native.PointerMoved += (_, e) => evidence.Add($"host-move={e.GetCurrentPoint(native).Position}");
                native.PointerPressed += (_, e) => evidence.Add($"host-down={e.GetCurrentPoint(native).Position}; handled={e.Handled}");
                native.PointerReleased += (_, e) => evidence.Add($"host-up={e.GetCurrentPoint(native).Position}");
                window.Activate(); Dispatcher.UIThread.RunJobs();
                window.MouseMove(windowPoint); Dispatcher.UIThread.RunJobs();
                evidence.Add($"after-move:{State(vm)}");
                window.MouseDown(windowPoint, MouseButton.Left); Dispatcher.UIThread.RunJobs();
                evidence.Add($"after-down:{State(vm)}; footer={vm.FooterMessage}; layer={vm.MapSession.ActiveRegionLayerId}");
                window.MouseUp(windowPoint, MouseButton.Left); Dispatcher.UIThread.RunJobs();
                evidence.Add($"after-up:{State(vm)}");
                AvaloniaViewportInputForwarder.Forward(vm.ViewportInput.Sink,
                    new(EditorPointerEventKind.Pressed, new(local.X, local.Y), EditorPointerButtons.Left,
                        EditorPointerModifiers.None, 0, 7), new("forensic-avalonia-sample"), 1);
                evidence.Add($"forwarder-sample:{State(vm)}; footer={vm.FooterMessage}; layer={vm.MapSession.ActiveRegionLayerId}");
            });
        }
        finally { host.Dispose(); }
        var path = Path.Combine(Path.GetTempPath(), "XYE-C-AUDIT-R1", "A-POINTER-01.log");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllLines(path, evidence);
        foreach (var line in evidence) output.WriteLine(line);
        output.WriteLine($"evidence={path}");
        Assert.NotEmpty(evidence);
    }

    static string State(UiVm vm) =>
        $"owner={vm.ViewportInput.Router.State.Owner}; captured={vm.ViewportInput.Router.State.IsCaptured}; active={vm.IsRegionDrawingTool}; hits={vm.RegionDrawingHitCount}; draft={vm.RegionDrawingDraftVertexCount}";
}

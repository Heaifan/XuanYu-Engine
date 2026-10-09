using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Tests;
using XuanYu.World.Tests.UiRuntime;

namespace XuanYu.World.Tests.Scene;

[Collection("UiRuntime")]
public sealed class CAuditR1FlowAPointerTests(UiHeadlessFixture fixture)
{
    [Fact]
    public void Headless_synthetic_input_commits_region_and_map_undo_redo_restores_it()
    {
        var vm = RegionDrawingTestVm.Create();
        var traceDirectory = Path.Combine(Path.GetTempPath(), "XYE-C-AUDIT-R1");
        Directory.CreateDirectory(traceDirectory);
        var tracePath = Path.Combine(traceDirectory, $"A-POINTER-01-synthetic-{Guid.NewGuid():N}.log");
        void Trace(string stage) => File.AppendAllText(tracePath, $"{DateTimeOffset.UtcNow:O} {stage}{Environment.NewLine}");
        Trace("fixture-start");
        var host = new UiRuntimeTestHost(fixture);
        try
        {
        var result = fixture.Run(() =>
        {
            Trace("ui-enter");
            var main = new Main { DataContext = vm };
            var window = host.Show(main, 800, 600);
            Trace("window-shown");
            window.UpdateLayout();
            Trace("layout-updated");
            vm.UpdateViewportFrame(800, 600);
            vm.SelectToolCommand.Execute("区域绘制");
            Trace("tool-selected");
            var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
            var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
            var hits = Enumerable.Range(1, 15).SelectMany(x => Enumerable.Range(1, 11)
                .Select(y => new Point(x * 50, y * 50)))
                .Where(p => MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, p.X, p.Y, out _))
                .ToArray();
            Assert.NotEmpty(hits);
            Trace($"surface-hits:{hits.Length}");
            var first = hits[0];
            var second = hits.First(p => Math.Pow(p.X - first.X, 2) + Math.Pow(p.Y - first.Y, 2) > 25_000);
            var third = hits.First(p => Math.Abs((second.X - first.X) * (p.Y - first.Y) -
                (second.Y - first.Y) * (p.X - first.X)) > 5_000);
            var points = new[] { first, second, third };
            var native = UiRuntimeTestHost.Descendants<VulkanNativeHost>(main).Single();
            foreach (var point in points)
                SyntheticClickAt(vm, native, point, Trace);
            Trace("synthetic-three-points-dispatched");
            Assert.Equal(3, vm.RegionDrawingDraftVertexCount);
            Assert.Empty(vm.MapSession.CurrentMap.Regions);
            SyntheticClickAt(vm, native, first, Trace);
            Trace("synthetic-close-click-dispatched");
            Assert.False(vm.IsRegionDrawingDraftActive);
            var created = Assert.Single(vm.MapSession.CurrentMap.Regions).RegionId;
            vm.MapUndo();
            Assert.Empty(vm.MapSession.CurrentMap.Regions);
            vm.MapRedo();
            Trace("undo-redo-complete");
            Assert.Equal(created, Assert.Single(vm.MapSession.CurrentMap.Regions).RegionId);
            return vm.RegionDrawingHitCount;
        });
        Trace("fixture-returned");
        Assert.Equal(4, result);
        Trace("assertions-complete");
        }
        finally
        {
            Trace("host-dispose-start");
            host.Dispose();
            Trace("host-dispose-complete");
        }
    }

    static void SyntheticClickAt(UiVm vm, Control target, Point targetPoint, Action<string> trace)
    {
        Assert.True(target.IsEffectivelyVisible);
        Assert.True(new Rect(target.Bounds.Size).Contains(targetPoint));
        CAuditR1FlowAHeadlessSyntheticInput.Click(vm, targetPoint.X, targetPoint.Y);
        Dispatcher.UIThread.RunJobs();
        trace("synthetic-move-press-release-forwarded");
    }
}

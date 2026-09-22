using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticFix2Tests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticFix2Tests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Diagnostic_mode_does_not_show_region_bounds_by_default()
    {
        var count = _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var target = new Border { Width = 100, Height = 30 };
            XYDiagnostic.SetDebugId(target, "XYE.TEST.REGION"); DiagnosticRegistry.Register(target);
            var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 200, DataContext = vm,
                Content = new Grid { Children = { target, host } } };
            host.DataContext = vm; window.Show(); window.UpdateLayout(); vm.RunCommand.Execute("诊断模式");
            Dispatcher.UIThread.RunJobs(); var result = host.ActiveBadgeCount + host.ActiveRectangleCount;
            window.Close(); return result;
        });
        Assert.Equal(0, count);
    }

    [Fact]
    public void Region_bounds_require_an_explicit_diagnostic_command()
    {
        var count = _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); vm.RunCommand.Execute("诊断模式");
            vm.RunCommand.Execute("区域边界");
            return vm.IsDiagnosticRegionBoundsMode;
        });
        Assert.True(count);
    }

    [Fact]
    public void Template_internal_hit_resolves_to_named_semantic_button()
    {
        var result = _fixture.Run(() =>
        {
            var button = new Button { Name = "FileButton", Content = new Border { Name = "PART_MenuZone",
                Child = new TextBlock { Text = "文件" } } };
            var internalNode = (Visual)button.Content!; XYDiagnostic.SetDebugId((Control)internalNode, "XYE.MENU");
            var window = new Window { Width = 320, Height = 200, Content = button };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            var resolved = DiagnosticProbeResolver.Resolve(internalNode); window.Close(); return resolved;
        });
        Assert.Equal("FileButton", result.Name); Assert.Equal("Button", result.SemanticTarget?.GetType().Name);
    }

    [Fact]
    public void Locked_card_overlay_covers_the_window_root()
    {
        _fixture.Run(() =>
        {
            var target = new Border { Width = 120, Height = 30 }; var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 200,
                Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); Dispatcher.UIThread.RunJobs();
            host.SetProbeResult(DiagnosticProbeResolver.Resolve(target)); host.LockProbe();
            var field = typeof(DiagnosticOverlayHost).GetField("_floatingLayer", BindingFlags.Instance | BindingFlags.NonPublic);
            var layer = field?.GetValue(host) as Control;
            Assert.NotNull(layer); Assert.Equal(window.ClientSize, layer!.Bounds.Size); window.Close();
        });
    }
}

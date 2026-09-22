using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticBoundsRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticBoundsRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Diagnostic_mode_renders_and_clears_registered_target_bounds()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); var target = Target();
            var host = new DiagnosticOverlayHost();
            var window = new Window { Width = 320, Height = 160, DataContext = vm,
                Content = new Grid { Children = { target, host } } };
            window.Show(); window.UpdateLayout(); vm.RunCommand.Execute("诊断模式");
            vm.RunCommand.Execute("区域边界");
            Dispatcher.UIThread.RunJobs(); window.UpdateLayout();
            Assert.Contains(host.GetVisualDescendants().OfType<Border>(), x => x.BorderThickness == new Thickness(2));
            Assert.Equal(1, host.ActiveRectangleCount);
            vm.RunCommand.Execute("诊断模式"); Dispatcher.UIThread.RunJobs();
            Assert.Equal(0, host.ActiveRectangleCount); window.Close();
        });
    }

    static Border Target()
    {
        var target = new Border { Width = 120, Height = 30 };
        XYDiagnostic.SetDebugId(target, "XYE.TEST.BOUNDS"); DiagnosticRegistry.Register(target); return target;
    }
}

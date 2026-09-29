using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class RegionDrawContextToolbarRuntimeFix1Tests
{
    readonly UiHeadlessFixture _fixture;

    public RegionDrawContextToolbarRuntimeFix1Tests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Active_region_draw_disables_the_real_context_split_button()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        Top top = null!;
        var started = host.Run(() =>
        {
            var vm = new UiVm(null, () => true, seedInitialScene: false);
            top = new Top { DataContext = vm };
            host.Show(top, 900, 180);
            top.UpdateLayout();
            return vm.BeginContextDrawingAsync("区域面").GetAwaiter().GetResult();
        });

        Assert.True(started);
        var enabled = host.Run(() =>
        {
            Dispatcher.UIThread.RunJobs(); top.UpdateLayout();
            var toolbar = UiRuntimeTestHost.Descendants<ContextToolBar>(top).Single();
            return toolbar.FindControl<XYUI.Avalonia.Controls.XYSplitButton>("DrawSplitButton")!.IsEnabled;
        });
        Assert.False(enabled);
    }
}

using Avalonia.Controls;
using Avalonia.Threading;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class RegionDrawContextToolbarCompositionFix1Tests
{
    readonly UiHeadlessFixture _fixture;

    public RegionDrawContextToolbarCompositionFix1Tests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Active_region_draw_disables_the_real_context_split_button()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        Top top = null!;
        UiVm vm = null!;
        host.Run(() =>
        {
            vm = new UiVm(null, () => true, seedInitialScene: false);
            top = new Top { DataContext = vm };
            host.Show(top, 900, 180);
            top.UpdateLayout();
        });
        var started = await host.RunAsync(() => vm.BeginContextDrawingAsync("区域面"));

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

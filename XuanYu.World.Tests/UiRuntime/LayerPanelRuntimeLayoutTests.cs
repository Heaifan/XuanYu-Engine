using Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class LayerPanelRuntimeLayoutTests
{
    readonly UiHeadlessFixture _fixture;

    public LayerPanelRuntimeLayoutTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void ColdStartHintDoesNotCollapseLayerList()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var widths = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var panel = new LayerPanel { DataContext = vm };
            var window = host.Show(panel);
            var list = panel.FindControl<ListBox>("LayerList")!;
            return (Panel: panel.Bounds.Width, List: list.Bounds.Width, Window: window);
        });

        Assert.True(widths.Panel > 0);
        Assert.True(widths.List >= widths.Panel * 0.80,
            $"冷启动列表宽度异常：Panel={widths.Panel}, List={widths.List}");
    }

    [Fact]
    public void AddingSecondLayerDoesNotCauseStructuralWidthJump()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var widths = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var panel = new LayerPanel { DataContext = vm };
            var window = host.Show(panel);
            var list = panel.FindControl<ListBox>("LayerList")!;
            var before = list.Bounds.Width;
            vm.AddLayer();
            panel.UpdateLayout();
            var after = list.Bounds.Width;
            return (Before: before, After: after, Window: window);
        });

        Assert.True(Math.Abs(widths.After - widths.Before) < 20,
            $"添加图层导致列表宽度跳变：Before={widths.Before}, After={widths.After}");
    }
}

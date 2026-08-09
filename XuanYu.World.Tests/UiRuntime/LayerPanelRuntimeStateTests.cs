using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class LayerPanelRuntimeStateTests
{
    readonly UiHeadlessFixture _fixture;

    public LayerPanelRuntimeStateTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void SelectedRowUsesProjectSelectionBrushInTemplate()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var color = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.AddLayer();
            var panel = new LayerPanel { DataContext = vm };
            var window = host.Show(panel);
            var list = panel.FindControl<ListBox>("LayerList")!;
            list.SelectedIndex = 0;
            panel.UpdateLayout();
            var item = list.ContainerFromIndex(0) as ListBoxItem;
            var presenter = UiRuntimeTestHost.Descendants<ContentPresenter>(item!).First();
            return (presenter.Background as SolidColorBrush)?.Color;
        });

        Assert.Equal(Color.Parse("#E5F0F4"), color);
    }

    [Theory]
    [InlineData("layerSwitch", "#EAF3F7")]
    [InlineData("layerLockSwitch", "#F4EFE5")]
    public void CheckedStateUsesProjectBrushInTemplate(string className, string expected)
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var color = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.AddLayer();
            var panel = new LayerPanel { DataContext = vm };
            host.Show(panel);
            var list = panel.FindControl<ListBox>("LayerList")!;
            var item = Enumerable.Range(0, list.ItemCount)
                .Select(list.ContainerFromIndex)
                .OfType<ListBoxItem>()
                .First(x => UiRuntimeTestHost.Descendants<ToggleButton>(x)
                    .Any(t => t.Classes.Contains(className) && t.IsEnabled) &&
                    x.DataContext is MapLayerRowViewModel { IsRegion: true });
            var toggle = UiRuntimeTestHost.Descendants<ToggleButton>(item!)
                .Single(x => x.Classes.Contains(className));
            if (item.DataContext is MapLayerRowViewModel row)
            {
                if (className == "layerLockSwitch") row.IsLocked = true;
                else row.IsVisible = true;
            }
            panel.UpdateLayout();
            return (toggle.Background as SolidColorBrush)?.Color;
        });

        if (className == "layerLockSwitch")
            Assert.NotEqual(Color.Parse("#0078D7"), color);
        else
            Assert.Equal(Color.Parse(expected), color);
    }
}

using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class UiRuntimeRiskTests
{
    readonly UiHeadlessFixture _fixture;

    public UiRuntimeRiskTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void TopCheckedToolKeepsProjectSelectionBrush()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var color = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var top = new Top { DataContext = vm };
            host.Show(top, 1200, 180);
            var toggle = UiRuntimeTestHost.Descendants<ToggleButton>(top)
                .Single(x => x.Classes.Contains("toolBtn") && x.IsChecked == true);
            toggle.IsChecked = true;
            top.UpdateLayout();
            return (toggle.Background as SolidColorBrush)?.Color;
        });

        Assert.NotEqual(Color.Parse("#0078D7"), color);
    }

    [Fact]
    public void FootSelectedLogKeepsProjectSelectionBrush()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var color = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.SelectToolCommand.Execute("移动");
            vm.IsLogOpen = true;
            var foot = new Foot { DataContext = vm };
            host.Show(foot, 1200, 420);
            var list = foot.FindControl<ListBox>("LogList")!;
            list.SelectedIndex = 0;
            foot.UpdateLayout();
            var item = list.ContainerFromIndex(0) as ListBoxItem;
            var presenter = UiRuntimeTestHost.Descendants<ContentPresenter>(item!).First();
            return (presenter.Background as SolidColorBrush)?.Color;
        });

        Assert.Equal(Color.Parse("#E5F0F4"), color);
    }
}

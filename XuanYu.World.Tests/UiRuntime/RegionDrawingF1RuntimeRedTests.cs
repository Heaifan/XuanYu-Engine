using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class RegionDrawingF1RuntimeRedTests
{
    readonly UiHeadlessFixture _fixture;
    public RegionDrawingF1RuntimeRedTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Region_drawing_belongs_to_map_editor_workspace()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var found = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var panel = new MapEditorPanel { DataContext = vm };
            host.Show(panel, 420, 520);
            return UiRuntimeTestHost.Descendants<Button>(panel)
                .Any(button => button.Content?.ToString() == "区域绘制");
        });

        Assert.True(found);
    }

    [Fact]
    public void Region_drawing_selected_text_uses_dark_foreground()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var color = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var panel = new MapEditorPanel { DataContext = vm };
            host.Show(panel, 420, 520);
            vm.SelectToolCommand.Execute("区域绘制");
            panel.UpdateLayout();
            var toggle = UiRuntimeTestHost.Descendants<ToggleButton>(panel)
                .Single(x => x.IsChecked == true && x.Classes.Contains("mapTool"));
            return (toggle.Foreground as SolidColorBrush)?.Color;
        });

        Assert.Equal(Color.Parse("#243744"), color);
    }
}

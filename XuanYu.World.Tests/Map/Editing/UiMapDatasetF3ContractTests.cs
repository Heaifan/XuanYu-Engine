using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapDatasetF3ContractTests
{
    static readonly string Root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
    static string Read(params string[] path) => File.ReadAllText(Path.Combine([Root, .. path]));

    [Fact]
    public void Dataset_list_is_a_single_28_dip_name_and_status_row()
    {
        var panel = Read("XuanYu.Editor.UI", "Right", "DatasetPanel.axaml");
        Assert.Contains("Control.Height.Standard", panel);
        Assert.Contains("Text=\"{Binding Name}\"", panel);
        Assert.Contains("Text=\"{Binding Status}\"", panel);
        Assert.Contains("ToolTip.Tip=\"{Binding TypeIdText}\"", panel);
        Assert.DoesNotContain("Text=\"{Binding TypeIdText}\"", panel);
        Assert.DoesNotContain("＋", panel);
    }

    [Fact]
    public void Dataset_layer_is_a_32_dip_single_line_formal_layer_switch_row()
    {
        var panel = Read("XuanYu.Editor.UI", "Right", "DatasetLayerPanel.axaml");
        Assert.Contains("LayerPanel.States.axaml", panel);
        Assert.Contains("Control.Height.Emphasized", panel);
        Assert.Contains("ColumnDefinitions=\"24,*,Auto,26,26\"", panel);
        Assert.Contains("Classes=\"layerSwitch\"", panel);
        Assert.Contains("Classes=\"layerLockSwitch\"", panel);
        Assert.DoesNotContain("datasetLayerAction", panel);
        Assert.DoesNotContain("Text=\"{Binding TypeIdText}\"", panel);
    }

    [Fact]
    public void Dataset_rows_center_text_and_reserve_a_64_dip_status_area()
    {
        var dataset = Read("XuanYu.Editor.UI", "Right", "DatasetPanel.axaml");
        var layer = Read("XuanYu.Editor.UI", "Right", "DatasetLayerPanel.axaml");
        Assert.All([dataset, layer], panel =>
        {
            Assert.Contains("HorizontalAlignment\" Value=\"Center", panel);
            Assert.Contains("VerticalAlignment\" Value=\"Center", panel);
            Assert.Contains("TextAlignment\" Value=\"Center", panel);
            Assert.Contains("MinWidth\" Value=\"{StaticResource Size.Width.64}", panel);
        });
    }

    [Fact]
    public void Dataset_selection_hides_map_form_and_names_dataset_inspector()
    {
        var panel = Read("XuanYu.Editor.UI", "Right", "InspectorPanel.axaml");
        Assert.Contains("IsVisible=\"{Binding !HasSelectedDataset}\"", panel);
        Assert.Contains("Text=\"{Binding InspectorSectionTitle}\"", panel);
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.Equal("基础信息", vm.InspectorSectionTitle);
    }
}

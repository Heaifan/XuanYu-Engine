using System.IO;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4-F3：图层视觉合同（V01～V06，源码合同模式）——状态样式/类型标签/热区/字号层级。
public sealed class UiLayerVisualContractTests
{
    static readonly string LayerPanel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "LayerPanel.axaml"));

    static readonly string Ui = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Ui.axaml"));

    static readonly string MapEditor = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapEditorPanel.axaml"));

    static readonly string Inspector = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "LayerInspectorPanel.axaml"));

    [Fact]
    public void V01_region_and_system_use_distinct_style_classes()
    {
        Assert.Contains("kindTagRegion", LayerPanel);
        Assert.Contains("kindTagSystem", LayerPanel);
        Assert.Contains("#E8F3F6", LayerPanel);
        Assert.Contains("#F0F2F4", LayerPanel);
    }

    [Fact]
    public void V02_visible_and_hidden_have_distinct_state_styles()
    {
        Assert.Contains("layerSwitch:checked", LayerPanel);
        Assert.Contains("#326F8A", LayerPanel);
        Assert.Contains("#8995A2", LayerPanel);
    }

    [Fact]
    public void V03_locked_and_unlocked_have_distinct_state_styles()
    {
        Assert.Contains("layerLockSwitch:checked", LayerPanel);
        Assert.Contains("#7A6238", LayerPanel);
        Assert.Contains("#7B8794", LayerPanel);
    }

    [Fact]
    public void V04_icon_buttons_keep_clickable_hotzone()
    {
        Assert.Contains("Width\" Value=\"26\"", LayerPanel);
        Assert.Contains("Height\" Value=\"24\"", LayerPanel);
    }

    [Fact]
    public void V05_right_tabs_use_converged_font_sizes()
    {
        Assert.Contains("FontSize\" Value=\"13\"", Ui);
        Assert.Contains("layerSubTab", MapEditor);
        Assert.Contains("FontSize\" Value=\"14\"", MapEditor);
    }

    [Fact]
    public void V06_inspector_field_font_below_tab_font()
    {
        Assert.Contains("TextBlock.key", Inspector);
        Assert.Contains("FontSize\" Value=\"12\"", Inspector);
    }
}

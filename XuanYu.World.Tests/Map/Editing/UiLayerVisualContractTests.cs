using System.IO;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4-F3：图层视觉合同（V01～V06，源码合同模式）——状态样式/类型标签/热区/字号层级。
// ARCH-UI-SPEC-R1-D4：色值断言升级为 Token 引用（Layer.*/Color.* 正式 Token，不依赖具体 hex）。
public sealed class UiLayerVisualContractTests
{
    static readonly string LayerPanel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "LayerPanel.axaml")) +
        File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Right", "LayerPanel.States.axaml"));

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
        Assert.Contains("Layer.Kind.Region.Bg", LayerPanel);   // D4：正式 Token 引用
        Assert.Contains("Layer.Kind.System.Bg", LayerPanel);
    }

    [Fact]
    public void V02_visible_and_hidden_have_distinct_state_styles()
    {
        Assert.Contains("layerSwitch:checked", LayerPanel);
        Assert.Contains("Layer.State.Visible", LayerPanel);
        Assert.Contains("Layer.State.Hidden", LayerPanel);
    }

    [Fact]
    public void V03_locked_and_unlocked_have_distinct_state_styles()
    {
        Assert.Contains("layerLockSwitch:checked", LayerPanel);
        Assert.Contains("Layer.State.Locked", LayerPanel);
        Assert.Contains("Layer.State.Unlocked", LayerPanel);
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
        Assert.Contains("Font.Body.Size", Ui);            // D4-F1：顶层页签 13 走正式 Token
        Assert.Contains("layerSubTab", MapEditor);
        Assert.Contains("Font.Section.Size", MapEditor);  // D4：二级页签 14 走正式 Token
    }

    [Fact]
    public void V06_inspector_field_font_below_tab_font()
    {
        Assert.Contains("uiLabel", Inspector);            // D4-F1：字段标签走公共 Label 12
        Assert.Contains("uiValue", Inspector);            // D4-F1：字段值走公共 Body 13
    }
}

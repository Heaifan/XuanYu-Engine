using System.IO;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4：图层面板结构合同（K03/K04/W50~W52）——图标 16/热区/笔画、Layer.* Token、三重区分。
public sealed class UiD4LayerContractTests
{
    static readonly string Panel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "LayerPanel.axaml"));

    static readonly string Inspector = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "LayerInspectorPanel.axaml"));

    [Fact]
    public void State_icons_use_16_viewport_and_15_stroke()
    {
        Assert.Contains("<Setter Property=\"Width\" Value=\"16\"/>", Panel);
        Assert.Contains("<Setter Property=\"Height\" Value=\"16\"/>", Panel);
        Assert.Contains("StrokeThickness\" Value=\"1.5\"", Panel);
    }

    [Fact]
    public void Hotzone_keeps_registered_component_exception()
    {
        Assert.Contains("<Setter Property=\"Width\" Value=\"26\"/>", Panel);
        Assert.Contains("<Setter Property=\"Height\" Value=\"24\"/>", Panel);
    }

    [Fact]
    public void State_colors_reference_frozen_layer_tokens()
    {
        Assert.Contains("Layer.State.VisibleBg", Panel);
        Assert.Contains("Layer.State.Visible", Panel);
        Assert.Contains("Layer.State.Hidden", Panel);
        Assert.Contains("Layer.State.LockedBg", Panel);
        Assert.Contains("Layer.State.Locked", Panel);
        Assert.Contains("Layer.State.Unlocked", Panel);
    }

    [Fact]
    public void Kind_tags_reference_layer_tokens_and_keep_text_labels()
    {
        Assert.Contains("Layer.Kind.Region.Bg", Panel);
        Assert.Contains("Layer.Kind.Region.Text", Panel);
        Assert.Contains("Layer.Kind.System.Bg", Panel);
        Assert.Contains("Layer.Kind.System.Text", Panel);
        Assert.Contains("Text=\"区域\"", Panel);   // 文字区分（三重之一）
        Assert.Contains("Text=\"系统\"", Panel);
    }

    [Fact]
    public void Drop_line_uses_frozen_token_at_2_dip()
    {
        Assert.Contains("Layer.DropLine", Panel);
        Assert.Contains("<Setter Property=\"Height\" Value=\"2\"/>", Panel);
        Assert.DoesNotContain("#7FA8C6", Panel);
    }

    [Fact]
    public void Active_mark_uses_accent()
    {
        Assert.Contains("Color.Accent", Panel);
        Assert.DoesNotContain("#5b8db8", Panel);
    }

    [Fact]
    public void Selected_row_style_is_explicit()
    {
        Assert.Contains("ListBoxItem:selected", Panel);
        Assert.Contains("Color.Selection.Bg", Panel);
    }

    [Fact]
    public void Toolbar_button_migrated_to_compact_24()
    {
        Assert.Contains("Control.Height.Compact", Panel);
        Assert.DoesNotContain("MinHeight\" Value=\"25\"", Panel);
    }

    [Fact]
    public void Layer_inspector_migrated_to_96_column_and_body_13()
    {
        Assert.Contains("ColumnDefinitions=\"96,*\"", Inspector);  // W53
        Assert.Contains("uiValue", Inspector);                     // W54：字段值走公共 Body 13
        Assert.DoesNotContain("ColumnDefinitions=\"70,*\"", Inspector);
        Assert.DoesNotContain("Classes=\"key\"", Inspector);       // D4-F1：局部 key/value 样式已统一为公共样式
    }
}

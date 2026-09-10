using System.IO;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4-F1：展示型动态文本默认（NoWrap + CharacterEllipsis + MaxLines=1）+ Tooltip 完整值。
public sealed class UiD4F1TextOverflowContractTests
{
    static readonly string Ui = Read("Design/UiStyles.D4F1.axaml"); // D4-F1 纠偏：公共样式独立文件
    static readonly string Inspector = Read("Right/InspectorPanel.axaml");
    static readonly string Right = Read("Right/EditorRightTabs.axaml");
    static readonly string MapPage = Read("Right/MapPagePanel.axaml");
    static readonly string MapForm = Read("Right/MapFormPanel.axaml");
    static readonly string LayerPanel = Read("Right/LayerPanel.axaml");
    static readonly string LayerInspector = Read("Right/LayerInspectorPanel.axaml");

    static string Read(string rel) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

    [Fact]
    public void Ui_value_style_implements_display_default()
    {
        Assert.Contains("<Style Selector=\"TextBlock.uiValue\">", Ui);
        Assert.Contains("TextWrapping\" Value=\"NoWrap\"", Ui);
        Assert.Contains("TextTrimming\" Value=\"CharacterEllipsis\"", Ui);
        Assert.Contains("MaxLines\" Value=\"1\"", Ui);
    }

    [Fact]
    public void Inspector_dynamic_values_single_line_with_full_tooltip()
    {
        Assert.Contains("<xy:XYText", Inspector);
        Assert.Contains("<xy:XYSelectableText", Inspector);
        Assert.Contains("Variant=\"Technical\"", Inspector);
        Assert.DoesNotContain("Classes=\"uiValue\"", Inspector);
        Assert.Contains("ToolTip.Tip=\"{Binding Value}\"", Inspector);
        Assert.DoesNotContain("NarrowFields", Inspector);
    }

    [Fact]
    public void Debug_rows_share_one_grid_row_and_never_wrap()
    {
        // 当前上下文/当前对象/输入状态：标签与值在同一 Grid 行（96 列）
        Assert.Contains("ColumnDefinitions=\"96,*\"", Right);
        Assert.Contains("DebugContextItems", Right);
        Assert.Contains("DebugObjectItems", Right);
        Assert.Contains("DebugInputItems", Right);
        Assert.Contains("ToolTip.Tip=\"{Binding Value}\"", Right); // 动态值完整 Tooltip
        Assert.DoesNotContain("TextWrapping=\"Wrap\"", Right);    // 调试页值不换行（走 uiValue）
    }

    [Fact]
    public void Map_summary_values_single_line_with_tooltips()
    {
        Assert.Contains("<xy:XYSectionTitle Text=\"地图资产\"", MapPage);
        Assert.Contains("<xy:XYSeparator Variant=\"Section\"", MapPage);
        Assert.Contains("<xy:XYLabel Text=\"名称\"", MapPage);
        Assert.Contains("<xy:XYText Grid.Column=\"1\"", MapPage);
        Assert.Contains("<xy:XYSelectableText Grid.Column=\"1\"", MapPage);
        Assert.Contains("ToolTip.Tip=\"{Binding MapName}\"", MapPage);
        Assert.Contains("ToolTip.Tip=\"{Binding MapPath}\"", MapPage);
        Assert.Contains("ToolTip.Tip=\"{Binding MapSizeText}\"", MapPage);
        Assert.Contains("ToolTip.Tip=\"{Binding MapStatusText}\"", MapPage);
        Assert.DoesNotContain("Classes=\"key\"", MapPage);        // 局部 key/value 已统一
        Assert.DoesNotContain("Classes=\"value\"", MapPage);
    }

    [Fact]
    public void Map_id_compression_copy_and_tooltip_kept()
    {
        Assert.Contains("MapIdDisplay", MapPage);
        Assert.Contains("ToolTip.Tip=\"{Binding MapIdText}\"", MapPage);
        Assert.Contains("<xy:XYSelectableText", MapPage);
        Assert.DoesNotContain("<xy:XYIconButton", MapPage);
    }

    [Fact]
    public void Layer_name_single_line_with_full_tooltip()
    {
        Assert.Contains("<xy:XYTruncatedText", LayerPanel);
        Assert.Contains("ToolTip.Tip=\"{Binding Name}\"", LayerPanel);
        Assert.Contains("ToolTip.Tip=\"{Binding LayerInspectorKindText}\"", LayerInspector);
    }

    [Fact]
    public void Explicit_multiline_content_keeps_dedicated_style()
    {
        Assert.Contains("<Style Selector=\"TextBlock.uiMultiline\">", Ui);
        Assert.DoesNotContain("uiMultiline", Inspector);  // R1 空状态改为轻量单行提示
        Assert.Contains("<xy:XYErrorText", MapForm); // 地图错误详情（XYUI-1-16）
    }
}


using System.IO;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4/D4-F1：地图编辑器结构合同（W48/补充裁决/纠偏 v2）——
// 只读摘要 72 列单行、MapId 压缩/复制、表单 96 列双模式、按钮网格布局。
public sealed class UiD4MapEditorContractTests
{
    static readonly string Page = Read("Right/MapPagePanel.axaml");
    static readonly string Form = Read("Right/MapFormPanel.axaml");
    static readonly string Editor = Read("Right/MapEditorPanel.axaml");

    static string Read(string rel) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

    [Fact]
    public void Readonly_summary_uses_compact_72_column()
    {
        Assert.Contains("ColumnDefinitions=\"72,*\"", Page);   // 只读摘要标签列 72（组件级例外 72~80）
        Assert.Contains("summaryRow", Page);
        Assert.Contains("MinHeight\" Value=\"24\"", Page);     // 单行高 24（24~28 合同）
    }

    [Fact]
    public void Map_id_shows_compressed_display_with_full_tooltip_and_copy()
    {
        Assert.Contains("MapIdDisplay", Page);                 // 前 8…后 6 显示
        Assert.Contains("ToolTip.Tip=\"{Binding MapIdText}\"", Page); // 完整 ID Tooltip
        Assert.Contains("CopyMapId_Click", Page);              // 复制按钮
        Assert.Contains("复制完整 MapId", Page);               // Tooltip 说明
    }

    [Fact]
    public void Map_id_never_wraps()
    {
        // MapId 行显式 NoWrap + Ellipsis + MaxLines=1（D4-F1 展示型动态文本默认）
        Assert.Contains("Text=\"{Binding MapIdDisplay}\"", Page);
        Assert.Contains("TextWrapping=\"NoWrap\"", Page);
        Assert.Contains("TextTrimming=\"CharacterEllipsis\"", Page);
        Assert.Contains("MaxLines=\"1\"", Page);
    }

    [Fact]
    public void Empty_path_shows_dash_placeholder()
    {
        Assert.Contains("MapPathDisplay", Page);               // VM 提供 — 占位
    }

    [Fact]
    public void Property_form_uses_96_column_and_narrow_mode()
    {
        Assert.Contains("PropsWide", Form);
        Assert.Contains("PropsNarrow", Form);                  // 可编辑表单窄模式（<360 整组上下）
        Assert.Contains("ColumnDefinitions=\"96,*\"", Form);   // 编辑表单标签列 96
        Assert.Contains("Spacing=\"2\"", Form);                // 窄模式标签→字段 2~4
        Assert.Contains("Spacing=\"6\"", Form);                // 窄模式字段组 6~8
    }

    [Fact]
    public void Button_group_keeps_spacing_6_and_critical_actions()
    {
        Assert.Contains("ColumnSpacing=\"6\"", Form);          // 属性按钮 Grid 间距 6
        Assert.Contains("RowSpacing=\"6\"", Form);
        Assert.Contains("应用地图属性", Form);
        Assert.Contains("撤销地图修改", Form);
        Assert.Contains("重做地图修改", Form);
    }

    [Fact]
    public void Each_page_has_single_vertical_scroll_container()
    {
        Assert.Contains("<ScrollViewer VerticalScrollBarVisibility=\"Auto\">", Editor);
        Assert.DoesNotContain("<ScrollViewer", Page);          // 页面内部不嵌套纵向滚动
    }

    [Fact]
    public void Map_editor_errors_use_error_token()
    {
        Assert.Contains("Color.Error", Form);                  // W47：错误色 Token
        Assert.DoesNotContain("#C0392B", Form);
    }

    [Fact]
    public void Map_id_copy_writes_full_untruncated_id()
    {
        var cs = Read("Right/MapPagePanel.axaml.cs");
        Assert.Contains("SetTextAsync(vm.MapIdText)", cs); // 复制完整 MapId（非显示压缩值）
        Assert.Contains("复制完整 MapId", Page);
    }

    [Fact]
    public void No_forbidden_legacy_values_in_map_pages()
    {
        foreach (var forbidden in new[] { "infoPanel", "#f7faff", "#185aa6", "#edf4ff", "#8cb2e2", "CornerRadius\" Value=\"5\"" })
            foreach (var text in new[] { Page, Form, Editor })
                Assert.DoesNotContain(forbidden, text);
    }
}

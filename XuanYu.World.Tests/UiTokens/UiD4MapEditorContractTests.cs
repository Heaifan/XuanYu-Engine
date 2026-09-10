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
        Assert.Contains("<xy:XYSelectableText", Page);         // XYUI 内建复制能力
        Assert.DoesNotContain("<xy:XYIconButton", Page);       // 不再存在外层重复入口
    }

    [Fact]
    public void Map_id_never_wraps()
    {
        // MapId 使用 XYUI-1-21 Technical，保留技术文本语义并由组件负责展示策略。
        Assert.Contains("Text=\"{Binding MapIdDisplay}\"", Page);
        Assert.Contains("Variant=\"Technical\"", Page);
        Assert.DoesNotContain("TextWrapping=\"NoWrap\"", Page);
        Assert.DoesNotContain("TextTrimming=\"CharacterEllipsis\"", Page);
    }

    [Fact]
    public void Empty_path_shows_dash_placeholder()
    {
        Assert.Contains("MapPathDisplay", Page);               // VM 提供 — 占位
    }
    [Fact]
    public void Property_form_uses_fixed_96_column_single_line_rows()
    {
        Assert.Contains("PropsWide", Form);
        Assert.DoesNotContain("PropsNarrow", Form);             // Engine 属性区不按侧栏宽度切换上下表单
        Assert.Contains("ColumnDefinitions=\"96,*\"", Form);   // 编辑表单标签列 96
        Assert.Contains("Spacing=\"{StaticResource Space.4}\"", Form); // 紧凑字段组间距
        Assert.Equal(5, Count(Form, "Grid.Column=\"1\""));
        Assert.Contains("VerticalAlignment=\"Center\"", Form);
    }
    [Fact]
    public void Button_group_uses_compact_spacing_and_critical_actions()
    {
        Assert.Contains("ColumnSpacing=\"{StaticResource Space.4}\"", Form);
        Assert.Contains("RowSpacing=\"{StaticResource Space.4}\"", Form);
        Assert.Equal(3, Count(Form, "Width=\"{StaticResource Size.Width.96}\""));
        Assert.Contains("应用地图属性", Form);
        Assert.Contains("撤销地图修改", Form);
        Assert.Contains("重做地图修改", Form);
    }

    static int Count(string text, string value) => text.Split(value).Length - 1;
    [Fact]
    public void Each_page_has_single_vertical_scroll_container()
    {
        Assert.Contains("VerticalScrollBarVisibility=\"Auto\"", Editor);
        Assert.DoesNotContain("<ScrollViewer", Page);          // 页面内部不嵌套纵向滚动
    }

    [Fact]
    public void Map_editor_errors_use_error_token()
    {
        Assert.Contains("<xy:XYErrorText", Form);              // W47：错误提示组件化（XYUI-1-16）
        Assert.DoesNotContain("#C0392B", Form);
    }

    [Fact]
    public void Map_id_copy_is_owned_by_xyui_selectable_text()
    {
        var cs = Read("Right/MapPagePanel.axaml.cs");
        Assert.DoesNotContain("SetTextAsync", cs);
        Assert.Contains("<xy:XYSelectableText", Page);
        Assert.DoesNotContain("<xy:XYIconButton", Page);
    }

    [Fact]
    public void No_forbidden_legacy_values_in_map_pages()
    {
        foreach (var forbidden in new[] { "infoPanel", "#f7faff", "#185aa6", "#edf4ff", "#8cb2e2", "CornerRadius\" Value=\"5\"" })
            foreach (var text in new[] { Page, Form, Editor })
                Assert.DoesNotContain(forbidden, text);
    }
}

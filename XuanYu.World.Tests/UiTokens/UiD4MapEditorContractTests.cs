using System.IO;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4：地图编辑器结构合同（W48/补充裁决）——紧凑摘要 72 列、MapId 显示/复制、单滚动、紧凑模式。
public sealed class UiD4MapEditorContractTests
{
    static readonly string Page = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapPagePanel.axaml"));

    static readonly string Editor = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapEditorPanel.axaml"));

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
        // MapId 行显式 NoWrap（覆盖 value 样式的 Wrap）；其他可换行字段不受影响
        Assert.Contains("<TextBlock Text=\"{Binding MapIdDisplay}\" Classes=\"value\" TextTrimming=\"CharacterEllipsis\" TextWrapping=\"NoWrap\"", Page);
    }

    [Fact]
    public void Empty_path_shows_dash_placeholder()
    {
        Assert.Contains("MapPathDisplay", Page);               // VM 提供 — 占位
    }

    [Fact]
    public void Property_form_uses_96_column_and_compact_narrow_mode()
    {
        Assert.Contains("PropsWide", Page);
        Assert.Contains("PropsNarrow", Page);                  // 紧凑模式整组上下
        Assert.Contains("ColumnDefinitions=\"96,*\"", Page);   // 编辑表单标签列 96
        Assert.Contains("Spacing=\"2\"", Page);                // 紧凑标签→字段 2~4
        Assert.Contains("Spacing=\"6\"", Page);                // 紧凑字段组 6~8
    }

    [Fact]
    public void Button_group_keeps_spacing_6_and_critical_actions()
    {
        Assert.Contains("ColumnSpacing=\"6\"", Page);
        Assert.Contains("RowSpacing=\"6\"", Page);
        Assert.Contains("新建地图", Page);
        Assert.Contains("应用地图属性", Page);
        Assert.Contains("撤销地图修改", Page);
    }

    [Fact]
    public void Each_page_has_single_vertical_scroll_container()
    {
        // 地图页：外层唯一 ScrollViewer + MapPagePanel（内部无 ScrollViewer）
        Assert.Contains("<ScrollViewer VerticalScrollBarVisibility=\"Auto\">", Editor);
        Assert.DoesNotContain("<ScrollViewer", Page);          // 页面内部不嵌套纵向滚动
    }

    [Fact]
    public void Map_id_copy_writes_full_untruncated_id()
    {
        var cs = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
            "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapPagePanel.axaml.cs"));
        Assert.Contains("SetTextAsync(vm.MapIdText)", cs); // 复制完整 MapId（非显示压缩值）
        Assert.Contains("复制完整 MapId", Page);
    }

    [Fact]
    public void Map_editor_errors_use_error_token()
    {
        Assert.Contains("Color.Error", Page);                  // W47：错误色 Token
        Assert.DoesNotContain("#C0392B", Page);
    }

    [Fact]
    public void No_forbidden_legacy_values_in_map_pages()
    {
        Assert.DoesNotContain("infoPanel", Page);
        Assert.DoesNotContain("infoPanel", Editor);
        Assert.DoesNotContain("#f7faff", Page);
        Assert.DoesNotContain("#185aa6", Editor);              // 旧蓝选中
        Assert.DoesNotContain("#edf4ff", Editor);
        Assert.DoesNotContain("#8cb2e2", Editor);
        Assert.DoesNotContain("CornerRadius\" Value=\"5\"", Editor);
    }
}

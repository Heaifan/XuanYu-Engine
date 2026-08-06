using System.IO;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4：检查器结构合同（K02/G03/密度）——字号 Token、双模式、96/128、无卡片嵌套。
public sealed class UiD4InspectorContractTests
{
    static readonly string Panel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));

    static readonly string Right = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "Right.axaml"));

    [Fact]
    public void Inspector_fonts_reference_formal_tokens()
    {
        Assert.Contains("Font.Title.Size", Panel);      // 面板标题（对象名）→ Title 16
        Assert.Contains("Font.Section.Size", Panel);    // 空状态标题/分组标题 → Section 14
        Assert.Contains("Font.Label.Size", Panel);      // 字段标签 → Label 12
        Assert.Contains("Font.Body.Size", Panel);       // 字段值 → Body 13
    }

    [Fact]
    public void Inspector_has_both_form_modes_sharing_same_binding()
    {
        Assert.Contains("x:Name=\"WideFields\"", Panel);
        Assert.Contains("x:Name=\"NarrowFields\"", Panel);
        Assert.Contains("ItemsSource=\"{Binding InspectorFields}\"", Panel); // 同一数据源
    }

    [Fact]
    public void Wide_form_uses_96_label_column_and_128_field_min()
    {
        Assert.Contains("ColumnDefinition Width=\"96\"", Panel);
        Assert.Contains("<ColumnDefinition Width=\"*\" MinWidth=\"128\"/>", Panel);
    }

    [Fact]
    public void No_forbidden_values_in_inspector_panel()
    {
        Assert.DoesNotContain("FontSize\" Value=\"15\"", Panel);   // 面板标题 15 禁止
        Assert.DoesNotContain("FontSize\" Value=\"12\"", Panel);   // 字段值落默认 12 禁止（显式 Token）
        Assert.DoesNotContain("#6b7688", Panel);
        Assert.DoesNotContain("#253247", Panel);
        Assert.DoesNotContain("#243149", Panel);
        Assert.DoesNotContain("infoPanel", Panel);                 // 分组不套卡片
    }

    [Fact]
    public void Inspector_group_uses_full_width_header_with_separator()
    {
        Assert.Contains("基础信息", Panel);
        Assert.Contains("fieldSeparator", Panel);                  // 1 DIP 底部分隔线
        Assert.Contains("Classes=\"section\"", Panel);             // 全宽分组标题
    }

    [Fact]
    public void Debug_tab_label_column_migrated_to_96()
    {
        Assert.Contains("ColumnDefinitions=\"96,*\"", Right);      // W44：调试页标签列 96
        Assert.DoesNotContain("ColumnDefinitions=\"70,*\"", Right);
    }

    [Fact]
    public void Empty_state_keeps_single_primary_entry()
    {
        Assert.Contains("未选择对象", Panel);
        Assert.Contains("IsEmptySelection", Panel);
    }
}

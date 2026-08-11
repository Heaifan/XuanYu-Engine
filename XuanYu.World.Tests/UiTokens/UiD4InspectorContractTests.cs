using System.IO;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4/D4-F1：检查器结构合同——只读键值行单行双列（标签 80/值 *）、
// 公共语义样式、无卡片嵌套、调试页 96 列。
public sealed class UiD4InspectorContractTests
{
    static readonly string Panel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));

    static readonly string Right = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "EditorRightTabs.axaml"));

    [Fact]
    public void Inspector_fonts_reference_formal_tokens()
    {
        Assert.Contains("Font.Title.Size", Panel);      // 面板标题（对象名）→ Title 16
        Assert.Contains("Font.Section.Size", Panel);    // 空状态标题 → Section 14
        Assert.Contains("Classes=\"uiLabel\"", Panel);  // 字段标签 → 公共 Label 12
        Assert.Contains("Classes=\"uiValue\"", Panel);  // 字段值 → 公共 Body 13
    }

    [Fact]
    public void Readonly_rows_use_single_horizontal_grid()
    {
        // D4-F1：只读字段无双布局树（WideFields/NarrowFields 已删除），一套水平 Grid 单行双列
        Assert.DoesNotContain("WideFields", Panel);
        Assert.DoesNotContain("NarrowFields", Panel);
        Assert.DoesNotContain("ColumnDefinition Width=\"96\"", Panel);
        Assert.Contains("ColumnDefinitions=\"80,*\"", Panel);      // ReadonlyKeyValueRow 默认标签列 80
        Assert.Contains("ToolTip.Tip=\"{Binding Value}\"", Panel); // 值省略后 Tooltip 完整值
    }

    [Fact]
    public void No_forbidden_values_in_inspector_panel()
    {
        Assert.DoesNotContain("FontSize\" Value=\"15\"", Panel);   // 面板标题 15 禁止
        Assert.DoesNotContain("FontSize\" Value=\"12\"", Panel);   // 字段值落默认 12 禁止（显式 Token）
        Assert.DoesNotContain("infoPanel", Panel);                 // 分组不套卡片
    }

    [Fact]
    public void Inspector_group_uses_full_width_header_with_separator()
    {
        Assert.Contains("基础信息", Panel);
        Assert.Contains("fieldSeparator", Panel);                  // 1 DIP 底部分隔线
        Assert.Contains("Classes=\"uiSection\"", Panel);           // 公共分组标题
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
        Assert.Contains("uiMultiline", Panel);                     // 空状态说明为显式多行类
    }
}

using System.IO;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4/D4-F1：检查器结构合同——只读键值行单行双列（标签 80/值 *）、
// 公共语义样式、无卡片嵌套、调试页 96 列。
public sealed class UiD4InspectorContractTests
{
    static readonly string Panel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));

    static readonly string Entity = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "EntityInspectorPanel.axaml"));

    static readonly string Right = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "EditorRightTabs.axaml"));

    [Fact]
    public void Inspector_fonts_reference_formal_tokens()
    {
        Assert.Contains("<xy:XYHeading", Panel);        // 面板标题（对象名）→ XYUI-1 Heading
        Assert.Contains("<xy:XYSectionTitle", Panel);  // 语义分组标题 → XYUI-1 SectionTitle
        Assert.Contains("<xy:XYLabel", Panel);         // 字段标签 → XYUI-1 Label
        Assert.Contains("<xy:XYText", Panel);          // 字段值 → XYUI-1 Text
        Assert.DoesNotContain("Classes=\"uiLabel\"", Panel);
        Assert.DoesNotContain("Classes=\"uiValue\"", Panel);
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
        Assert.Contains("InspectorCategories", Panel);
        Assert.Contains("SearchBox", Panel);
        Assert.DoesNotContain("XYPager", Panel);
    }

    [Fact]
    public void Debug_tab_is_not_a_production_right_tab()
    {
        Assert.DoesNotContain("Id=\"debug\"", Right);
        Assert.DoesNotContain("DebugWorkspace", Right);
    }

    [Fact]
    public void Empty_state_keeps_single_primary_entry()
    {
        Assert.Contains("未选择对象", Panel);
        Assert.Contains("IsInspectorEmpty", Panel);
        Assert.Contains("<xy:XYEmptyText", Panel);
        Assert.Contains("选择地图、点、道路或区域以查看属性。", Panel);
        Assert.DoesNotContain("uiMultiline", Panel);
    }

    [Fact]
    public void Entity_inspector_uses_real_xyui_editors_and_section_rails()
    {
        Assert.Contains("IsInspectorBasicExpanded", Entity);
        Assert.Contains("IsInspectorTechnicalExpanded", Entity);
        Assert.Contains("<xy:XYTextField", Entity);
        Assert.Equal(3, Count(Entity, "<xy:XYVectorProperty"));
        Assert.Contains("Mode=OneWay", Entity);
        Assert.DoesNotContain("<TextBox", Entity);
    }

    [Fact]
    public void Inspector_input_routing_preserves_textbox_focus()
    {
        var shortcuts = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
            "..", "..", "..", "..", "XuanYu.Editor.UI", "Win", "UiWin.Shortcuts.cs"));
        Assert.Contains("GetFocusedElement() is TextBox", shortcuts);
    }

    [Fact]
    public void Technical_info_prevents_overlap_with_auto_rows()
    {
        Assert.Contains("RowDefinitions=\"Auto,Auto,Auto\"", Entity); Assert.Contains("Variant=\"Technical\"", Entity); Assert.Contains("<xy:XYSelectableText", Entity);
    }
    static int Count(string source, string value) => source.Split(value, StringSplitOptions.None).Length - 1;
}

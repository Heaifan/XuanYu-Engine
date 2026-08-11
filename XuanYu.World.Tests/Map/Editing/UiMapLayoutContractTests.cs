using System.IO;

namespace XuanYu.World.Tests.Map.Editing;

// EDITOR-A-R3：Map Context 只在 Edit Mode 的左侧出现；Inspector 与 Shell 保持全局。
public sealed class UiMapLayoutContractTests
{
    static readonly string Left = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Left", "Left.axaml"));

    static readonly string MapEditor = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapEditorPanel.axaml"));

    static readonly string Right = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "EditorRightTabs.axaml"));

    static readonly string Top = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Top", "Top.axaml"));

    [Fact]
    public void Left_keeps_global_tabs_and_edit_context_tab()
    {
        Assert.Contains("Header=\"项目\"", Left);
        Assert.Contains("Header=\"层级\"", Left);
        Assert.Contains("Header=\"地图\"", Left);
        Assert.DoesNotContain("LayerPanel", Left);
    }

    [Fact]
    public void Map_editor_has_three_second_level_tabs()
    {
        Assert.Contains("Header=\"地图\"", MapEditor);
        Assert.Contains("Header=\"环境\"", MapEditor);
    }

    [Fact]
    public void Layer_ui_is_removed_from_map_editor_context()
    {
        Assert.DoesNotContain("LayerPanel", MapEditor);
        Assert.DoesNotContain("LayerInspectorPanel", MapEditor);
    }

    [Fact]
    public void Global_inspector_has_no_layer_panel()
    {
        Assert.Contains("LayerInspectorPanel", File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml")));
    }

    // F2：顶部"添加"菜单扁平化——无"基础实体"级联，"立方体"为直接子项。
    [Fact]
    public void Add_menu_has_no_category_level()
    {
        Assert.DoesNotContain("基础实体", Top);
        Assert.Contains("<MenuItem Header=\"立方体\" Command=\"{Binding RunCommand}\" CommandParameter=\"添加立方体\"/>", Top);
    }

    // EDITOR-A-R3：右侧顶层仅保留全局检查器与调试，地图 Context 不再替换整块右栏。
    [Fact]
    public void Right_keeps_only_global_top_tabs()
    {
        Assert.Contains("Header=\"检查器\"", Right);
        Assert.DoesNotContain("Header=\"地图编辑器\"", Right);
        Assert.Contains("Header=\"调试\"", Right);
        Assert.DoesNotContain("Header=\"偏好\"", Right);
        Assert.DoesNotContain("Header=\"模式\"", Right);
        Assert.DoesNotContain("PropertyItems", Right);
    }
}

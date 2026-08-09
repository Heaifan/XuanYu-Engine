using System.IO;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4-F1：图层 UI 归位合同——左侧仅项目/层级，图层管理迁入右侧地图编辑器二级页。
// 源码合同测试（只读仓库 axaml），防止图层模块回到错误的全局导航层级。
public sealed class UiMapLayoutContractTests
{
    static readonly string Left = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Left", "Left.axaml"));

    static readonly string MapEditor = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapEditorPanel.axaml"));

    static readonly string Right = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "Right.axaml"));

    static readonly string Top = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Top", "Top.axaml"));

    [Fact]
    public void Left_keeps_only_project_and_hierarchy_tabs()
    {
        Assert.Contains("Header=\"项目\"", Left);
        Assert.Contains("Header=\"层级\"", Left);
        Assert.DoesNotContain("Header=\"图层\"", Left);
        Assert.DoesNotContain("LayerPanel", Left);
    }

    [Fact]
    public void Map_editor_has_three_second_level_tabs()
    {
        Assert.Contains("Header=\"地图\"", MapEditor);
        Assert.Contains("Header=\"图层\"", MapEditor);
        Assert.Contains("Header=\"环境\"", MapEditor);
    }

    [Fact]
    public void Layer_ui_lives_inside_map_editor_layer_tab()
    {
        Assert.Contains("<local:LayerPanel HorizontalAlignment=\"Stretch\"/>", MapEditor);
        Assert.Contains("<local:LayerInspectorPanel", MapEditor);
        Assert.Contains("HorizontalScrollBarVisibility=\"Disabled\"", MapEditor);
        Assert.Contains("HorizontalAlignment=\"Stretch\"", MapEditor);
    }

    [Fact]
    public void Global_inspector_has_no_layer_panel()
    {
        Assert.DoesNotContain("LayerInspectorPanel", Right);
    }

    // F2：顶部"添加"菜单扁平化——无"基础实体"级联，"立方体"为直接子项。
    [Fact]
    public void Add_menu_has_no_category_level()
    {
        Assert.DoesNotContain("基础实体", Top);
        Assert.Contains("<MenuItem Header=\"立方体\" Command=\"{Binding RunCommand}\" CommandParameter=\"添加立方体\"/>", Top);
    }

    // F2：右侧顶层仅 检查器 / 地图编辑器 / 调试。
    [Fact]
    public void Right_keeps_only_three_top_tabs()
    {
        Assert.Contains("Header=\"检查器\"", Right);
        Assert.Contains("Header=\"地图编辑器\"", Right);
        Assert.Contains("Header=\"调试\"", Right);
        Assert.DoesNotContain("Header=\"偏好\"", Right);
        Assert.DoesNotContain("Header=\"模式\"", Right);
        Assert.DoesNotContain("PropertyItems", Right);
    }
}

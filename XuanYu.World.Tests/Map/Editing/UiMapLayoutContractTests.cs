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
        Assert.Contains("<local:LayerPanel/>", MapEditor);
        Assert.Contains("<local:LayerInspectorPanel", MapEditor);
    }

    [Fact]
    public void Global_inspector_has_no_layer_panel()
    {
        Assert.DoesNotContain("LayerInspectorPanel", Right);
    }
}

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

    static readonly string FileModule = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Top", "FileModule.axaml"));

    static readonly string Top = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Top", "Top.axaml"));

    [Fact]
    public void Left_is_only_local_project_file_content()
    {
        Assert.DoesNotContain("XYNavigationRail", Left);
        Assert.DoesNotContain("WorkspaceRail", Left);
        Assert.Contains("<local:ProjectWorkspace", Left);
        Assert.Contains("<xy:XYTabs", Left);
        Assert.Contains("Label=\"项目\"", Left);
        Assert.Contains("Label=\"文件\"", Left);
        Assert.Contains("暂无文件", Left);
        Assert.DoesNotContain("<local:HierarchyWorkspace", Left);
        Assert.DoesNotContain("<local:MapEditorPanel", Left);
        Assert.DoesNotContain("LayerPanel", Left);
    }

    [Fact]
    public void Map_editor_has_frozen_content_navigation_tabs()
    {
        Assert.Contains("<xy:XYPager", MapEditor);
        Assert.Contains("Id=\"base\" Label=\"基础\"", MapEditor);
        Assert.Contains("Id=\"environment\" Label=\"环境\"", MapEditor);
        Assert.Contains("Id=\"data\" Label=\"数据\"", MapEditor);
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
        Assert.Contains("<xy:XYMenuItem Label=\"立方体\"", FileModule);
        Assert.Contains("CommandParameter=\"添加立方体\"", FileModule);
    }

    // AREA-D-R2：地图 Context 由 Inspector 内容宿主承载，不再作为 Right sibling。
    [Fact]
    public void Right_keeps_global_tabs_and_rehomes_edit_contexts()
    {
        Assert.Contains("Label=\"检查器\"", Right);
        Assert.Contains("Label=\"调试\"", Right);
        var rightShell = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "Right.axaml"));
        var inspector = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));
        Assert.Contains("<local:MapEditorPanel", inspector);
        Assert.DoesNotContain("<local:MapEditorPanel", rightShell);
        Assert.DoesNotContain("<local:RegionalAuthoringPanel", rightShell);
        Assert.DoesNotContain("<local:RegionalAuthoringPanel", File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml")));
        Assert.DoesNotContain("Header=\"偏好\"", Right);
        Assert.DoesNotContain("Header=\"模式\"", Right);
        Assert.DoesNotContain("PropertyItems", Right);
    }
}

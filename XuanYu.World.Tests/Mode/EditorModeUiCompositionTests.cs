using System.IO;

namespace XuanYu.World.Tests.Mode;

public sealed class EditorModeUiCompositionTests
{
    static readonly string Root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
    static string Read(params string[] path) => File.ReadAllText(Path.Combine([Root, .. path]));

    [Fact]
    public void Top_has_only_the_unified_mode_control()
    {
        var top = Read("XuanYu.Editor.UI", "Top", "Top.axaml");
        Assert.Contains("WorkspaceSelector", top);
        Assert.DoesNotContain("进入编辑", top); Assert.DoesNotContain("返回管理", top);
        Assert.DoesNotContain("Tab 切换", top); Assert.DoesNotContain("编辑目标", top);
    }

    [Fact]
    public void Root_has_only_foot_but_one_main_and_viewport()
    {
        var root = Read("XuanYu.Editor.UI", "Root", "UiRoot.axaml");
        Assert.DoesNotContain("BottomDockHost", root); Assert.Equal(1, Count(root, "<local:Foot"));
        Assert.Equal(1, Count(root, "<local:Main"));
        Assert.Equal(1, Count(Read("XuanYu.Editor.UI", "Main", "Main.axaml"), "VulkanViewport"));
    }

    [Fact]
    public void Bottom_asset_browser_is_retired_but_file_import_remains()
    {
        Assert.False(File.Exists(Path.Combine(Root, "XuanYu.Editor.UI", "Shell", "BottomDockHost.axaml")));
        var file = Read("XuanYu.Editor.UI", "Top", "FileModule.axaml");
        Assert.Contains("CommandParameter=\"导入 GLB\"", file);
    }

    [Fact]
    public void Project_and_inspector_remain_global_shell_panels()
    {
        var left = Read("XuanYu.Editor.UI", "Left", "Left.axaml");
        Assert.Contains("<local:ProjectWorkspace", left);
        Assert.DoesNotContain("XYNavigationRail", left);
        Assert.Contains("<local:HierarchyWorkspace", Read("XuanYu.Editor.UI", "Right", "EditorRightTabs.axaml"));
        Assert.Contains("<xy:XYTabs", Read("XuanYu.Editor.UI", "Right", "EditorRightTabs.axaml"));
    }

    [Fact]
    public void Map_context_lives_in_inspector_content_and_not_right_sibling()
    {
        Assert.Contains("<local:MapEditorPanel", Read("XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));
        Assert.DoesNotContain("<local:MapEditorPanel", Read("XuanYu.Editor.UI", "Right", "Right.axaml"));
        Assert.Contains("MapPagePanel", Read("XuanYu.Editor.UI", "Right", "MapPagePanel.axaml"));
        Assert.Contains("EditorLayerDock", Read("XuanYu.Editor.UI", "Right", "Right.axaml"));
    }

    [Fact]
    public void Region_context_shows_the_current_drawing_target_and_old_right_map_tab_is_retired()
    {
        var rightShell = Read("XuanYu.Editor.UI", "Right", "Right.axaml");
        var inspector = Read("XuanYu.Editor.UI", "Right", "InspectorPanel.axaml");
        var region = Read("XuanYu.Editor.UI", "Left", "RegionPanel.axaml");
        var right = Read("XuanYu.Editor.UI", "Right", "EditorRightTabs.axaml");
        Assert.DoesNotContain("RegionalAuthoringPanel", rightShell);
        Assert.Contains("<local:RegionalAuthoringPanel", inspector);
        Assert.Equal(1, Count(inspector, "<ScrollViewer"));
        Assert.Contains("RegionPanel", Read("XuanYu.Editor.UI", "Right", "RegionalAuthoringPanel.axaml"));
        Assert.Contains("当前绘制目标", region);
        Assert.Contains("RegionDrawingTargetName", region);
        Assert.Contains("RegionDrawingTargetStatus", region);
        Assert.Contains("CanUndoRegionDrawingVertex", region);
        Assert.DoesNotContain("Text=\"绘制区域\"", Read("XuanYu.Editor.UI", "Top", "Top.axaml"));
        Assert.DoesNotContain("Header=\"地图编辑器\"", right);
    }

    [Fact]
    public void Shortcut_routes_tab_without_changing_escape_contract()
    {
        var code = Read("XuanYu.Editor.UI", "Win", "UiWin.Shortcuts.cs");
        Assert.Contains("e.Key == Key.Tab", code); Assert.Contains("ToggleEditorMode", code);
        Assert.Contains("e.Key != Key.Escape", code);
    }

    [Fact]
    public void Selector_separates_mode_and_workspace_and_uses_one_toggle_gesture()
    {
        var selector = Read("XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml");
        var code = Read("XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml.cs");
        Assert.Contains("Command=\"{Binding ToggleEditorModeCommand}\"", selector);
        Assert.Contains("Command=\"{Binding SwitchWorkspaceCommand}\"", selector);
        Assert.Contains("CheckKind=\"Radio\"", selector);
        Assert.DoesNotContain("DoubleTapped", selector);
        Assert.DoesNotContain("DoubleTapped", code);
        Assert.DoesNotContain("ToggleEditorMode", code);
    }

    static int Count(string text, string value) => text.Split(value).Length - 1;
}

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
        var top = Read("XuanYu.Editor.UI", "Top", "Top.axaml");
        Assert.Contains("CommandParameter=\"导入 GLB\"", top);
    }

    [Fact]
    public void Project_and_inspector_remain_global_shell_panels()
    {
        Assert.Contains("Header=\"项目\"", Read("XuanYu.Editor.UI", "Left", "Left.axaml"));
        Assert.Contains("Header=\"检查器\"", Read("XuanYu.Editor.UI", "Right", "Right.axaml"));
    }

    [Fact]
    public void Map_context_moves_to_left_and_inspector()
    {
        Assert.Contains("Header=\"地图\"", Read("XuanYu.Editor.UI", "Left", "Left.axaml"));
        Assert.Contains("MapFormPanel", Read("XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));
    }

    [Fact]
    public void Region_context_is_placeholder_and_old_right_map_tab_is_retired()
    {
        var left = Read("XuanYu.Editor.UI", "Left", "Left.axaml");
        var right = Read("XuanYu.Editor.UI", "Right", "Right.axaml");
        Assert.Contains("REGION-A 接入后显示区域列表", left);
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
    public void Selector_hides_menu_in_manage_and_routes_double_tap_to_shared_toggle()
    {
        var selector = Read("XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml");
        var code = Read("XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml.cs");
        Assert.Contains("IsVisible=\"{Binding IsManageMode}\"", selector);
        Assert.Contains("IsVisible=\"{Binding IsEditMode}\"", selector);
        Assert.Contains("ChevronDownIcon", selector); Assert.Contains("ToggleType=\"Radio\"", selector);
        Assert.Contains("ModeSurface_DoubleTapped", code); Assert.Contains("ToggleEditorMode", code);
    }

    static int Count(string text, string value) => text.Split(value).Length - 1;
}

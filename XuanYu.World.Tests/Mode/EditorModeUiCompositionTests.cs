using System.IO;

namespace XuanYu.World.Tests.Mode;

public sealed class EditorModeUiCompositionTests
{
    static readonly string Root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
    static string Read(params string[] path) => File.ReadAllText(Path.Combine([Root, .. path]));

    [Fact]
    public void Top_binds_mode_target_and_tab_action()
    {
        var top = Read("XuanYu.Editor.UI", "Top", "Top.axaml");
        Assert.Contains("CurrentModeDisplayName", top); Assert.Contains("ToggleEditorModeCommand", top);
        Assert.Contains("WorkspaceSelector", top);
    }

    [Fact]
    public void Root_has_asset_browser_but_one_main_and_viewport()
    {
        var root = Read("XuanYu.Editor.UI", "Root", "UiRoot.axaml");
        Assert.Contains("BottomDockHost", root); Assert.Equal(1, Count(root, "<local:Main"));
        Assert.Equal(1, Count(Read("XuanYu.Editor.UI", "Main", "Main.axaml"), "VulkanViewport"));
    }

    [Fact]
    public void Asset_browser_reuses_import_command()
    {
        var asset = Read("XuanYu.Editor.UI", "Shell", "BottomDockHost.axaml");
        Assert.Contains("资源浏览器", asset); Assert.Contains("导入 GLB", asset);
        Assert.Contains("CommandParameter=\"导入 GLB\"", asset);
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

    static int Count(string text, string value) => text.Split(value).Length - 1;
}

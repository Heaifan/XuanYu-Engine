using System.IO;

namespace XuanYu.World.Tests.Workspace;

// EDITOR-A-R2：源码组合合同，禁止 Workspace Host 接管或复制 Main / VulkanViewport。
public sealed class EditorWorkspaceUiCompositionTests
{
    static readonly string Root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
    static string Read(params string[] path) => File.ReadAllText(Path.Combine([Root, .. path]));

    [Fact]
    public void Ui_root_keeps_exactly_one_main_and_uses_workspace_hosts()
    {
        var axaml = Read("XuanYu.Editor.UI", "Root", "UiRoot.axaml");
        Assert.Equal(1, Count(axaml, "<local:Main"));
        Assert.Contains("<local:WorkspaceLeftHost", axaml);
        Assert.Contains("<local:WorkspaceRightHost", axaml);
    }

    [Fact]
    public void Main_keeps_exactly_one_vulkan_viewport()
    {
        var axaml = Read("XuanYu.Editor.UI", "Main", "Main.axaml");
        Assert.Equal(1, Count(axaml, "<local:VulkanViewport"));
    }

    [Fact]
    public void Workspace_hosts_do_not_contain_main_or_viewport()
    {
        var left = Read("XuanYu.Editor.UI", "Workspace", "WorkspaceLeftHost.axaml");
        var right = Read("XuanYu.Editor.UI", "Workspace", "WorkspaceRightHost.axaml");
        Assert.DoesNotContain("<local:Main", left + right);
        Assert.DoesNotContain("VulkanViewport", left + right);
    }

    [Fact]
    public void Map_context_remains_accessible_through_existing_left_and_right()
    {
        Assert.Contains("<local:Left", Read("XuanYu.Editor.UI", "Workspace", "WorkspaceLeftHost.axaml"));
        Assert.Contains("<local:Right", Read("XuanYu.Editor.UI", "Workspace", "WorkspaceRightHost.axaml"));
    }

    [Fact]
    public void Region_context_contains_only_declared_placeholders()
    {
        var left = Read("XuanYu.Editor.UI", "Workspace", "WorkspaceLeftHost.axaml");
        var right = Read("XuanYu.Editor.UI", "Workspace", "WorkspaceRightHost.axaml");
        Assert.Contains("区域列表将在 REGION-A 接入", left);
        Assert.Contains("REGION-A 接入后显示正式属性", right);
        Assert.DoesNotContain("RegionDrawing", left + right);
    }

    [Fact]
    public void Toolbar_has_selector_and_hides_map_only_tools_in_region_workspace()
    {
        var top = Read("XuanYu.Editor.UI", "Top", "Top.axaml");
        Assert.Contains("<local:WorkspaceSelector", top);
        Assert.Contains("IsVisible=\"{Binding IsMapWorkspace}\"", top);
        Assert.Contains("CommandParameter=\"RegionEditor\"", Read("XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml"));
    }

    static int Count(string text, string value) => text.Split(value).Length - 1;
}

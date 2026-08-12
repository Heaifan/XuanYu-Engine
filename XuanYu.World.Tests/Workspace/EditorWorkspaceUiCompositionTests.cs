using System.IO;

namespace XuanYu.World.Tests.Workspace;

// EDITOR-A-R2：源码组合合同，禁止 Workspace Host 接管或复制 Main / VulkanViewport。
public sealed class EditorWorkspaceUiCompositionTests
{
    static readonly string Root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");
    static string Read(params string[] path) => File.ReadAllText(Path.Combine([Root, .. path]));

    [Fact]
    public void Ui_root_keeps_exactly_one_main_and_global_shell_panels()
    {
        var axaml = Read("XuanYu.Editor.UI", "Root", "UiRoot.axaml");
        Assert.Equal(1, Count(axaml, "<local:Main"));
        Assert.Contains("<local:Left", axaml);
        Assert.Contains("<local:Right", axaml);
        Assert.DoesNotContain("WorkspaceLeftHost", axaml);
    }

    [Fact]
    public void Main_keeps_exactly_one_vulkan_viewport()
    {
        var axaml = Read("XuanYu.Editor.UI", "Main", "Main.axaml");
        Assert.Equal(1, Count(axaml, "<local:VulkanViewport"));
    }

    [Fact]
    public void Workspace_selector_does_not_contain_main_or_viewport()
    {
        var selector = Read("XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml");
        Assert.DoesNotContain("<local:Main", selector);
        Assert.DoesNotContain("VulkanViewport", selector);
    }

    [Fact]
    public void Map_context_remains_accessible_through_existing_left_and_inspector()
    {
        Assert.Contains("Header=\"地图\"", Read("XuanYu.Editor.UI", "Left", "Left.axaml"));
        Assert.Contains("<local:MapFormPanel", Read("XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));
    }

    [Fact]
    public void Region_context_contains_the_drawing_target_and_declared_inspector_placeholder()
    {
        var left = Read("XuanYu.Editor.UI", "Left", "Left.axaml");
        var right = Read("XuanYu.Editor.UI", "Right", "InspectorPanel.axaml");
        Assert.Contains("当前绘制目标", left);
        Assert.Contains("RegionDrawingTargetName", left);
        Assert.Contains("RegionDrawingTargetStatus", left);
        Assert.Contains("REGION-A 接入后显示正式属性", right);
    }

    [Fact]
    public void Toolbar_has_selector_and_hides_map_only_tools_in_region_workspace()
    {
        var top = Read("XuanYu.Editor.UI", "Top", "Top.axaml");
        Assert.Contains("<local:WorkspaceSelector", top);
        Assert.Contains("IsVisible=\"{Binding IsMapEditMode}\"", top);
        Assert.Contains("CommandParameter=\"RegionEditor\"", Read("XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml"));
    }

    static int Count(string text, string value) => text.Split(value).Length - 1;
}

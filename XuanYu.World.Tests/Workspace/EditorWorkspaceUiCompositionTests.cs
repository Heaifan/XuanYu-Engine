using System.IO;

namespace XuanYu.World.Tests.Workspace;

// EDITOR-A-R2：源码组合合同，禁止 Workspace Host 接管或复制 Main / VulkanViewport。
public sealed class EditorWorkspaceUiCompositionTests
{
    static readonly string Root = TestSourceRoot.Root;
    static string Read(params string[] path) => File.ReadAllText(Path.Combine([Root, .. path]));

    [Fact]
    public void Ui_root_keeps_exactly_one_main_and_global_shell_panels()
    {
        var axaml = Read("XuanYu.Editor.UI", "Shell", "Composition", "Application", "UiRoot.axaml");
        Assert.Equal(1, Count(axaml, "<local:Main"));
        Assert.Contains("<local:Left", axaml);
        Assert.Contains("<local:Right", axaml);
        Assert.DoesNotContain("WorkspaceLeftHost", axaml);
    }

    [Fact]
    public void Main_keeps_exactly_one_vulkan_viewport()
    {
        var axaml = Read("XuanYu.Editor.UI", "Shell", "Composition", "Application", "Main.axaml");
        Assert.Equal(1, Count(axaml, "<local:VulkanViewport"));
    }

    [Fact]
    public void Workspace_selector_does_not_contain_main_or_viewport()
    {
        var selector = Read("XuanYu.Editor.UI", "Panels", "Workspace", "WorkspaceSelector.axaml");
        Assert.DoesNotContain("<local:Main", selector);
        Assert.DoesNotContain("VulkanViewport", selector);
        Assert.DoesNotContain("道路编辑", selector);
        Assert.DoesNotContain("RoadEditor", selector);
    }

    [Fact]
    public void Map_context_is_rehomed_to_right_and_inspector_remains_available()
    {
        var left = Read("XuanYu.Editor.UI", "Panels", "Navigation", "LeftWorkspace", "Left.axaml");
        Assert.DoesNotContain("XYNavigationRail", left);
        var inspector = Read("XuanYu.Editor.UI", "Panels", "Inspector", "InspectorPanels", "InspectorPanel.axaml");
        Assert.Contains("InspectorPropertyRow", inspector);
        Assert.Equal(1, Count(inspector, "<local:MapEditorPanel"));
        Assert.DoesNotContain("<local:MapEditorPanel", Read("XuanYu.Editor.UI", "Panels", "Inspector", "InspectorPanels", "Right.axaml"));
        Assert.Contains("<local:MapPagePanel", Read("XuanYu.Editor.UI", "MapUI", "Presentation", "EditorPanels", "MapEditorPanel.axaml"));
        Assert.DoesNotContain("<local:MapFormPanel", Read("XuanYu.Editor.UI", "Panels", "Inspector", "InspectorPanels", "InspectorPanel.axaml"));
    }

    [Fact]
    public void Read_only_inspector_values_wrap_and_keep_full_value_available()
    {
        var presenter = Read("XuanYu.Editor.UI", "Panels", "Inspector", "InspectorReadOnlyValuePresenter.axaml");
        Assert.Contains("TextWrapping=\"Wrap\"", presenter);
        Assert.Contains("TextTrimming=\"None\"", presenter);
    }

    [Fact]
    public void Region_context_contains_the_drawing_target_and_declared_inspector_placeholder()
    {
        var region = Read("XuanYu.Editor.UI", "Panels", "Tools", "ContextTools", "ContextToolBar.axaml");
        var code = Read("XuanYu.Editor.UI", "Panels", "Tools", "ContextTools", "ContextToolBar.axaml.cs");
        var right = Read("XuanYu.Editor.UI", "Panels", "Inspector", "InspectorPanels", "InspectorPanel.axaml");
        Assert.DoesNotContain("RegionalAuthoringPanel", right);
        Assert.Contains("DrawButtonLabel", region);
        Assert.Contains("区域面", code);
        Assert.Contains("InspectorPropertyRow", right);
        Assert.Contains("CanCompleteDrawing", region);
    }

    [Fact]
    public void Toolbar_has_selector_and_disables_map_only_tools_in_region_workspace()
    {
        var top = Read("XuanYu.Editor.UI", "Panels", "Tools", "WorkspaceModules", "Top.axaml");
        var snap = Read("XuanYu.Editor.UI", "Panels", "Tools", "ContextTools", "SnapModule.axaml");
        Assert.Contains("<local:WorkspaceSelector", top);
        Assert.Contains("IsEnabled=\"{Binding IsMapEditMode}\"", snap);
        Assert.Contains("SelectFeatureWorkspaceCommand", Read("XuanYu.Editor.UI", "Panels", "Workspace", "WorkspaceSelector.axaml.cs"));
    }

    static int Count(string text, string value) => text.Split(value).Length - 1;
}

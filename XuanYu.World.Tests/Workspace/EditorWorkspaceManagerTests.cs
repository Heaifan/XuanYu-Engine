using System.Reflection;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.Workspace;

// EDITOR-A-R1：Workspace 纯合同，不涉及可见 UI 或 Region Drawing。
public sealed class EditorWorkspaceManagerTests
{
    [Fact]
    public void Default_workspace_is_map_editor_with_select_tool()
    {
        var manager = new EditorWorkspaceManager();
        Assert.Same(EditorWorkspaceDefinitions.MapEditor, manager.CurrentWorkspace);
        Assert.Equal(EditorWorkspaceTool.Select, manager.CurrentWorkspace.DefaultTool);
    }

    [Fact]
    public void Map_to_region_switch_preserves_context_and_resets_to_select()
    {
        var transition = new EditorWorkspaceManager().Switch(EditorWorkspaceId.RegionEditor);
        Assert.True(transition.Changed);
        Assert.True(transition.EndsTemporaryToolState);
        Assert.True(transition.PreservesWorldContext);
        Assert.True(transition.PreservesCameraContext);
        Assert.True(transition.PreservesSelection);
        Assert.Equal(EditorWorkspaceTool.Select, transition.NextTool);
    }

    [Fact]
    public void Region_to_map_switch_returns_to_map_workspace()
    {
        var manager = new EditorWorkspaceManager();
        manager.Switch(EditorWorkspaceId.RegionEditor);
        var transition = manager.Switch(EditorWorkspaceId.MapEditor);
        Assert.True(transition.Changed);
        Assert.Same(EditorWorkspaceDefinitions.MapEditor, manager.CurrentWorkspace);
    }

    [Fact]
    public void Repeating_current_workspace_does_not_create_new_state()
    {
        var manager = new EditorWorkspaceManager();
        var current = manager.CurrentWorkspace;
        var transition = manager.Switch(EditorWorkspaceId.MapEditor);
        Assert.False(transition.Changed);
        Assert.False(transition.EndsTemporaryToolState);
        Assert.Same(current, manager.CurrentWorkspace);
    }

    [Fact]
    public void Enter_uses_the_same_switch_contract()
    {
        var manager = new EditorWorkspaceManager();
        var transition = manager.Enter(EditorWorkspaceId.RegionEditor);
        Assert.True(transition.Changed);
        Assert.Same(EditorWorkspaceDefinitions.RegionEditor, manager.CurrentWorkspace);
    }

    [Fact]
    public void Leave_ends_temporary_tool_without_clearing_current_workspace()
    {
        var manager = new EditorWorkspaceManager();
        var transition = manager.Leave();
        Assert.True(transition.IsLeave);
        Assert.True(transition.EndsTemporaryToolState);
        Assert.Same(EditorWorkspaceDefinitions.MapEditor, manager.CurrentWorkspace);
    }

    [Fact]
    public void Manager_has_no_world_or_camera_state_and_editor_has_no_vulkan_reference()
    {
        var fields = typeof(EditorWorkspaceManager).GetFields(
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.DoesNotContain(fields, field => field.FieldType.Name.Contains("World") ||
            field.FieldType.Name.Contains("Camera"));
        var references = typeof(EditorWorkspaceManager).Assembly.GetReferencedAssemblies();
        Assert.DoesNotContain(references, reference => reference.Name!.Contains("Vulkan"));
    }

    [Fact]
    public void Region_workspace_has_no_region_drawing_tool()
    {
        Assert.Equal(EditorWorkspaceTool.Select, EditorWorkspaceDefinitions.RegionEditor.DefaultTool);
        Assert.DoesNotContain(Enum.GetNames<EditorWorkspaceTool>(), name =>
            name.Contains("Region", StringComparison.OrdinalIgnoreCase));
    }
}

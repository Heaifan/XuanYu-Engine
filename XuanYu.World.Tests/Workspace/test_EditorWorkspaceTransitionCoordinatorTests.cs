using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.Workspace;

public sealed class test_EditorWorkspaceTransitionCoordinatorTests
{
    [Fact]
    public void Switch_cancels_old_interaction_before_publishing_new_workspace()
    {
        var coordinator = new EditorWorkspaceTransitionCoordinator(new EditorWorkspaceManager());
        var order = new List<string>();

        coordinator.Switch(EditorWorkspaceId.RegionEditor, true,
            () => order.Add("cancel"), () => order.Add("clear"), () => order.Add("sync"),
            () => order.Add("select"), _ => order.Add("publish"));

        Assert.Equal(["cancel", "clear", "sync", "select", "publish"], order);
        Assert.Same(EditorWorkspaceDefinitions.RegionEditor, coordinator.CurrentWorkspace);
    }
}

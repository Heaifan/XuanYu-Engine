using XuanYu.Editor.Mode;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Editor;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class EditorApplicationProjectionTests
{
    [Fact]
    public void Context_and_toolbar_are_derived_from_one_editor_state_snapshot()
    {
        var owner = new EditorStateOwner(() => true);
        var context = new EditorContextOwner();
        var authoring = new EditorAuthoringOwner();
        context.Change(new ChangeEditorContextCommand(EditorContextId.Terrain));
        owner.ChangeTool(new ChangeEditorToolCommand("区域绘制"));

        var state = EditorApplicationStateProjection.Create(
            EditorModeId.Edit,
            EditorWorkspaceDefinitions.RegionEditor, context.Snapshot,
            owner.ToolSnapshot, owner.Snapshot, owner.InteractionSnapshot, authoring.Snapshot,
            lastAuthoringTool: "区域面",
            progressPercentage: 25,
            progressMessage: "读取中",
            isProgressActive: true,
            UiNotificationSnapshot.Empty);

        Assert.Equal(EditorContextId.Terrain, state.Context);
        Assert.Equal("地形", state.ContextLabel);
        Assert.Equal("地形", state.ToolbarLabel);
        Assert.Equal("区域绘制", state.ActiveToolText);
        Assert.Equal(25, state.Progress.Percentage);
    }

    [Fact]
    public void Context_change_is_not_a_viewmodel_local_boolean()
    {
        var owner = new EditorContextOwner();
        Assert.Equal(EditorContextId.Region, owner.Snapshot.Context);

        owner.Change(new ChangeEditorContextCommand(EditorContextId.Terrain));

        Assert.Equal(EditorContextId.Terrain, owner.Snapshot.Context);
        Assert.Equal("地形", owner.Snapshot.ContextText);
    }

    [Fact]
    public void Editor_state_owner_is_provided_by_the_editor_assembly()
    {
        Assert.Same(typeof(EditorWorkspaceManager).Assembly, typeof(EditorStateOwner).Assembly);
        Assert.Same(typeof(EditorWorkspaceManager).Assembly, typeof(EditorContextOwner).Assembly);
        Assert.Same(typeof(EditorWorkspaceManager).Assembly, typeof(EditorAuthoringOwner).Assembly);
    }

    [Fact]
    public void UI_receives_authoring_snapshot_and_cannot_own_the_mutable_session()
    {
        var owner = new EditorAuthoringOwner();
        owner.Begin(new BeginAuthoringInputCommand(AuthoringInputKind.Region));

        Assert.True(owner.Snapshot.IsActive);
        Assert.Equal(AuthoringInputKind.Region, owner.Snapshot.Kind);
    }
}

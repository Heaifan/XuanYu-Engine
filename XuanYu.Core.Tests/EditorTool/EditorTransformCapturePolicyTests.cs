using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.EditorTool;

public sealed class EditorTransformCapturePolicyTests
{
    [Fact]
    public void Move_tool_can_begin_move_gizmo_capture()
    {
        var owner = new EditorStateOwner(() => true);

        owner.ChangeTool(new ChangeEditorToolCommand("移动"));

        Assert.True(EditorTransformCapturePolicy.CanBeginMoveGizmo(owner.ToolSnapshot));
    }

    [Fact]
    public void Rotate_and_scale_do_not_fall_back_to_move_capture()
    {
        var owner = new EditorStateOwner(() => true);

        owner.ChangeTool(new ChangeEditorToolCommand("旋转"));
        Assert.False(EditorTransformCapturePolicy.CanBeginMoveGizmo(owner.ToolSnapshot));
        owner.ChangeTool(new ChangeEditorToolCommand("缩放"));
        Assert.False(EditorTransformCapturePolicy.CanBeginMoveGizmo(owner.ToolSnapshot));
    }

    [Fact]
    public void Snap_toggle_does_not_change_move_capture_tool()
    {
        var owner = new EditorStateOwner(() => true);

        owner.ChangeTool(new ChangeEditorToolCommand("移动"));
        owner.ToggleSnap(new ToggleEditorSnapCommand());

        Assert.Equal(EditorToolId.Move, owner.ToolSnapshot.ActiveTool);
        Assert.True(EditorTransformCapturePolicy.CanBeginMoveGizmo(owner.ToolSnapshot));
    }
}

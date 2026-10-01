using XuanYu.Editor.Mode;

namespace XuanYu.Editor.Tests.Mode;

public sealed class EditorModeManagerTests
{
    [Fact]
    public void Toggle_moves_between_manage_and_edit_without_dropping_world_state()
    {
        var manager = new EditorModeManager();

        var toEdit = manager.Toggle();
        Assert.Equal(EditorModeId.Edit, manager.CurrentMode);
        Assert.True(toEdit.Changed);
        Assert.True(toEdit.PreservesWorld);

        var toManage = manager.Toggle();
        Assert.Equal(EditorModeId.Manage, manager.CurrentMode);
        Assert.True(toManage.Changed);
        Assert.True(toManage.PreservesWorld);
    }
}

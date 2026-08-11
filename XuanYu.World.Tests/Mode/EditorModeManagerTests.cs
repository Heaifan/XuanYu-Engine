using XuanYu.Editor.Mode;

namespace XuanYu.World.Tests.Mode;

public sealed class EditorModeManagerTests
{
    [Fact]
    public void Default_mode_is_manage() => Assert.Equal(EditorModeId.Manage,
        new EditorModeManager().CurrentMode);

    [Fact]
    public void Toggle_enters_edit_preserving_shell_context()
    {
        var transition = new EditorModeManager().Toggle();
        Assert.Equal(EditorModeId.Edit, transition.CurrentMode); Assert.True(transition.Changed);
        Assert.True(transition.PreservesWorld); Assert.True(transition.PreservesCamera);
        Assert.True(transition.PreservesSelection); Assert.True(transition.PreservesAssets);
        Assert.True(transition.PreservesViewport);
    }

    [Fact]
    public void Toggle_from_edit_returns_to_manage()
    {
        var manager = new EditorModeManager(); manager.Toggle();
        Assert.Equal(EditorModeId.Manage, manager.Toggle().CurrentMode);
    }

    [Fact]
    public void Repeating_mode_does_not_create_transition()
    {
        var transition = new EditorModeManager().Switch(EditorModeId.Manage);
        Assert.False(transition.Changed); Assert.False(transition.EndsTemporaryInput);
    }
}

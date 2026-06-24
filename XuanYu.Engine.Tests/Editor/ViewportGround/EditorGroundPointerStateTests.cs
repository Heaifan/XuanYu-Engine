using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.ViewportGround;

namespace FluidWarfare.Tests.Editor.ViewportGround;

public sealed class EditorGroundPointerStateTests
{
    [Fact]
    public void SetHover_SamePosition_ReturnsNoChange()
    {
        var state = new EditorGroundPointerState();
        var result1 = state.SetHover(new Vector3d(1, 0, 2), "鼠标");
        Assert.True(result1.IsChanged);

        var result2 = state.SetHover(new Vector3d(1, 0, 2), "鼠标");
        Assert.False(result2.IsChanged);
    }

    [Fact]
    public void SetHover_DifferentPosition_ReturnsChanged()
    {
        var state = new EditorGroundPointerState();
        state.SetHover(new Vector3d(1, 0, 2), "鼠标");

        var result = state.SetHover(new Vector3d(3, 0, 4), "鼠标");
        Assert.True(result.IsChanged);
    }

    [Fact]
    public void Commit_SamePosition_ReturnsNoChange()
    {
        var state = new EditorGroundPointerState();
        var r1 = state.Commit(new Vector3d(1, 0, 2));
        Assert.True(r1.IsChanged);
        Assert.True(r1.IsCommit);

        var r2 = state.Commit(new Vector3d(1, 0, 2));
        Assert.False(r2.IsChanged);
    }

    [Fact]
    public void Commit_DifferentPosition_IncrementsRevision()
    {
        var state = new EditorGroundPointerState();
        Assert.Equal(0, state.Revision);

        state.Commit(new Vector3d(1, 0, 2));
        Assert.Equal(1, state.Revision);

        state.Commit(new Vector3d(3, 0, 4));
        Assert.Equal(2, state.Revision);
    }

    [Fact]
    public void ClearCommit_WhenCommitted_ReturnsChanged()
    {
        var state = new EditorGroundPointerState();
        state.Commit(new Vector3d(1, 0, 2));

        var result = state.ClearCommit();
        Assert.True(result.IsChanged);
        Assert.True(result.IsCommit);
        Assert.NotNull(result.PreviousPosition);
        Assert.Null(result.CurrentPosition);
        Assert.Null(state.CommittedHit);
    }

    [Fact]
    public void ClearCommit_WhenAlreadyNull_ReturnsNoChange()
    {
        var state = new EditorGroundPointerState();
        var result = state.ClearCommit();
        Assert.False(result.IsChanged);
    }

    [Fact]
    public void HoverAndCommit_AreIndependent()
    {
        var state = new EditorGroundPointerState();

        state.SetHover(new Vector3d(1, 0, 2), "鼠标");
        Assert.Equal(new Vector3d(1, 0, 2), state.HoverHit);
        Assert.Null(state.CommittedHit);

        state.Commit(new Vector3d(3, 0, 4));
        Assert.Equal(new Vector3d(3, 0, 4), state.CommittedHit);
        // Hover still has previous value
        Assert.Equal(new Vector3d(1, 0, 2), state.HoverHit);
    }
}

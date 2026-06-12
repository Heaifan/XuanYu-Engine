using FluidWarfare.Core.Math;
using FluidWarfare.Render.Vulkan.Scene3D.GroundCursor;

namespace FluidWarfare.Tests.Render.Vulkan.Scene3D.GroundCursor;

public sealed class VulkanGroundCursorStateTests
{
    [Fact]
    public void SetSameGroundPoint_ReturnsNoChange()
    {
        var state = new VulkanGroundCursorState();
        Assert.True(state.Set(new Vector3d(1, 0, 2))); // first set = changed
        Assert.False(state.Set(new Vector3d(1, 0, 2))); // same point = NoOp
    }

    [Fact]
    public void SetDifferentGroundPoint_IncrementsRevision()
    {
        var state = new VulkanGroundCursorState();
        Assert.Equal(0, state.Revision);

        state.Set(new Vector3d(1, 0, 2));
        Assert.Equal(1, state.Revision);

        state.Set(new Vector3d(3, 0, 4));
        Assert.Equal(2, state.Revision);
    }

    [Fact]
    public void HideVisibleCursor_IncrementsRevision()
    {
        var state = new VulkanGroundCursorState();
        state.Set(new Vector3d(1, 0, 2));
        Assert.Equal(1, state.Revision);

        Assert.True(state.Hide());
        Assert.Equal(2, state.Revision);
        Assert.False(state.IsVisible);
    }

    [Fact]
    public void HideAlreadyHiddenCursor_ReturnsNoChange()
    {
        var state = new VulkanGroundCursorState();
        Assert.False(state.Hide()); // already hidden
    }

    [Fact]
    public void SetNull_ClearsVisibility()
    {
        var state = new VulkanGroundCursorState();
        state.Set(new Vector3d(1, 0, 2));
        Assert.True(state.IsVisible);

        Assert.True(state.Set(null));
        Assert.False(state.IsVisible);
        Assert.Null(state.WorldPosition);
    }

    [Fact]
    public void SetNullWhenAlreadyHidden_ReturnsNoChange()
    {
        var state = new VulkanGroundCursorState();
        Assert.False(state.Set(null)); // already hidden
    }

    [Fact]
    public void ShowAfterHide_IncrementsRevisionAgain()
    {
        var state = new VulkanGroundCursorState();

        state.Set(new Vector3d(1, 0, 2)); // rev 1
        state.Hide();                       // rev 2

        Assert.True(state.Set(new Vector3d(1, 0, 2))); // rev 3 (show same point again)
        Assert.Equal(3, state.Revision);
        Assert.True(state.IsVisible);
    }

    [Fact]
    public void IsVisible_ReflectsCurrentState()
    {
        var state = new VulkanGroundCursorState();
        Assert.False(state.IsVisible);

        state.Set(new Vector3d(0, 0, 0));
        Assert.True(state.IsVisible);

        state.Hide();
        Assert.False(state.IsVisible);
    }
}

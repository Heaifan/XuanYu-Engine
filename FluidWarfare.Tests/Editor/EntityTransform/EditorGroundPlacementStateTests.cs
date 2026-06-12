using FluidWarfare.Editor.EntityTransform;

namespace FluidWarfare.Tests.Editor.EntityTransform;

public sealed class EditorGroundPlacementStateTests
{
    [Fact]
    public void Begin_WithEntity_EntersPlacementMode()
    {
        var state = new EditorGroundPlacementState();
        Assert.True(state.Begin("entity:1"));
        Assert.True(state.IsActive);
        Assert.Equal("entity:1", state.TargetEntityId);
    }

    [Fact]
    public void Begin_SameEntity_Twice_ReturnsNoChange()
    {
        var state = new EditorGroundPlacementState();
        state.Begin("entity:1");
        Assert.False(state.Begin("entity:1"));
    }

    [Fact]
    public void Begin_DifferentEntity_ReturnsChanged()
    {
        var state = new EditorGroundPlacementState();
        state.Begin("entity:1");
        Assert.True(state.Begin("entity:2"));
        Assert.Equal("entity:2", state.TargetEntityId);
    }

    [Fact]
    public void Complete_ExitsPlacementMode()
    {
        var state = new EditorGroundPlacementState();
        state.Begin("entity:1");
        state.Complete();
        Assert.False(state.IsActive);
        Assert.Null(state.TargetEntityId);
    }

    [Fact]
    public void Cancel_ExitsPlacementMode()
    {
        var state = new EditorGroundPlacementState();
        state.Begin("entity:1");
        state.Cancel();
        Assert.False(state.IsActive);
        Assert.Null(state.TargetEntityId);
    }

    [Fact]
    public void Revision_IncrementsOnBegin()
    {
        var state = new EditorGroundPlacementState();
        Assert.Equal(0, state.Revision);
        state.Begin("entity:1");
        Assert.Equal(1, state.Revision);
        state.Complete();
        Assert.Equal(2, state.Revision);
    }

    [Fact]
    public void WithoutEntity_IsNotActive()
    {
        var state = new EditorGroundPlacementState();
        Assert.False(state.IsActive);
        Assert.Null(state.TargetEntityId);
    }
}

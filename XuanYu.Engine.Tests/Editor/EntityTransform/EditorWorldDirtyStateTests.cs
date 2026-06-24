using XuanYu.Engine.Editor.EntityTransform;

namespace FluidWarfare.Tests.Editor.EntityTransform;

public sealed class EditorWorldDirtyStateTests
{
    [Fact]
    public void Initially_IsNotDirty()
    {
        var state = new EditorWorldDirtyState();
        Assert.False(state.IsDirty);
        Assert.Equal(0, state.Revision);
        Assert.Null(state.LastChangedEntityId);
    }

    [Fact]
    public void MarkDirty_SetsIsDirty()
    {
        var state = new EditorWorldDirtyState();
        state.MarkDirty("entity:1");
        Assert.True(state.IsDirty);
    }

    [Fact]
    public void MarkDirty_UpdatesLastChangedEntityId()
    {
        var state = new EditorWorldDirtyState();
        state.MarkDirty("entity:1");
        Assert.Equal("entity:1", state.LastChangedEntityId);
    }

    [Fact]
    public void MarkDirty_IncrementsRevision()
    {
        var state = new EditorWorldDirtyState();
        state.MarkDirty("entity:1");
        Assert.Equal(1, state.Revision);
        state.MarkDirty("entity:2");
        Assert.Equal(2, state.Revision);
    }

    [Fact]
    public void Reset_ClearsDirty()
    {
        var state = new EditorWorldDirtyState();
        state.MarkDirty("entity:1");
        Assert.True(state.IsDirty);
        state.Reset();
        Assert.False(state.IsDirty);
        Assert.Null(state.LastChangedEntityId);
    }

    [Fact]
    public void Reset_IncrementsRevision()
    {
        var state = new EditorWorldDirtyState();
        state.MarkDirty("entity:1");
        var rev = state.Revision;
        state.Reset();
        Assert.True(state.Revision > rev);
    }
}

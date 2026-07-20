using XuanYu.Core.History;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;

namespace XuanYu.Core.Tests.History;

public sealed class EditorHistoryOwnerTests
{
    [Fact]
    public void Empty_undo_is_noop()
    {
        var history = new EditorHistoryOwner();

        Assert.False(history.TryUndo(out _));
        Assert.Equal(0, history.Count);
    }

    [Fact]
    public void Push_ignores_unchanged_transform()
    {
        var history = new EditorHistoryOwner();
        var transform = CommittedTransform.Identity;

        history.Push(new TransformHistoryEntry(EntityId.FromInt(1), transform, transform));

        Assert.Equal(0, history.Count);
    }

    [Fact]
    public void Undo_is_lifo()
    {
        var history = new EditorHistoryOwner();
        var entity = EntityId.FromInt(1);

        history.Push(Entry(entity, 0, 1));
        history.Push(Entry(entity, 1, 2));
        history.Push(Entry(entity, 2, 3));

        Assert.True(history.TryUndo(out var last));
        Assert.Equal(new Vector3d(2, 0, 0), last.Before.Position);
        Assert.True(history.TryUndo(out var middle));
        Assert.Equal(new Vector3d(1, 0, 0), middle.Before.Position);
        Assert.True(history.TryUndo(out var first));
        Assert.Equal(Vector3d.Zero, first.Before.Position);
        Assert.False(history.TryUndo(out _));
    }

    static TransformHistoryEntry Entry(EntityId entity, double before, double after) =>
        new(entity, new CommittedTransform(new Vector3d(before, 0, 0)),
            new CommittedTransform(new Vector3d(after, 0, 0)));
}

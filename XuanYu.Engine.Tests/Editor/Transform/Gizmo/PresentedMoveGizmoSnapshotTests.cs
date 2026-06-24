using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;

namespace FluidWarfare.Tests.Editor.Transform.Gizmo;

/// <summary>
/// PresentedMoveGizmoSnapshot 的有效性追踪测试。
/// </summary>
public sealed class PresentedMoveGizmoSnapshotTests
{
    private static MoveGizmoLayout CreateDummyLayout() =>
        MoveGizmoLayout.Build(
            (200, 300), (350, 290), (200, 170), (180, 350),
            false, false, false)!;

    [Fact]
    public void EntityId_StoredAndRetrievable()
    {
        var snapshot = new PresentedMoveGizmoSnapshot(
            true, "entity-001", 1, 2, 3, 800, 600, CreateDummyLayout());
        Assert.Equal("entity-001", snapshot.EntityId);
    }

    [Fact]
    public void SelectionRevision_StoredAndRetrievable()
    {
        var snapshot = new PresentedMoveGizmoSnapshot(
            true, "e1", 42, 2, 3, 800, 600, CreateDummyLayout());
        Assert.Equal(42, snapshot.SelectionRevision);
    }

    [Fact]
    public void TransformRevision_StoredAndRetrievable()
    {
        var snapshot = new PresentedMoveGizmoSnapshot(
            true, "e1", 1, 99, 3, 800, 600, CreateDummyLayout());
        Assert.Equal(99, snapshot.TransformRevision);
    }

    [Fact]
    public void CameraRevision_StoredAndRetrievable()
    {
        var snapshot = new PresentedMoveGizmoSnapshot(
            true, "e1", 1, 2, 7, 800, 600, CreateDummyLayout());
        Assert.Equal(7, snapshot.CameraRevision);
    }

    [Fact]
    public void ViewportDimensions_StoredAndRetrievable()
    {
        var snapshot = new PresentedMoveGizmoSnapshot(
            true, "e1", 1, 2, 3, 1920, 1080, CreateDummyLayout());
        Assert.Equal(1920, snapshot.ViewportWidth);
        Assert.Equal(1080, snapshot.ViewportHeight);
    }

    [Fact]
    public void None_IsNotAvailable()
    {
        Assert.False(PresentedMoveGizmoSnapshot.None.IsAvailable);
    }

    [Fact]
    public void None_HasEmptyEntityId()
    {
        Assert.Equal(string.Empty, PresentedMoveGizmoSnapshot.None.EntityId);
    }

    [Fact]
    public void DifferentEntityId_ChangesSnapshot()
    {
        var a = new PresentedMoveGizmoSnapshot(
            true, "entity-a", 1, 2, 3, 800, 600, CreateDummyLayout());
        var b = new PresentedMoveGizmoSnapshot(
            true, "entity-b", 1, 2, 3, 800, 600, CreateDummyLayout());
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void DifferentSelectionRevision_ChangesSnapshot()
    {
        var a = new PresentedMoveGizmoSnapshot(
            true, "e1", 1, 2, 3, 800, 600, CreateDummyLayout());
        var b = new PresentedMoveGizmoSnapshot(
            true, "e1", 2, 2, 3, 800, 600, CreateDummyLayout());
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void DifferentTransformRevision_ChangesSnapshot()
    {
        var a = new PresentedMoveGizmoSnapshot(
            true, "e1", 1, 2, 3, 800, 600, CreateDummyLayout());
        var b = new PresentedMoveGizmoSnapshot(
            true, "e1", 1, 5, 3, 800, 600, CreateDummyLayout());
        Assert.NotEqual(a, b);
    }

    [Fact]
    public void SameFields_AreEqual()
    {
        var layout = CreateDummyLayout();
        var a = new PresentedMoveGizmoSnapshot(
            true, "e1", 1, 2, 3, 800, 600, layout);
        var b = new PresentedMoveGizmoSnapshot(
            true, "e1", 1, 2, 3, 800, 600, layout);
        Assert.Equal(a, b);
    }
}

using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Core.Transform;

namespace XuanYu.Core.Tests.Scene.Render;

public sealed class SceneRenderSnapshotTests
{
    static SceneEntitySnapshot Entity(int id, Vector3d position) =>
        new(EntityId.FromInt(id), $"E{id}", "Test", new CommittedTransform(position));

    [Fact]
    public void EmptyHasNoEntitiesAndIdentityTransform()
    {
        var snapshot = SceneRenderSnapshot.Empty;

        Assert.False(snapshot.HasEntity);
        Assert.Empty(snapshot.Entities);
        Assert.Equal(CommittedTransform.Identity, snapshot.RenderTransform);
        Assert.False(snapshot.IsSelected);
        Assert.False(snapshot.ShowMoveGizmo);
        Assert.False(snapshot.ShowRotateGizmo);
        Assert.False(snapshot.ShowScaleGizmo);
    }

    [Fact]
    public void TestEntityAtOriginHasOneEntity()
    {
        var snapshot = SceneRenderSnapshot.TestEntityAtOrigin;

        Assert.True(snapshot.HasEntity);
        Assert.Equal(1, Assert.Single(snapshot.Entities).EntityKey.Value);
        Assert.Equal(Vector3d.Zero, snapshot.RenderPosition);
    }

    [Fact]
    public void RenderEntitiesOverridePrimaryFallback()
    {
        var primary = Entity(1, Vector3d.Zero);
        var list = new[] { Entity(2, Vector3d.UnitX) };
        var snapshot = new SceneRenderSnapshot(primary, RenderEntities: list);

        Assert.Same(list, snapshot.Entities);
        Assert.Equal(2, Assert.Single(snapshot.Entities).EntityKey.Value);
        Assert.Single(new SceneRenderSnapshot(primary).Entities);
    }

    [Fact]
    public void PreviewAppliesOnlyToPrimaryId()
    {
        var primary = Entity(1, Vector3d.Zero);
        var other = Entity(2, Vector3d.UnitY);
        var moved = new Vector3d(4, 5, 6);
        var snapshot = new SceneRenderSnapshot(primary,
            PreviewTransform: new PreviewTransform(moved));

        Assert.Equal(moved, snapshot.RenderPosition);
        Assert.Equal(moved, snapshot.TransformFor(primary).Position);
        Assert.Equal(moved, snapshot.PositionFor(primary));
        Assert.Equal(other.Transform, snapshot.TransformFor(other));
        Assert.Equal(other.Transform.Position, snapshot.PositionFor(other));
    }

    [Fact]
    public void InvalidPrimaryIdNeverReceivesPreview()
    {
        var invalid = new SceneEntitySnapshot(EntityId.None, "", "",
            CommittedTransform.Identity);
        var snapshot = new SceneRenderSnapshot(invalid,
            PreviewTransform: new PreviewTransform(Vector3d.UnitX));

        Assert.False(snapshot.HasEntity);
        Assert.Equal(CommittedTransform.Identity, snapshot.RenderTransform);
        Assert.Equal(Vector3d.Zero, snapshot.RenderPosition);
        Assert.Equal(CommittedTransform.Identity, snapshot.TransformFor(invalid));
        Assert.Equal(Vector3d.Zero, snapshot.PositionFor(invalid));
    }

    [Fact]
    public void ExplicitCameraOverridesDefault()
    {
        var camera = DefaultEditorCamera.Create(12);
        var snapshot = new SceneRenderSnapshot(Entity(1, Vector3d.Zero),
            Camera: camera, IsSelected: true, ShowMoveGizmo: true,
            ShowRotateGizmo: true, ShowScaleGizmo: true);

        Assert.Equal(camera, snapshot.CameraState);
        Assert.True(snapshot.IsSelected);
        Assert.True(snapshot.ShowMoveGizmo);
        Assert.True(snapshot.ShowRotateGizmo);
        Assert.True(snapshot.ShowScaleGizmo);
        Assert.NotNull(SceneRenderSnapshot.Empty.CameraState);
    }
}

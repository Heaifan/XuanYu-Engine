using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed class EntityTests
{
    [Fact]
    public void Blank_scene_is_empty_and_add_cube_has_stable_defaults()
    {
        var scene = new SceneStateOwner(null, false);
        Assert.Empty(scene.Entities);

        var cube = scene.AddCubeEntity();

        Assert.Equal(EntityId.FromInt(1), cube.EntityKey);
        Assert.Equal("立方体", cube.Name);
        Assert.Equal(WorldEntityTypes.Cube, cube.Type);
        Assert.Equal(CommittedTransform.Identity, cube.Transform);
        Assert.Equal(EntityId.None, cube.ParentId);
        Assert.Equal(0, cube.SiblingOrder);
    }

    [Fact]
    public void Cube_names_are_unique_and_reuse_smallest_gap()
    {
        var scene = new SceneStateOwner(null, false);
        var first = scene.AddCubeEntity();
        var middle = scene.AddCubeEntity();
        var last = scene.AddCubeEntity();
        Assert.Equal(["立方体", "立方体 001", "立方体 002"],
            scene.Entities.Select(x => x.Name));

        Assert.True(scene.DestroyEntity(middle.EntityKey));
        var replacement = scene.AddCubeEntity();

        Assert.Equal("立方体 001", replacement.Name);
        Assert.NotEqual(last.EntityKey, replacement.EntityKey);
        Assert.Equal(3, replacement.SiblingOrder);
        Assert.Equal(first.EntityKey, scene.Entities[0].EntityKey);
    }

    [Fact]
    public void Rename_trims_and_resolves_duplicate_without_changing_identity()
    {
        var scene = new SceneStateOwner(null, false);
        var first = scene.AddCubeEntity();
        var second = scene.AddCubeEntity();
        Assert.True(scene.RenameEntity(second.EntityKey, "临时方块", out _));

        Assert.True(scene.RenameEntity(second.EntityKey, "  立方体  ", out var finalName));

        Assert.Equal("立方体 001", finalName);
        Assert.Equal(second.EntityKey, scene.Entities.Single(x => x.Name == finalName).EntityKey);
        Assert.False(scene.RenameEntity(first.EntityKey, "   ", out _));
    }

    [Fact]
    public void Snapshot_restore_preserves_all_identity_fields_and_is_atomic()
    {
        var scene = new SceneStateOwner(null, false);
        var cube = scene.AddCubeEntity();
        var moved = new CommittedTransform(new Vector3d(1, 2, 3), new Vector3d(4, 5, 6), new(2, 2, 2));
        scene.CommitTransformWithResult(cube.EntityKey, moved);
        var snapshot = scene.Entities.Single();
        Assert.True(scene.DestroyEntity(cube.EntityKey));

        Assert.True(scene.RestoreEntity(snapshot));
        Assert.Equal(snapshot, scene.Entities.Single());
        Assert.False(scene.RestoreEntity(snapshot));
        Assert.Equal(snapshot, scene.Entities.Single());
    }
}

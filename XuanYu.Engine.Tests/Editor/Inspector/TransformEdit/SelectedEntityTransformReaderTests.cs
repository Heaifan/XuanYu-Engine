using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Windows.Inspector.TransformEdit;
using XuanYu.Engine.World;

namespace XuanYu.Engine.Tests.Editor.Inspector.TransformEdit;

public sealed class SelectedEntityTransformReaderTests
{
    [Fact]
    public void Read_EntityWithFullTransform_ReturnsAllFields()
    {
        var world = new WorldState();
        var id = world.CreateEntity("测试实体", new Vector3d(10, 20, 30),
            new Vector3d(0, 45, 90), new Vector3d(2, 2, 2));

        var snap = SelectedEntityTransformReader.Read(id, world);

        Assert.NotNull(snap);
        Assert.Equal(10, snap.Position.X);
        Assert.Equal(20, snap.Position.Y);
        Assert.Equal(30, snap.Position.Z);
        Assert.Equal(0, snap.RotationDegrees.X);
        Assert.Equal(45, snap.RotationDegrees.Y);
        Assert.Equal(90, snap.RotationDegrees.Z);
        Assert.Equal(2, snap.Scale.X);
        Assert.Equal(2, snap.Scale.Y);
        Assert.Equal(2, snap.Scale.Z);
    }

    [Fact]
    public void Read_EntityWithDefaultRotationScale_ReturnsDefaults()
    {
        var world = new WorldState();
        var id = world.CreateEntity("测试实体", Vector3d.Zero);

        var snap = SelectedEntityTransformReader.Read(id, world);

        Assert.NotNull(snap);
        Assert.Equal(0, snap.RotationDegrees.X);
        Assert.Equal(0, snap.RotationDegrees.Y);
        Assert.Equal(0, snap.RotationDegrees.Z);
        Assert.Equal(1, snap.Scale.X);
        Assert.Equal(1, snap.Scale.Y);
        Assert.Equal(1, snap.Scale.Z);
    }

    [Fact]
    public void Read_EntityNotFound_ReturnsNull()
    {
        var world = new WorldState();
        var snap = SelectedEntityTransformReader.Read(EntityId.FromInt(999), world);
        Assert.Null(snap);
    }
}

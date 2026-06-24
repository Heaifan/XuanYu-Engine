using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.EntityTransform;
using XuanYu.Engine.Editor.Windows.Inspector.TransformEdit;
using XuanYu.Engine.World;

namespace XuanYu.Engine.Tests.Editor.Inspector.TransformEdit;

public sealed class SelectedEntityTransformApplyTests
{
    [Fact]
    public void Apply_PositionUpdate_Succeeds()
    {
        var (world, id, dirty) = Setup();
        var req = new TransformEditRequest(id, new Vector3d(100, 200, 300), Vector3d.Zero, new Vector3d(1, 1, 1));
        var r = SelectedEntityTransformApply.Apply(req, world, dirty, null, null);

        Assert.True(r.Succeeded);
        Assert.Equal(new Vector3d(100, 200, 300), world.FindPosition(EntityId.FromInt(1))!.Value.Value);
    }

    [Fact]
    public void Apply_RotationUpdate_Succeeds()
    {
        var (world, id, dirty) = Setup();
        var req = new TransformEditRequest(id, Vector3d.Zero, new Vector3d(0, 90, 180), new Vector3d(1, 1, 1));
        var r = SelectedEntityTransformApply.Apply(req, world, dirty, null, null);

        Assert.True(r.Succeeded);
        Assert.Equal(new Vector3d(0, 90, 180), world.FindRotation(EntityId.FromInt(1))!.Value.Value);
    }

    [Fact]
    public void Apply_ScaleUpdate_Succeeds()
    {
        var (world, id, dirty) = Setup();
        var req = new TransformEditRequest(id, Vector3d.Zero, Vector3d.Zero, new Vector3d(2.5, 1.5, 0.5));
        var r = SelectedEntityTransformApply.Apply(req, world, dirty, null, null);

        Assert.True(r.Succeeded);
        Assert.Equal(new Vector3d(2.5, 1.5, 0.5), world.FindScale(EntityId.FromInt(1))!.Value.Value);
    }

    [Fact]
    public void Apply_ScaleZero_Fails()
    {
        var (world, id, dirty) = Setup();
        var req = new TransformEditRequest(id, Vector3d.Zero, Vector3d.Zero, new Vector3d(0, 1, 1));
        var r = SelectedEntityTransformApply.Apply(req, world, dirty, null, null);

        Assert.False(r.Succeeded);
        Assert.Contains("大于 0", r.Message);
    }

    [Fact]
    public void Apply_ScaleNegative_Fails()
    {
        var (world, id, dirty) = Setup();
        var req = new TransformEditRequest(id, Vector3d.Zero, Vector3d.Zero, new Vector3d(-1, 1, 1));
        var r = SelectedEntityTransformApply.Apply(req, world, dirty, null, null);

        Assert.False(r.Succeeded);
        Assert.Contains("大于 0", r.Message);
    }

    [Fact]
    public void Apply_ScaleNaN_Fails()
    {
        var (world, id, dirty) = Setup();
        var req = new TransformEditRequest(id, Vector3d.Zero, Vector3d.Zero, new Vector3d(double.NaN, 1, 1));
        var r = SelectedEntityTransformApply.Apply(req, world, dirty, null, null);

        Assert.False(r.Succeeded);
        Assert.Contains("无效", r.Message);
    }

    [Fact]
    public void Apply_PositionNaN_Fails()
    {
        var (world, id, dirty) = Setup();
        var req = new TransformEditRequest(id, new Vector3d(double.NaN, 0, 0), Vector3d.Zero, new Vector3d(1, 1, 1));
        var r = SelectedEntityTransformApply.Apply(req, world, dirty, null, null);

        Assert.False(r.Succeeded);
        Assert.Contains("无效", r.Message);
    }

    [Fact]
    public void Apply_RotationInfinity_Fails()
    {
        var (world, id, dirty) = Setup();
        var req = new TransformEditRequest(id, Vector3d.Zero, new Vector3d(double.PositiveInfinity, 0, 0), new Vector3d(1, 1, 1));
        var r = SelectedEntityTransformApply.Apply(req, world, dirty, null, null);

        Assert.False(r.Succeeded);
        Assert.Contains("无效", r.Message);
    }

    [Fact]
    public void Apply_EntityNotFound_Fails()
    {
        var world = new WorldState();
        world.CreateEntity("other", Vector3d.Zero);
        var dirty = new EditorWorldDirtyState();
        var req = new TransformEditRequest("999", Vector3d.Zero, Vector3d.Zero, new Vector3d(1, 1, 1));
        var r = SelectedEntityTransformApply.Apply(req, world, dirty, null, null);

        Assert.False(r.Succeeded);
        Assert.Contains("不存在", r.Message);
    }

    [Fact]
    public void Apply_EmptyEntityId_Fails()
    {
        var world = new WorldState();
        var dirty = new EditorWorldDirtyState();
        var req = new TransformEditRequest("", Vector3d.Zero, Vector3d.Zero, new Vector3d(1, 1, 1));
        var r = SelectedEntityTransformApply.Apply(req, world, dirty, null, null);

        Assert.False(r.Succeeded);
        Assert.Contains("未选中", r.Message);
    }

    [Fact]
    public void Apply_MarksDirty()
    {
        var (world, id, dirty) = Setup();
        var req = new TransformEditRequest(id, new Vector3d(1, 2, 3), Vector3d.Zero, new Vector3d(1, 1, 1));
        SelectedEntityTransformApply.Apply(req, world, dirty, null, null);

        Assert.True(dirty.IsDirty);
    }

    // ── 帮助方法 ──────────────────────────────────────
    static (WorldState world, string id, EditorWorldDirtyState dirty) Setup()
    {
        var world = new WorldState();
        var eid = world.CreateEntity("测试", Vector3d.Zero);
        return (world, eid.Value.ToString(), new EditorWorldDirtyState());
    }
}

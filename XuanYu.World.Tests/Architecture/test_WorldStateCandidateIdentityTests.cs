using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.World;

namespace XuanYu.World.Tests.Architecture;

public sealed class WorldStateCandidateIdentityTests
{
    [Fact]
    public void Duplicate_entity_ids_are_rejected()
    {
        var entity = Create(EntityId.FromInt(1));
        AssertInvalid([entity, entity]);
    }

    [Fact]
    public void Default_snapshot_is_rejected()
    {
        AssertInvalid([default(WorldEntitySnapshot)]);
    }

    [Fact]
    public void Snapshot_constructor_rejects_blank_required_fields()
    {
        var id = EntityId.FromInt(1);
        Assert.Throws<ArgumentException>(() => new WorldEntitySnapshot(
            id, " ", "Custom", CommittedTransform.Identity, Vector3d.Zero,
            RegionKey.Origin, WorldEntityActivity.Active, default));
        Assert.Throws<ArgumentException>(() => new WorldEntitySnapshot(
            id, "Entity", " ", CommittedTransform.Identity, Vector3d.Zero,
            RegionKey.Origin, WorldEntityActivity.Active, default));
    }

    [Fact]
    public void Undefined_activity_is_rejected()
    {
        var entity = Create(EntityId.FromInt(1), activity: (WorldEntityActivity)99);
        AssertInvalid([entity]);
    }

    [Fact]
    public void Invalid_transform_or_spatial_bounds_are_rejected()
    {
        var id = EntityId.FromInt(1);
        var invalidTransform = new WorldEntitySnapshot(
            id, "Entity", "Custom", default, Vector3d.Zero,
            RegionKey.Origin, WorldEntityActivity.Active, default);
        var invalidBounds = Create(id, globalPosition: new Vector3d(double.NaN, 0, 0));

        AssertInvalid([invalidTransform]);
        AssertInvalid([invalidBounds]);
    }

    [Fact]
    public void Externalized_and_distinct_world_position_are_preserved()
    {
        var entity = new WorldEntitySnapshot(
            EntityId.FromInt(1), "External", "Custom", CommittedTransform.Identity,
            new Vector3d(20, 0, 0), RegionKey.FromGrid(7, 8),
            WorldEntityActivity.Externalized, default);

        var candidate = WorldStateCandidate.Create([entity]);

        Assert.Equal(entity, candidate.Entities[0]);
    }

    static WorldEntitySnapshot Create(
        EntityId id,
        string name = "Entity",
        string type = "Custom",
        WorldEntityActivity activity = WorldEntityActivity.Active,
        Vector3d? globalPosition = null) =>
        new(id, name, type, CommittedTransform.Identity,
            globalPosition ?? Vector3d.Zero, RegionKey.Origin, activity, default);

    static void AssertInvalid(IReadOnlyList<WorldEntitySnapshot> entities)
    {
        var error = Assert.Throws<ArgumentException>(
            () => WorldStateCandidate.Create(entities));
        Assert.False(string.IsNullOrWhiteSpace(error.Message));
    }
}

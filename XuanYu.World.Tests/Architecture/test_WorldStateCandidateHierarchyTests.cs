using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.World;

namespace XuanYu.World.Tests.Architecture;

public sealed class WorldStateCandidateHierarchyTests
{
    [Fact]
    public void Missing_parent_is_rejected()
    {
        var child = Create(2, EntityId.FromInt(1), 0);
        AssertInvalid([child]);
    }

    [Fact]
    public void Parent_cycle_is_rejected()
    {
        var first = Create(1, EntityId.FromInt(2), 0);
        var second = Create(2, EntityId.FromInt(1), 0);

        AssertInvalid([first, second]);
    }

    [Fact]
    public void Sibling_order_conflict_is_rejected()
    {
        var parent = Create(1, default, 0);
        var first = Create(2, EntityId.FromInt(1), 4);
        var second = Create(3, EntityId.FromInt(1), 4);

        AssertInvalid([parent, first, second]);
    }

    [Fact]
    public void Valid_entities_may_arrive_in_parent_child_reverse_order()
    {
        var parent = Create(1, default, 0);
        var child = Create(2, EntityId.FromInt(1), -3);

        var candidate = WorldStateCandidate.Create([child, parent]);

        Assert.Equal(child.EntityKey, candidate.Entities[0].EntityKey);
        Assert.Equal(parent.EntityKey, candidate.Entities[1].EntityKey);
    }

    static WorldEntitySnapshot Create(int id, EntityId parent, int siblingOrder) =>
        new(EntityId.FromInt(id), $"Entity {id}", "Custom",
            CommittedTransform.Identity, Vector3d.Zero, RegionKey.Origin,
            WorldEntityActivity.Active, default, parent, siblingOrder);

    static void AssertInvalid(IReadOnlyList<WorldEntitySnapshot> entities) =>
        Assert.Throws<ArgumentException>(() => WorldStateCandidate.Create(entities));
}

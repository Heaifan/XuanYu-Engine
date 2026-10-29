using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.Architecture;

public sealed class WorldStateCandidateIsolationTests
{
    [Fact]
    public void Candidate_owns_a_copy_of_the_input_collection()
    {
        var original = Create(1, "Before");
        var input = new List<WorldEntitySnapshot> { original };
        var candidate = WorldStateCandidate.Create(input);

        input[0] = Create(2, "After");

        Assert.Equal(original, candidate.Entities[0]);
        var exposed = Assert.IsAssignableFrom<IList<WorldEntitySnapshot>>(
            candidate.Entities);
        Assert.Throws<NotSupportedException>(() => exposed[0] = input[0]);
    }

    [Fact]
    public void Candidate_validation_failure_does_not_touch_world_or_publish()
    {
        var world = new GlobalWorld();
        var existing = world.Create("Existing");
        var beforeEntities = world.Entities.ToArray();
        var beforePartition = world.PartitionSnapshot.ToArray();
        var scene = new SceneStateOwner();
        var ownerRevision = ((IWorldTransformMutationContract)scene).WorldRevision;
        var publications = 0;
        scene.RenderSnapshotChanged += _ => publications++;

        Assert.Throws<ArgumentException>(
            () => WorldStateCandidate.Create([existing, existing]));

        Assert.Equal(beforeEntities, world.Entities);
        Assert.Equal(beforePartition, world.PartitionSnapshot);
        Assert.Equal(ownerRevision, ((IWorldTransformMutationContract)scene).WorldRevision);
        Assert.Equal(0, publications);
    }

    static WorldEntitySnapshot Create(int id, string name) =>
        new(EntityId.FromInt(id), name, "Custom", CommittedTransform.Identity,
            Vector3d.Zero, RegionKey.Origin, WorldEntityActivity.Active, default);
}

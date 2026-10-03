using XuanYu.Core.Gizmo;
using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Core.Transform;
using XuanYu.Editor.Transform;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.Transform;

public sealed class EditorTransformBoundaryTests
{
    [Fact]
    public void Session_creates_command_without_world_concrete_dependency()
    {
        var entity = new SceneEntitySnapshot(
            EntityId.FromInt(7), "Entity", "Type", CommittedTransform.Identity);
        var session = new TransformSession();

        Assert.True(session.Begin(11, entity, MoveGizmoAxis.X));
        Assert.True(session.TryPreview(11, new(4, 0, 0)));
        Assert.True(session.TryCreateCommand(11, out var command));
        Assert.Equal(entity.EntityKey, command.EntityKey);
        Assert.Equal(new(4, 0, 0), command.Transform.Position);
        Assert.False(session.IsActive);
    }

    [Fact]
    public void Command_routes_mutation_through_world_owned_contract()
    {
        var world = new SpyWorld();
        var command = new TransformCommand(
            11, EntityId.FromInt(7), CommittedTransform.Identity.WithPosition(new(4, 0, 0)));

        Assert.True(command.TryApply(world, out var result));
        Assert.Equal(command.EntityKey, world.EntityKey);
        Assert.Equal(command.Transform, world.Transform);
        Assert.True(result.Changed);
    }

    sealed class SpyWorld : IWorldTransformMutationContract
    {
        public long WorldRevision => 1;
        public long GetEntityMutationRevision(EntityId entityKey) => 1;
        public EntityId EntityKey { get; private set; }
        public CommittedTransform Transform { get; private set; }

        public bool TryCommitTransform(
            EntityId entityKey,
            long expectedWorldRevision,
            CommittedTransform transform,
            out SceneTransformCommitResult result)
        {
            EntityKey = entityKey;
            Transform = transform;
            result = new(entityKey, CommittedTransform.Identity, transform, true);
            return true;
        }
    }
}

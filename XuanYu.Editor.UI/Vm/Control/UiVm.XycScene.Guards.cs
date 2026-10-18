using System.Globalization;
using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Editor.Transform;
using XuanYu.Editor.UI.XycControl;
using XuanYu.World;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool TryXycTransform(EntityId id, XycControl.Vector3 position)
    {
        if (!_sceneState.TryGetEntity(id, out var entity) || !IdleXycInteraction()) return false;
        var next = entity.Transform.WithPosition(new(position.X, position.Y, position.Z));
        var command = new TransformCommand(0, id, next, _worldMutation.WorldRevision);
        SceneTransformCommitResult commit;
        try
        {
            if (!command.TryApply(_worldMutation, out commit) || !commit.Changed) return false;
        }
        catch
        {
            // World mutation precedes snapshot notifications. If a notification throws
            // after the requested transform is already authoritative, finish recording
            // the matching history entry instead of exposing a half-committed XYC step.
            if (!_sceneState.TryGetEntity(id, out var committed) || committed.Transform != next)
                throw;
            commit = new SceneTransformCommitResult(id, entity.Transform, committed.Transform, true);
        }
        RecordTransformHistory(commit);
        PublishSceneRenderSnapshot();
        return true;
    }

    void ValidateXycBaseline(string sceneId, long revision)
    {
        if (sceneId != SceneId) throw new XycSceneOperationException("SCENE_MISMATCH");
        if (revision != SceneRevision) throw new XycSceneOperationException("STALE_REVISION");
        if (!string.IsNullOrEmpty(CurrentScenePath) || IsSceneDirty || _sceneState.Entities.Count != 0 ||
            _historyOwner.Count != 0 || _historyOwner.RedoCount != 0 || IsRegionEditMode ||
            IsDrawingTransactionActive || !IdleXycInteraction())
            throw new XycSceneOperationException("UNSAFE_SCENE");
    }

    void ValidateXycTarget(string sceneId, long revision, string id, int stage, int undo, int redo)
    {
        if (sceneId != SceneId) throw new XycSceneOperationException("SCENE_MISMATCH");
        if (revision != SceneRevision) throw new XycSceneOperationException("STALE_REVISION");
        if (_xycTestStage != stage || id != _xycTestEntityId)
            throw new XycSceneOperationException("UNAUTHORIZED_ENTITY");
        if (_historyOwner.Count != undo || _historyOwner.RedoCount != redo ||
            IsRegionEditMode || IsDrawingTransactionActive || !IdleXycInteraction())
            throw new XycSceneOperationException("HISTORY_CONFLICT");
    }

    bool AtTransform(string id, CommittedTransform? transform, int undo, int redo) =>
        _sceneState.Entities.FirstOrDefault(e => e.EntityKey.ToString() == id) is var entity &&
        entity.EntityKey.IsValid && transform is { } expected && entity.Transform == expected &&
        _historyOwner.Count == undo && _historyOwner.RedoCount == redo && IdleXycInteraction();

    XycSceneState XycState() => new(SceneId, SceneRevision, _sceneState.Entities.Count,
        _sceneState.Entities.Select(x => x.EntityKey.ToString()).ToArray(),
        BuildHierarchyItems().Where(x => x.IsEntity).Select(x => x.Key).ToArray(),
        RenderSnapshot.Entities.Select(x => x.EntityKey.ToString()).ToArray(),
        InspectorFields.Select(x => new XycInspectorField(x.Label, x.Value)).ToArray(), CurrentScenePath,
        !IdleXycInteraction(), _historyOwner.Count, _historyOwner.RedoCount,
        _xycTestEntityId, _xycTestStage.ToString(CultureInfo.InvariantCulture));

    static XycEntity ToXycEntity(WorldEntitySnapshot entity) => new(entity.EntityKey.ToString(),
        entity.Type, entity.Name, new(new(entity.Transform.Position.X, entity.Transform.Position.Y,
            entity.Transform.Position.Z), new(entity.Transform.Rotation.X, entity.Transform.Rotation.Y,
            entity.Transform.Rotation.Z), new(entity.Transform.Scale.X, entity.Transform.Scale.Y,
            entity.Transform.Scale.Z)));

    static EntityId ParseXycEntityId(string value)
    {
        if (!value.StartsWith("EntityId(", StringComparison.Ordinal) || !value.EndsWith(')') ||
            !int.TryParse(value.AsSpan(9, value.Length - 10), NumberStyles.None,
                CultureInfo.InvariantCulture, out var id))
            throw new XycSceneOperationException("INVALID_ENTITY_ID");
        try { return EntityId.FromInt(id); }
        catch (ArgumentOutOfRangeException) { throw new XycSceneOperationException("INVALID_ENTITY_ID"); }
    }

    bool IdleXycInteraction() => _editorState.InteractionSnapshot.Phase == EditorInteractionPhase.Idle;
    static bool FiniteBounded(double value) => double.IsFinite(value) && Math.Abs(value) <= 10_000;
}

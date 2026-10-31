using System.Globalization;
using XuanYu.Core.Identity;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    XycSceneState XycState()
    {
        var selected = TrySelectedEntityKey(out var selectedId) && _sceneState.TryGetEntity(selectedId, out _)
            ? selectedId.ToString() : null;
        var fields = selected is null ? Array.Empty<XycInspectorField>() :
            InspectorFields.Select(x => new XycInspectorField(x.Label, x.Value)).ToArray();
        return new(SceneId, SceneRevision, _sceneState.Entities.Count,
            _sceneState.Entities.Select(x => x.EntityKey.ToString()).ToArray(),
            BuildHierarchyItems().Where(x => x.IsEntity).Select(x => x.Key).ToArray(),
            RenderSnapshot.Entities.Select(x => x.EntityKey.ToString()).ToArray(), fields,
            CurrentScenePath, !IdleXycInteraction(), _historyOwner.Count, _historyOwner.RedoCount,
            _xycTestEntityId, _xycTestStage.ToString(CultureInfo.InvariantCulture),
            _xycSessionEntityOrder.Select(x => x.ToString()).ToArray(), selected,
            XycTop(undo: true), XycTop(undo: false), _xycCommitStatus, _xycRecoveryReason);
    }

    XycHistorySnapshot? XycTop(bool undo)
    {
        var found = undo ? _historyOwner.TryPeekUndoAny(out var top) : _historyOwner.TryPeekRedoAny(out top);
        if (!found) return null;
        var known = TryDescribeXycEntry(top.Entry, out var operation, out var id);
        return new(known ? operation : "UNKNOWN", known ? id.ToString() : null,
            top.BeforeRevision, top.AfterRevision, top.CurrentRevision);
    }

    static XycEntity ToXycEntity(XuanYu.World.WorldEntitySnapshot entity) => new(entity.EntityKey.ToString(),
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
}

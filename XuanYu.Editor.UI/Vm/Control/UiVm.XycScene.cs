using System.Globalization;
using XuanYu.Core.Identity;
using XuanYu.Core.Scene;
using XuanYu.Editor.Transform;
using XuanYu.Editor.UI.XycControl;
using XuanYu.World;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    string? _xycTestEntityId;
    CommittedTransform? _xycTestInitialTransform;
    CommittedTransform? _xycTestTargetTransform;
    int _xycTestStage;

    internal XycSceneState InspectXycTestScene() => XycState();
    internal XycEntity[] ListXycSceneEntities() => _sceneState.Entities.Select(ToXycEntity).ToArray();

    internal XycEntity InspectXycSceneEntity(string id)
    {
        var entityId = ParseXycEntityId(id);
        return _sceneState.TryGetEntity(entityId, out var entity)
            ? ToXycEntity(entity) : throw new XycSceneOperationException("ENTITY_NOT_FOUND");
    }

    internal XycSceneState CreateXycTestCube(string sceneId, long revision)
    {
        ValidateXycBaseline(sceneId, revision);
        if (_xycTestStage != 0) throw new XycSceneOperationException("INVALID_STAGE");
        var before = _sceneState.Entities.Select(x => x.EntityKey).ToHashSet();
        AddCubeEntity();
        var added = _sceneState.Entities.Where(x => !before.Contains(x.EntityKey)).ToArray();
        if (added.Length != 1 || added[0].Type != WorldEntityTypes.Cube)
            throw new XycSceneOperationException("POSTCONDITION_FAILED");
        _xycTestEntityId = added[0].EntityKey.ToString();
        _xycTestInitialTransform = added[0].Transform;
        _xycTestStage = 1;
        return XycState();
    }

    internal XycSceneState SetXycTestPosition(string sceneId, long revision,
        string id, XycControl.Vector3 position)
    {
        ValidateXycTarget(sceneId, revision, id, 1, 1, 0);
        if (!FiniteBounded(position.X) || !FiniteBounded(position.Y) || !FiniteBounded(position.Z))
            throw new XycSceneOperationException("INVALID_INPUT");
        if (!_sceneState.TryGetEntity(ParseXycEntityId(id), out var entity))
            throw new XycSceneOperationException("ENTITY_NOT_FOUND");
        _xycTestTargetTransform = entity.Transform.WithPosition(new(position.X, position.Y, position.Z));
        if (!TryXycTransform(ParseXycEntityId(id), position))
            throw new XycSceneOperationException("TRANSFORM_REJECTED");
        _xycTestStage = 2;
        return XycState();
    }

    internal XycSceneState UndoXycTestOperation(string sceneId, long revision, string id)
    {
        if (_xycTestStage == 2)
        {
            ValidateXycTarget(sceneId, revision, id, 2, 2, 0);
            TryUndoFromShortcut();
            if (!AtTransform(id, _xycTestInitialTransform, 1, 1)) throw new XycSceneOperationException("POSTCONDITION_FAILED");
            _xycTestStage = 3;
        }
        else if (_xycTestStage == 4)
        {
            ValidateXycTarget(sceneId, revision, id, 4, 2, 0);
            TryUndoFromShortcut();
            if (!AtTransform(id, _xycTestInitialTransform, 1, 1)) throw new XycSceneOperationException("POSTCONDITION_FAILED");
            _xycTestStage = 5;
        }
        else if (_xycTestStage == 5)
        {
            ValidateXycTarget(sceneId, revision, id, 5, 1, 1);
            TryUndoFromShortcut();
            if (_sceneState.Entities.Any(x => x.EntityKey.ToString() == id))
                throw new XycSceneOperationException("POSTCONDITION_FAILED");
            _xycTestStage = 6;
            _xycTestEntityId = null;
            _xycTestInitialTransform = null;
            _xycTestTargetTransform = null;
        }
        else throw new XycSceneOperationException("INVALID_STAGE");
        return XycState();
    }

    internal XycSceneState RedoXycTestOperation(string sceneId, long revision, string id)
    {
        if (_xycTestStage != 3) throw new XycSceneOperationException("INVALID_STAGE");
        ValidateXycTarget(sceneId, revision, id, 3, 1, 1);
        TryRedoFromShortcut();
        if (!AtTransform(id, _xycTestTargetTransform, 2, 0)) throw new XycSceneOperationException("POSTCONDITION_FAILED");
        _xycTestStage = 4;
        return XycState();
    }

}

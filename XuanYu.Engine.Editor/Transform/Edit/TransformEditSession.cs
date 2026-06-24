using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Project.World.Transform;

namespace XuanYu.Engine.Editor.Transform.Edit;

/// <summary>
/// Transform 编辑事务状态机。
/// Begin → Preview* → Confirm | Cancel.
/// Cancel 恢复 InitialTransform 和 InitialDirty。
/// </summary>
public sealed class TransformEditSession
{
    private TransformEditSnapshot _snapshot;
    private bool _active;
    private SceneTransform _preview;

    public event Action<TransformEditResult>? Completed;

    public bool IsActive => _active;
    public SceneTransform PreviewTransform => _preview;
    public string EntityId => _snapshot.EntityId;
    public Vector3d InitialPosition => _snapshot.InitialTransform.Position;

    public void Begin(TransformEditSnapshot snapshot)
    {
        _snapshot = snapshot;
        _preview = snapshot.InitialTransform;
        _active = true;
    }

    public void Preview(SceneTransform transform)
    {
        _preview = transform;
    }

    public void Confirm()
    {
        if (!_active) return;
        _active = false;
        Completed?.Invoke(new TransformEditResult(
            _snapshot.Kind, true, false, _preview, _snapshot.InitialDirty));
    }

    public void Cancel()
    {
        if (!_active) return;
        _active = false;
        Completed?.Invoke(new TransformEditResult(
            _snapshot.Kind, false, true, _snapshot.InitialTransform, _snapshot.InitialDirty));
    }

    public void Abort() => Cancel();
}

namespace FluidWarfare.Editor.EntityTransform;

/// <summary>
/// 一次性地面放置模式状态。
/// 选择实体后进入放置模式，点击空白地面完成放置，Esc 取消。
/// </summary>
public sealed class EditorGroundPlacementState
{
    private bool _isActive;
    private string? _targetEntityId;
    private int _revision;

    /// <summary>是否处于放置模式中。</summary>
    public bool IsActive => _isActive;

    /// <summary>放置目标实体 ID。</summary>
    public string? TargetEntityId => _targetEntityId;

    /// <summary>状态修订号。</summary>
    public int Revision => _revision;

    /// <summary>
    /// 进入放置模式。重复进入同一实体为 NoOp。
    /// </summary>
    public bool Begin(string entityId)
    {
        if (_isActive && _targetEntityId == entityId)
            return false; // Already in placement mode for this entity
        _isActive = true;
        _targetEntityId = entityId;
        _revision++;
        return true;
    }

    /// <summary>
    /// 完成放置（成功放置后自动退出模式）。
    /// </summary>
    public void Complete()
    {
        _isActive = false;
        _targetEntityId = null;
        _revision++;
    }

    /// <summary>
    /// 取消放置模式，保持实体原位置。
    /// </summary>
    public void Cancel()
    {
        _isActive = false;
        _targetEntityId = null;
        _revision++;
    }
}

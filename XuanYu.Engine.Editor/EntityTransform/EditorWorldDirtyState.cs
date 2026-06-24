namespace XuanYu.Engine.Editor.EntityTransform;

/// <summary>
/// 场景修改状态跟踪。只提供状态反馈，不实现磁盘保存。
/// </summary>
public sealed class EditorWorldDirtyState
{
    private bool _isDirty;
    private int _revision;
    private string? _lastChangedEntityId;

    /// <summary>场景是否已被修改（未保存）。</summary>
    public bool IsDirty => _isDirty;

    /// <summary>状态修订号。</summary>
    public int Revision => _revision;

    /// <summary>最后被修改的实体 ID。</summary>
    public string? LastChangedEntityId => _lastChangedEntityId;

    /// <summary>
    /// 标记场景已修改。
    /// </summary>
    public void MarkDirty(string entityId)
    {
        if (!_isDirty)
            _isDirty = true;
        _lastChangedEntityId = entityId;
        _revision++;
    }

    /// <summary>
    /// 重置为未修改状态。
    /// </summary>
    public void Reset()
    {
        _isDirty = false;
        _lastChangedEntityId = null;
        _revision++;
    }
}

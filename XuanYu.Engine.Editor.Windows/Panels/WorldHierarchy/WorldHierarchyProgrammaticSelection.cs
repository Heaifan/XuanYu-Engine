namespace XuanYu.Engine.Editor.Windows.Panels.WorldHierarchy;

/// <summary>
/// 程序化选择一次性令牌。
/// 当 3D Picking 等外部操作需要同步树时，设置此令牌；
/// TreeView SelectionChanged 事件中检查并消费，防止向 EditorShell 发出重复选择命令。
/// </summary>
public sealed class WorldHierarchyProgrammaticSelection
{
    public string? ExpectedEntityId { get; private set; }
    public long ExpectedRevision { get; private set; }
    public bool IsPending { get; private set; }

    public void Begin(string? entityId, long revision)
    {
        ExpectedEntityId = entityId;
        ExpectedRevision = revision;
        IsPending = true;
    }

    /// <summary>
    /// 尝试消费令牌。如果匹配则消耗并返回 true，否则返回 false。
    /// </summary>
    public bool TryConsume(string? entityId, long revision)
    {
        if (!IsPending) return false;
        if (ExpectedEntityId != entityId)
        {
            // 不一致令牌：清除，不消费（视为真实用户操作）
            Reset();
            return false;
        }

        Reset();
        return true;
    }

    public void Reset()
    {
        ExpectedEntityId = null;
        ExpectedRevision = 0;
        IsPending = false;
    }
}

namespace XuanYu.Engine.Editor.Selection;

/// <summary>
/// 唯一选择状态——SelectedEntityId 是实体选择的唯一真源。
/// 相同 EntityId 再次选择是 NoOp（Revision 不增加，IsChanged==false）。
/// </summary>
public sealed class EditorEntitySelectionState
{
    public string? SelectedEntityId { get; private set; }
    public long Revision { get; private set; }

    /// <summary>
    /// 尝试应用选择变更。
    /// </summary>
    public EditorEntitySelectionChange TryApply(
        string? entityId,
        EditorEntitySelectionOrigin origin)
    {
        if (SelectedEntityId == entityId)
        {
            return new EditorEntitySelectionChange(
                false, Revision,
                SelectedEntityId, SelectedEntityId, origin);
        }

        var prev = SelectedEntityId;
        SelectedEntityId = entityId;
        Revision++;

        return new EditorEntitySelectionChange(
            true, Revision,
            prev, entityId, origin);
    }
}

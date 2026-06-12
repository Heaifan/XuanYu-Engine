namespace FluidWarfare.Editor.Selection;

/// <summary>
/// 一次选择状态提交的结果。IsChanged==false 表示相同 EntityId 幂等 NoOp。
/// </summary>
public sealed record EditorEntitySelectionChange(
    bool IsChanged,
    long Revision,
    string? PreviousEntityId,
    string? CurrentEntityId,
    EditorEntitySelectionOrigin Origin);

using FluidWarfare.Project.World.Transform;

namespace FluidWarfare.Editor.Transform.Edit;

/// <summary>
/// 编辑事务完成结果。Confirm 或 Cancel 时由 Transaction 发出。
/// </summary>
public readonly record struct TransformEditResult(
    TransformEditKind Kind,
    bool WasConfirmed,
    bool WasCancelled,
    SceneTransform? FinalTransform,
    bool InitialDirty);

using XuanYu.Engine.Project.World.Transform;

namespace FluidWarfare.Editor.Transform.Edit;

/// <summary>
/// 编辑事务开始时的快照。用于 Cancel 时恢复。
/// </summary>
public readonly record struct TransformEditSnapshot(
    string EntityId,
    SceneTransform InitialTransform,
    bool InitialDirty,
    TransformEditKind Kind);

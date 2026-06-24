namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;

/// <summary>
/// 最近成功渲染的 Move Gizmo 布局快照。
/// HitTest 必须使用此快照，保证所见即所点。
/// </summary>
public readonly record struct PresentedMoveGizmoSnapshot(
    bool IsAvailable,
    string EntityId,
    long SelectionRevision,
    long TransformRevision,
    int CameraRevision,
    int ViewportWidth,
    int ViewportHeight,
    MoveGizmoLayout Layout)
{
    public static readonly PresentedMoveGizmoSnapshot None = new(
        false, string.Empty, 0, 0, 0, 0, 0, null!);
}

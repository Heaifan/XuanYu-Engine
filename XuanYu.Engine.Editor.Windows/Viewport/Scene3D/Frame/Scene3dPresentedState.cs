using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
using FluidWarfare.Render.Selection.Presented;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Frame;

/// <summary>
/// Gizmo 和 Pick Snapshot 的 Pending/Presented 双缓冲。
/// Pending 在帧执行前设置，Present 成功后提升为 Presented。
/// Presented 只在 Present 成功后更改；Present 失败保留旧值。
/// </summary>
public sealed class Scene3dPresentedState
{
    public PresentedMoveGizmoSnapshot PendingGizmo { get; private set; } = PresentedMoveGizmoSnapshot.None;
    public PresentedScenePickSnapshot PendingPick { get; private set; } = PresentedScenePickSnapshot.None;

    public PresentedMoveGizmoSnapshot PresentedGizmo { get; private set; } = PresentedMoveGizmoSnapshot.None;
    public PresentedScenePickSnapshot PresentedPick { get; private set; } = PresentedScenePickSnapshot.None;

    /// <summary>设置 Gizmo Pending（Presenter 在帧执行前调用）。</summary>
    public void SetPendingGizmo(PresentedMoveGizmoSnapshot snapshot)
    {
        PendingGizmo = snapshot.IsAvailable ? snapshot : PresentedMoveGizmoSnapshot.None;
    }

    /// <summary>设置 Pick Snapshot Pending。</summary>
    public void SetPendingPick(PresentedScenePickSnapshot snapshot)
    {
        PendingPick = snapshot.IsValid ? snapshot : PresentedScenePickSnapshot.None;
    }

    /// <summary>清除 Gizmo Pending（不修改 Presented）。只在 Present 成功后才清除 Presented。</summary>
    public void ClearPendingGizmo()
    {
        PendingGizmo = PresentedMoveGizmoSnapshot.None;
    }

    /// <summary>Present 成功：提升 Pending 为 Presented。</summary>
    public void OnPresentSuccess()
    {
        if (PendingGizmo.IsAvailable)
            PresentedGizmo = PendingGizmo;
        if (PendingPick.IsValid)
            PresentedPick = PendingPick;
    }

    /// <summary>Present 失败：保留 Presented 不变。</summary>
    public void OnPresentFailed()
    {
        // Presented 保留之前的值
    }

    /// <summary>重置所有状态。</summary>
    public void Reset()
    {
        PendingGizmo = PresentedMoveGizmoSnapshot.None;
        PendingPick = PresentedScenePickSnapshot.None;
        PresentedGizmo = PresentedMoveGizmoSnapshot.None;
        PresentedPick = PresentedScenePickSnapshot.None;
    }
}

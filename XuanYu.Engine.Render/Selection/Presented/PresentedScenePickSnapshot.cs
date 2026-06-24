namespace XuanYu.Engine.Render.Selection.Presented;

/// <summary>
/// 已成功 Present 的场景拾取快照。
/// 代表用户当前真正看见的那一帧中每个实体的实际位置和包围盒。
/// Picking 只能读取此快照，不直接使用可能提前更新的 _renderScene。
/// </summary>
public sealed record PresentedScenePickSnapshot(
    long FrameIndex,
    int CameraRevision,
    int ViewportWidth,
    int ViewportHeight,
    bool IsValid,
    IReadOnlyList<PresentedEntityBounds> Entities)
{
    public static readonly PresentedScenePickSnapshot None = new(
        -1, -1, 0, 0, false, Array.Empty<PresentedEntityBounds>());
}

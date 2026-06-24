using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Editor.ViewportGround;

/// <summary>
/// 地面指针状态变更结果，用于判断是否需要刷新 UI 或提交 Scene3D 帧。
/// </summary>
public sealed record EditorGroundPointerChange(
    bool IsChanged,
    bool IsCommit,
    Vector3d? PreviousPosition,
    Vector3d? CurrentPosition)
{
    public static readonly EditorGroundPointerChange NoChange = new(
        false, false, null, null);
}

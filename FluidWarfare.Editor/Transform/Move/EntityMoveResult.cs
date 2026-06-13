using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Move;

/// <summary>
/// 编辑工具移动结果：Confirmed 或 Cancelled。
/// </summary>
public readonly record struct EntityMoveResult(
    bool IsConfirmed,
    bool IsCancelled,
    bool HasPositionChanged,
    Vector3d? FinalPosition)
{
    public bool IsCompleted => IsConfirmed || IsCancelled;
}

using FluidWarfare.Core.Math;

namespace FluidWarfare.Engine.Components;

/// <summary>
/// 实体位置组件，包装 Vector3d。
/// </summary>
public readonly record struct PositionComponent(Vector3d Value);

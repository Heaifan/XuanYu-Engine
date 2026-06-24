using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Components;

/// <summary>实体缩放组件，包装 Vector3d。默认值 (1, 1, 1)。</summary>
public readonly record struct ScaleComponent(Vector3d Value);

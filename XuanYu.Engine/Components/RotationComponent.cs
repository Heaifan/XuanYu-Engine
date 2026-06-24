using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Components;

/// <summary>实体旋转组件（欧拉角，单位：度），包装 Vector3d。</summary>
public readonly record struct RotationComponent(Vector3d Value);

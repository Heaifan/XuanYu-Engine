using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Editor.Windows.Inspector.TransformEdit;

/// <summary>Inspector 发起的 Transform 编辑请求。</summary>
public sealed record TransformEditRequest(
    string? EntityId,
    Vector3d Position,
    Vector3d RotationDegrees,
    Vector3d Scale);

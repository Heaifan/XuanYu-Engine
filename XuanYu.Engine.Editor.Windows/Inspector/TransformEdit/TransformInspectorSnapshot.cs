using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Editor.Windows.Inspector.TransformEdit;

/// <summary>Inspector 显示的选中实体 Transform 快照。</summary>
public sealed record TransformInspectorSnapshot(
    string EntityId,
    Vector3d Position,
    Vector3d RotationDegrees,
    Vector3d Scale);

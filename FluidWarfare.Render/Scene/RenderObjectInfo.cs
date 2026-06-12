using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Render.Selection;

namespace FluidWarfare.Render.Scene;

/// <summary>
/// 表示一个可被渲染后端消费的最小对象。
/// 只保存数据，不画图，不持有 GPU 资源。
/// SelectionBounds 与渲染尺寸使用同一数据源，防止绘制与 Picking 尺寸分叉。
/// </summary>
public sealed record RenderObjectInfo(
    EntityId EntityId,
    string DisplayName,
    Vector3d Position,
    RenderObjectVisualKind VisualKind,
    string? SourcePath,
    SceneAxisAlignedBounds? SelectionBounds);

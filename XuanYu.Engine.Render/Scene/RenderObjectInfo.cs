using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using FluidWarfare.Render.Selection;

namespace FluidWarfare.Render.Scene;

/// <summary>
/// 表示一个可被渲染后端消费的最小对象。
/// 只保存数据，不画图，不持有 GPU 资源。
/// Placement 是渲染位置与 Picking 包围盒的单一真源。
/// </summary>
public sealed record RenderObjectInfo(
    EntityId EntityId,
    string DisplayName,
    Vector3d Position,
    RenderObjectVisualKind VisualKind,
    string? SourcePath,
    SceneAxisAlignedBounds? SelectionBounds)
{
    /// <summary>
    /// 渲染单位放置信息（单一真源）。
    /// 由 WorldToRenderSceneBuilder 创建，EditorShell.BuildUnitDrawList 从此读取视觉中心。
    /// </summary>
    public RenderUnitPlacement? Placement { get; init; }
}

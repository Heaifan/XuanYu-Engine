using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.World;
using XuanYu.Engine.Render.Scene;

namespace FluidWarfare.Editor.Windows.Viewport.Camera;

/// <summary>
/// 计算 FrameSelected 的聚焦目标（包围盒中心 + 半径）。
/// 从世界实体位置生成相机 FrameSelected 参数。
/// </summary>
public static class ViewportCameraFocusTarget
{
    /// <summary>
    /// 从实体 ID 和 WorldState 计算聚焦目标。
    /// </summary>
    /// <param name="entityId">目标实体 ID。</param>
    /// <param name="world">世界状态。</param>
    /// <returns>聚焦目标，或 null（实体不存在、无位置）。</returns>
    public static (float CenterX, float CenterY, float CenterZ, float Radius)? Compute(
        EntityId entityId, WorldState world)
    {
        var pos = world.FindPosition(entityId);
        if (pos is null) return null;

        var placement = new RenderUnitPlacement(pos.Value.Value);
        return (
            (float)placement.VisualCenter.X,
            (float)placement.VisualCenter.Y,
            (float)placement.VisualCenter.Z,
            (float)RenderUnitPlacement.HalfExtent);
    }
}

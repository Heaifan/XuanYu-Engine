using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Vulkan.Scene3D;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Frame;

/// <summary>
/// 从 RenderScene 构建 Vulkan 单位绘制列表。
/// 优先读取 RenderUnitPlacement.VisualCenter，无 Placement 时从 Position 推算。
/// </summary>
public static class Scene3dDrawListBuilder
{
    public static List<VulkanScene3dUnitDrawInfo> Build(RenderScene scene)
    {
        var list = new List<VulkanScene3dUnitDrawInfo>();
        foreach (var obj in scene.Objects)
        {
            if (obj.VisualKind != RenderObjectVisualKind.UnitMarker) continue;
            var (vx, vy, vz) = obj.Placement is not null
                ? ((float)obj.Placement.VisualCenter.X,
                   (float)obj.Placement.VisualCenter.Y,
                   (float)obj.Placement.VisualCenter.Z)
                : ((float)obj.Position.X,
                   (float)obj.Position.Y,
                   (float)obj.Position.Z + (float)RenderUnitPlacement.HalfExtent);
            list.Add(new VulkanScene3dUnitDrawInfo(
                obj.EntityId.Value.ToString(), vx, vy, vz,
                (float)RenderUnitPlacement.Scale));
        }
        return list;
    }
}

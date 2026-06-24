using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

/// <summary>Scene3D 实体位置更新能力。仅暴露 UpdateEntityPosition，不暴露 Session 生命周期。</summary>
public sealed class Scene3dEntityPositionWriter
{
    readonly VulkanScene3dSession _session;

    public Scene3dEntityPositionWriter(VulkanScene3dSession session) => _session = session;

    public void Write(EntityId entityId, Vector3d position)
    {
        var p = new RenderUnitPlacement(position);
        _session.UpdateEntityPosition(
            entityId.Value.ToString(),
            (float)p.VisualCenter.X, (float)p.VisualCenter.Y, (float)p.VisualCenter.Z);
    }
}

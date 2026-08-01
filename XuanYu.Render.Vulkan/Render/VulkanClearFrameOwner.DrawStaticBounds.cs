using Silk.NET.Vulkan;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void DrawStaticModelBounds(CommandBuffer cb, float* scene,
        RenderEntityProjection entity, RenderStaticModelResource model)
    {
        var min = model.LocalBounds.Min;
        var max = model.LocalBounds.Max;
        var center = new Vector3d(
            (min.X + max.X) * 0.5,
            (min.Y + max.Y) * 0.5,
            (min.Z + max.Z) * 0.5);
        var size = new Vector3d(max.X - min.X, max.Y - min.Y, max.Z - min.Z);
        var scaledCenter = new Vector3d(
            center.X * entity.Scale.X,
            center.Y * entity.Scale.Y,
            center.Z * entity.Scale.Z);
        var worldCenter = entity.Position + Rotate(scaledCenter, entity.Rotation);
        var outlineScale = new Vector3d(
            size.X * entity.Scale.X,
            size.Y * entity.Scale.Y,
            size.Z * entity.Scale.Z);
        FillScenePushConstants(scene, _renderProjection, worldCenter, entity.Rotation,
            outlineScale, 0.0f, 2.0f, -1.0f);
        PushSceneConstants(cb, scene);
        _vk.CmdDraw(cb, (uint)RenderDrawPlan.CubeOutlineRibbonVertexCount, 1, 0, 0);
    }

    static Vector3d Rotate(Vector3d v, Vector3d deg)
    {
        var rx = double.Pi * deg.X / 180.0;
        var ry = double.Pi * deg.Y / 180.0;
        var rz = double.Pi * deg.Z / 180.0;
        var x1 = v.X;
        var y1 = (v.Y * Math.Cos(rx)) - (v.Z * Math.Sin(rx));
        var z1 = (v.Y * Math.Sin(rx)) + (v.Z * Math.Cos(rx));
        var x2 = (x1 * Math.Cos(ry)) + (z1 * Math.Sin(ry));
        var z2 = (-x1 * Math.Sin(ry)) + (z1 * Math.Cos(ry));
        return new Vector3d(
            (x2 * Math.Cos(rz)) - (y1 * Math.Sin(rz)),
            (x2 * Math.Sin(rz)) + (y1 * Math.Cos(rz)),
            z2);
    }
}

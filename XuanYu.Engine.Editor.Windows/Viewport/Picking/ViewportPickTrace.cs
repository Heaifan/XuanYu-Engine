using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Selection;
using XuanYu.Engine.Render.Vulkan.Camera;

namespace XuanYu.Engine.Editor.Windows.Viewport.Picking;

/// <summary>
/// Debug Picking 诊断。非 Entity 命中时输出每个实体的位置和 Ray-AABB 结果。
/// </summary>
public static class ViewportPickTrace
{
    public static void Write(int pixelX, int pixelY, PresentedCameraSnapshot snapshot,
        SceneRay ray, RenderScene renderScene)
    {
        System.Diagnostics.Debug.WriteLine(
            $"[PickTrace] Click({pixelX},{pixelY}) " +
            $"RO({ray.Origin.X:F2},{ray.Origin.Y:F2},{ray.Origin.Z:F2}) " +
            $"RD({ray.Direction.X:F3},{ray.Direction.Y:F3},{ray.Direction.Z:F3})");

        var vp = snapshot.ViewProjection;
        var w = snapshot.ViewportWidth;
        var h = snapshot.ViewportHeight;

        foreach (var obj in renderScene.Objects)
        {
            if (obj.VisualKind != RenderObjectVisualKind.UnitMarker) continue;
            if (obj.SelectionBounds is null) continue;

            var p = obj.Placement;
            var b = obj.SelectionBounds;
            var sc = TryProject(b.Center, vp, w, h, out var sp) ? $"({sp.X:F1},{sp.Y:F1})" : "N/A";
            var hit = SceneRayBoundsIntersection.Test(ray, b, out var d) ? $"HIT d={d:F3}" : "MISS";

            System.Diagnostics.Debug.WriteLine(
                $"[PickTrace]  E{obj.EntityId.Value} " +
                $"Draw({p?.VisualCenter.X:F2},{p?.VisualCenter.Y:F2},{p?.VisualCenter.Z:F2}) " +
                $"BC({b.Center.X:F2},{b.Center.Y:F2},{b.Center.Z:F2}) " +
                $"SC{sc} Pick:{hit}");
        }
    }

    private static bool TryProject(Vector3d world, float[] vp, int vw, int vh, out (double X, double Y) pixel)
    {
        pixel = default;
        if (vp is not { Length: 16 } || vw <= 0 || vh <= 0) return false;
        var cw = vp[3] * world.X + vp[7] * world.Y + vp[11] * world.Z + vp[15];
        if (!double.IsFinite(cw) || Math.Abs(cw) < 1e-6) return false;
        var nx = (vp[0] * world.X + vp[4] * world.Y + vp[8] * world.Z + vp[12]) / cw;
        var ny = (vp[1] * world.X + vp[5] * world.Y + vp[9] * world.Z + vp[13]) / cw;
        if (!double.IsFinite(nx) || !double.IsFinite(ny)) return false;
        pixel = ((nx * 0.5 + 0.5) * vw, (ny * 0.5 + 0.5) * vh);
        return true;
    }
}

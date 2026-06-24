using XuanYu.Engine.Core.Math;
using FluidWarfare.Render.Selection;

namespace FluidWarfare.Render.Vulkan.Camera;

/// <summary>从 Vulkan 视口像素坐标生成世界空间 SceneRay。使用已呈现帧的 ViewProjection 反投影。</summary>
public static class VulkanSceneRayBuilder
{
    public static SceneRayBuildStatus TryBuild(float pixelX, float pixelY, PresentedCameraSnapshot snapshot,
        uint vpW, uint vpH, out SceneRay? ray)
    {
        ray = null;
        if (snapshot is null || !snapshot.IsValid) return SceneRayBuildStatus.SnapshotUnavailable;
        if (snapshot.InverseViewProjection.Length != 16) return SceneRayBuildStatus.MatrixInvalid;
        if (snapshot.ViewportWidth != (int)vpW || snapshot.ViewportHeight != (int)vpH)
            return SceneRayBuildStatus.SnapshotExtentMismatch;
        if (vpW == 0 || vpH == 0 || pixelX < 0 || pixelX >= vpW || pixelY < 0 || pixelY >= vpH)
            return SceneRayBuildStatus.PixelOutOfBounds;
        var ndcX = 2.0 * pixelX / vpW - 1.0; var ndcY = 2.0 * pixelY / vpH - 1.0;
        var ivp = snapshot.InverseViewProjection;
        var nearNdc = new[] { ndcX, ndcY, 0.0, 1.0 }; var farNdc = new[] { ndcX, ndcY, 1.0, 1.0 };
        if (!Transform(ivp, nearNdc, out var nw, out _) || !Transform(ivp, farNdc, out var fw, out _))
            return SceneRayBuildStatus.MatrixInvalid;
        var dx = fw[0] - nw[0]; var dy = fw[1] - nw[1]; var dz = fw[2] - nw[2];
        var len = Math.Sqrt(dx * dx + dy * dy + dz * dz);
        if (len < 1e-12) return SceneRayBuildStatus.DirectionInvalid;
        ray = new SceneRay(new Vector3d(nw[0], nw[1], nw[2]), new Vector3d(dx / len, dy / len, dz / len));
        return SceneRayBuildStatus.Success;
    }

    static bool Transform(double[] m, double[] v, out double[] r, out string e)
    {
        r = null!; e = string.Empty;
        if (m.Length != 16 || v.Length != 4) { e = "矩阵或向量维度错误。"; return false; }
        var res = new double[4];
        for (var row = 0; row < 4; row++) { res[row] = 0; for (var col = 0; col < 4; col++) res[row] += m[col * 4 + row] * v[col]; }
        if (Math.Abs(res[3]) < 1e-15) { e = "变换后 w=0。"; return false; }
        r = [res[0] / res[3], res[1] / res[3], res[2] / res[3], 1.0]; return true;
    }
}

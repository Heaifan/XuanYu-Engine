using FluidWarfare.Core.Math;
using FluidWarfare.Render.Selection;

namespace FluidWarfare.Render.Vulkan.Camera;

/// <summary>
/// 从 Vulkan 视口像素坐标生成世界空间 SceneRay。
/// 使用已呈现帧的 ViewProjection 和逆矩阵将 NDC 坐标反投影到世界空间。
/// 射线起点为近平面世界点（而非相机位置），统一支持透视和正交投影。
/// 只接受 PresentedCameraSnapshot，不接受分散的参数。
/// </summary>
public static class VulkanSceneRayBuilder
{
    /// <summary>
    /// 从像素坐标和已呈现帧快照构建世界射线。
    /// </summary>
    /// <param name="pixelX">视口物理像素 X（左上角 0，右下角 width-1）。</param>
    /// <param name="pixelY">视口物理像素 Y（左上角 0，右下角 height-1）。</param>
    /// <param name="snapshot">已呈现帧的快照。</param>
    /// <param name="currentViewportWidth">当前视口宽度（用于尺寸闸门）。</param>
    /// <param name="currentViewportHeight">当前视口高度（用于尺寸闸门）。</param>
    /// <param name="ray">输出世界射线。</param>
    /// <returns>构建状态。</returns>
    public static SceneRayBuildStatus TryBuild(
        float pixelX, float pixelY,
        PresentedCameraSnapshot snapshot,
        uint currentViewportWidth, uint currentViewportHeight,
        out SceneRay? ray)
    {
        ray = null;

        if (snapshot is null || !snapshot.IsValid)
            return SceneRayBuildStatus.SnapshotUnavailable;

        if (snapshot.InverseViewProjection.Length != 16)
            return SceneRayBuildStatus.MatrixInvalid;

        // 尺寸闸门：快照尺寸必须与当前视口一致
        if (snapshot.ViewportWidth != (int)currentViewportWidth ||
            snapshot.ViewportHeight != (int)currentViewportHeight)
            return SceneRayBuildStatus.SnapshotExtentMismatch;

        if (currentViewportWidth == 0 || currentViewportHeight == 0)
            return SceneRayBuildStatus.PixelOutOfBounds;

        if (pixelX < 0 || pixelX >= currentViewportWidth ||
            pixelY < 0 || pixelY >= currentViewportHeight)
            return SceneRayBuildStatus.PixelOutOfBounds;

        // NDC 坐标
        var ndcX = 2.0 * pixelX / currentViewportWidth - 1.0;
        var ndcY = 2.0 * pixelY / currentViewportHeight - 1.0;

        var invVp = snapshot.InverseViewProjection;

        // 近平面和远平面 NDC 点
        var nearNdc = new[] { ndcX, ndcY, 0.0, 1.0 };
        var farNdc = new[] { ndcX, ndcY, 1.0, 1.0 };

        // 反投影到世界空间
        if (!Transform(invVp, nearNdc, out var nearWorld, out _))
            return SceneRayBuildStatus.MatrixInvalid;

        if (!Transform(invVp, farNdc, out var farWorld, out _))
            return SceneRayBuildStatus.MatrixInvalid;

        // 射线起点 = nearWorld（统一透视和正交）
        // 透视时：cameraPosition、nearWorld、farWorld 在同一直线上
        // 正交时：每条射线起点不同（近平面各像素独立），方向平行
        var originX = nearWorld[0];
        var originY = nearWorld[1];
        var originZ = nearWorld[2];

        var dx = farWorld[0] - nearWorld[0];
        var dy = farWorld[1] - nearWorld[1];
        var dz = farWorld[2] - nearWorld[2];
        var len = Math.Sqrt(dx * dx + dy * dy + dz * dz);
        if (len < 1e-12)
            return SceneRayBuildStatus.DirectionInvalid;

        ray = new SceneRay(
            new Vector3d(originX, originY, originZ),
            new Vector3d(dx / len, dy / len, dz / len));
        return SceneRayBuildStatus.Success;
    }

    /// <summary>
    /// 4x4 矩阵变换列向量 (x, y, z, w)，double 版本。
    /// </summary>
    private static bool Transform(double[] matrix, double[] vec, out double[] result, out string error)
    {
        result = null!;
        error = string.Empty;
        if (matrix.Length != 16 || vec.Length != 4)
        {
            error = "矩阵或向量维度错误。";
            return false;
        }

        var r = new double[4];
        for (var row = 0; row < 4; row++)
        {
            r[row] = 0;
            for (var col = 0; col < 4; col++)
                r[row] += matrix[col * 4 + row] * vec[col];
        }

        if (Math.Abs(r[3]) < 1e-15)
        {
            error = "变换后 w=0。";
            return false;
        }
        result = [r[0] / r[3], r[1] / r[3], r[2] / r[3], 1.0];
        return true;
    }

    /// <summary>
    /// 4x4 矩阵求逆（列优先 float[]）。使用高斯-约旦消元法。
    /// </summary>
    internal static bool TryInvert(float[] m, out double[] result, out string error)
    {
        result = null!;
        error = string.Empty;

        var aug = new double[4, 8];
        for (var col = 0; col < 4; col++)
            for (var row = 0; row < 4; row++)
                aug[row, col] = m[col * 4 + row];
        for (var i = 0; i < 4; i++)
            aug[i, 4 + i] = 1.0;

        for (var col = 0; col < 4; col++)
        {
            var best = col;
            for (var row = col + 1; row < 4; row++)
                if (Math.Abs(aug[row, col]) > Math.Abs(aug[best, col]))
                    best = row;

            if (Math.Abs(aug[best, col]) < 1e-15)
            {
                error = "矩阵奇异。";
                return false;
            }

            for (var j = 0; j < 8; j++)
                (aug[col, j], aug[best, j]) = (aug[best, j], aug[col, j]);

            var pivot = aug[col, col];
            for (var j = 0; j < 8; j++)
                aug[col, j] /= pivot;

            for (var row = 0; row < 4; row++)
            {
                if (row == col) continue;
                var factor = aug[row, col];
                for (var j = 0; j < 8; j++)
                    aug[row, j] -= factor * aug[col, j];
            }
        }

        result = new double[16];
        for (var col = 0; col < 4; col++)
            for (var row = 0; row < 4; row++)
                result[col * 4 + row] = aug[row, 4 + col];
        return true;
    }
}

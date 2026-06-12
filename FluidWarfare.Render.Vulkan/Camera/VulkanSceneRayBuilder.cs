using FluidWarfare.Core.Math;
using FluidWarfare.Render.Selection;

namespace FluidWarfare.Render.Vulkan.Camera;

/// <summary>
/// 从 Vulkan 视口像素坐标生成世界空间 SceneRay。
/// 使用当前 ViewProjection 逆矩阵将 NDC 坐标反投影到世界空间。
/// 只负责数学变换，不负责遍历 RenderObject 或修改选择。
/// </summary>
public static class VulkanSceneRayBuilder
{
    /// <summary>
    /// 从像素坐标构建世界射线。
    /// </summary>
    /// <param name="pixelX">视口物理像素 X（左上角 0，右下角 width-1）。</param>
    /// <param name="pixelY">视口物理像素 Y（左上角 0，右下角 height-1）。</param>
    /// <param name="viewportWidth">视口物理宽度（像素）。</param>
    /// <param name="viewportHeight">视口物理高度（像素）。</param>
    /// <param name="viewProjection">列优先 ViewProjection 矩阵 (float[16])。</param>
    /// <param name="cameraPosition">相机世界位置。</param>
    /// <param name="ray">输出世界射线。</param>
    /// <param name="errorMessage">失败原因。</param>
    /// <returns>成功返回 true。</returns>
    public static bool TryBuild(
        float pixelX, float pixelY,
        uint viewportWidth, uint viewportHeight,
        float[] viewProjection,
        Vector3d cameraPosition,
        out SceneRay ray,
        out string errorMessage)
    {
        ray = null!;
        errorMessage = string.Empty;

        if (viewportWidth == 0 || viewportHeight == 0)
        {
            errorMessage = $"视口尺寸无效：{viewportWidth}x{viewportHeight}。";
            return false;
        }

        if (viewProjection is null || viewProjection.Length != 16)
        {
            errorMessage = "ViewProjection 矩阵无效。";
            return false;
        }

        // NDC 坐标：Vulkan 原点左上角，Y 向下，[0,0]→[-1,+1]，[w,h]→[+1,-1]
        var ndcX = 2.0 * pixelX / viewportWidth - 1.0;
        var ndcY = -(2.0 * pixelY / viewportHeight - 1.0);

        // 计算逆矩阵
        if (!TryInvert(viewProjection, out var invVp, out var invErr))
        {
            errorMessage = $"ViewProjection 不可逆：{invErr}。";
            return false;
        }

        // 近平面和远平面 NDC 点
        var nearNdc = new[] { ndcX, ndcY, 0.0, 1.0 };
        var farNdc = new[] { ndcX, ndcY, 1.0, 1.0 };

        // 反投影到世界空间
        if (!Transform(invVp, nearNdc, out var nearWorld, out var tErr))
        {
            errorMessage = $"近平面反投影失败：{tErr}。";
            return false;
        }
        if (!Transform(invVp, farNdc, out var farWorld, out _))
        {
            errorMessage = "远平面反投影失败。";
            return false;
        }

        // 方向 = Normalize(far - near)
        var dx = farWorld[0] - nearWorld[0];
        var dy = farWorld[1] - nearWorld[1];
        var dz = farWorld[2] - nearWorld[2];
        var len = Math.Sqrt(dx * dx + dy * dy + dz * dz);
        if (len < 1e-12)
        {
            errorMessage = "反投影后射线方向为零向量。";
            return false;
        }

        ray = new SceneRay(
            new Vector3d(cameraPosition.X, cameraPosition.Y, cameraPosition.Z),
            new Vector3d(dx / len, dy / len, dz / len));
        return true;
    }

    /// <summary>
    /// 4x4 矩阵求逆（列优先 float[]）。使用高斯-约旦消元法。
    /// </summary>
    private static bool TryInvert(float[] m, out double[] result, out string error)
    {
        result = null!;
        error = string.Empty;

        // 扩展矩阵 [A|I]
        var aug = new double[4, 8];
        for (var col = 0; col < 4; col++)
            for (var row = 0; row < 4; row++)
                aug[row, col] = m[col * 4 + row];
        for (var i = 0; i < 4; i++)
            aug[i, 4 + i] = 1.0;

        for (var col = 0; col < 4; col++)
        {
            // 选主元
            var best = col;
            for (var row = col + 1; row < 4; row++)
                if (Math.Abs(aug[row, col]) > Math.Abs(aug[best, col]))
                    best = row;

            if (Math.Abs(aug[best, col]) < 1e-15)
            {
                error = "矩阵奇异。";
                return false;
            }

            // 交换行
            for (var j = 0; j < 8; j++)
                (aug[col, j], aug[best, j]) = (aug[best, j], aug[col, j]);

            // 归一化
            var pivot = aug[col, col];
            for (var j = 0; j < 8; j++)
                aug[col, j] /= pivot;

            // 消去其他行
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

    /// <summary>
    /// 4x4 矩阵变换列向量 (x, y, z, w)。
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

        // 除以 w
        if (Math.Abs(r[3]) < 1e-15)
        {
            error = "变换后 w=0。";
            return false;
        }
        result = [r[0] / r[3], r[1] / r[3], r[2] / r[3], 1.0];
        return true;
    }
}

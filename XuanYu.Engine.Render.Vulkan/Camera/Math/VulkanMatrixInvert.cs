namespace XuanYu.Engine.Render.Vulkan.Camera;

/// <summary>4x4 矩阵求逆（列优先 float[]）。使用高斯-约旦消元法。</summary>
public static class VulkanMatrixInvert
{
    public static bool TryInvert(float[] m, out double[] result, out string error)
    {
        result = null!; error = string.Empty;
        var aug = new double[4, 8];
        for (var col = 0; col < 4; col++) for (var row = 0; row < 4; row++) aug[row, col] = m[col * 4 + row];
        for (var i = 0; i < 4; i++) aug[i, 4 + i] = 1.0;
        for (var col = 0; col < 4; col++)
        {
            var best = col;
            for (var row = col + 1; row < 4; row++) if (Math.Abs(aug[row, col]) > Math.Abs(aug[best, col])) best = row;
            if (Math.Abs(aug[best, col]) < 1e-15) { error = "矩阵奇异。"; return false; }
            for (var j = 0; j < 8; j++) (aug[col, j], aug[best, j]) = (aug[best, j], aug[col, j]);
            var pivot = aug[col, col]; for (var j = 0; j < 8; j++) aug[col, j] /= pivot;
            for (var row = 0; row < 4; row++) { if (row == col) continue; var f = aug[row, col]; for (var j = 0; j < 8; j++) aug[row, j] -= f * aug[col, j]; }
        }
        result = new double[16];
        for (var col = 0; col < 4; col++) for (var row = 0; row < 4; row++) result[col * 4 + row] = aug[row, 4 + col];
        return true;
    }
}

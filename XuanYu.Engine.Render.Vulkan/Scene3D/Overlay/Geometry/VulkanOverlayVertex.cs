using System.Runtime.InteropServices;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Overlay;

/// <summary>Overlay 顶点：像素坐标 + RGBA 颜色。顶点着色器将像素坐标转换为 Vulkan NDC。</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct VulkanOverlayVertex
{
    /// <summary>像素 X 坐标。</summary>
    public readonly float PixelX;
    /// <summary>像素 Y 坐标。</summary>
    public readonly float PixelY;
    /// <summary>红色。</summary>
    public readonly float R;
    /// <summary>绿色。</summary>
    public readonly float G;
    /// <summary>蓝色。</summary>
    public readonly float B;
    /// <summary>透明度。</summary>
    public readonly float A;

    public VulkanOverlayVertex(float pixelX, float pixelY, float r, float g, float b, float a)
    { PixelX = pixelX; PixelY = pixelY; R = r; G = g; B = b; A = a; }

    /// <summary>将顶点数组转换为 float[] 交错格式（x, y, r, g, b, a）。</summary>
    public static float[] ToInterleaved(ReadOnlySpan<VulkanOverlayVertex> vertices)
    {
        var result = new float[vertices.Length * 6];
        for (var i = 0; i < vertices.Length; i++)
        { var v = vertices[i]; var o = i * 6; result[o] = v.PixelX; result[o + 1] = v.PixelY; result[o + 2] = v.R; result[o + 3] = v.G; result[o + 4] = v.B; result[o + 5] = v.A; }
        return result;
    }
}

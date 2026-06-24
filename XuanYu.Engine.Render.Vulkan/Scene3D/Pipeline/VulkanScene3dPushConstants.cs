namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>
/// Push Constant 数据布局：MVP (mat4, 64 字节) + Tint (vec4, 16 字节) = 80 字节。
/// 普通单位 TintAlpha = 0（混合系数 0，使用顶点色），选中单位 TintAlpha = 1（使用 TintRGB）。
/// Grid 始终使用 TintAlpha = 0。
/// </summary>
internal static class VulkanScene3dPushConstants
{
    /// <summary>Push Constant 总字节数。</summary>
    public const int ByteSize = 80;

    /// <summary>float 数量。</summary>
    public const int FloatCount = 20;

    /// <summary>MVP 偏移量（字节）。</summary>
    public const int MvpOffset = 0;

    /// <summary>MVP 字节数。</summary>
    public const int MvpByteSize = 64;

    /// <summary>Tint 偏移量（字节）。</summary>
    public const int TintOffset = 64;

    /// <summary>Tint 字节数。</summary>
    public const int TintByteSize = 16;

    // ─── Tint 颜色常量 ───────────────────────────────────────────

    /// <summary>普通单位颜色 (rgba)。</summary>
    public static readonly float[] NormalTint = [1.00f, 0.82f, 0.20f, 0f];

    /// <summary>选中单位高亮颜色 (rgba)。</summary>
    public static readonly float[] SelectedTint = [1.00f, 0.35f, 0.05f, 1f];

    /// <summary>Grid 始终使用顶点色。</summary>
    public static readonly float[] GridTint = [1f, 1f, 1f, 0f];
}

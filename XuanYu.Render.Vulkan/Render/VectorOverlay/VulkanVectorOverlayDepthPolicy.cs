using System.Numerics;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render.VectorOverlay;

internal static class VulkanVectorOverlayDepthPolicy
{
    public const float FillClipBias = 0.000002f;
    public const float StrokeClipBias = 0.000010f;
    public const float MarkerClipBias = 0.000020f;

    public static float ModeFor(RenderVectorOverlayPrimitiveKind kind) => (float)kind;

    public static float ClipBiasFor(RenderVectorOverlayPrimitiveKind kind) => kind switch
    {
        RenderVectorOverlayPrimitiveKind.Fill => FillClipBias,
        RenderVectorOverlayPrimitiveKind.Stroke => StrokeClipBias,
        RenderVectorOverlayPrimitiveKind.Marker => MarkerClipBias,
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    public static Vector4 Apply(Vector4 clipPosition, RenderVectorOverlayPrimitiveKind kind)
    {
        var z = MathF.Max(0.0f, clipPosition.Z - (clipPosition.W * ClipBiasFor(kind)));
        return new Vector4(clipPosition.X, clipPosition.Y, z, clipPosition.W);
    }
}

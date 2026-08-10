using System.Runtime.InteropServices;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render.VectorOverlay;

[StructLayout(LayoutKind.Sequential)]
readonly record struct VulkanVectorOverlayVertex(
    float X, float Y, float Z, float Sx, float Sy, float Sz, float U, float V)
{
    public const uint Stride = 32;

    public static VulkanVectorOverlayVertex From(RenderVectorOverlayVertex v) => new(
        (float)v.Position.X, (float)v.Position.Y, (float)v.Position.Z,
        (float)v.Secondary.X, (float)v.Secondary.Y, (float)v.Secondary.Z,
        (float)v.U, (float)v.V);
}

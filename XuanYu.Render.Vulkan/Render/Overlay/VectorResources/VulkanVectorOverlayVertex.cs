using System.Runtime.InteropServices;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render.VectorOverlay;

[StructLayout(LayoutKind.Sequential)]
readonly record struct VulkanVectorOverlayVertex(
    float X, float Y, float Z, float Sx, float Sy, float Sz, float U, float V)
{
    public const uint Stride = 32;

    public static VulkanVectorOverlayVertex From(RenderVectorOverlayVertex v, Vector3d renderOrigin) => new(
        (float)(v.Position.X - renderOrigin.X), (float)(v.Position.Y - renderOrigin.Y), (float)(v.Position.Z - renderOrigin.Z),
        (float)(v.Secondary.X - renderOrigin.X), (float)(v.Secondary.Y - renderOrigin.Y), (float)(v.Secondary.Z - renderOrigin.Z),
        (float)v.U, (float)v.V);
}

using System.Runtime.InteropServices;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render.StaticModels;

[StructLayout(LayoutKind.Sequential)]
readonly record struct VulkanStaticModelVertex(
    float X, float Y, float Z,
    float Nx, float Ny, float Nz,
    float U, float V)
{
    public const uint Stride = 32;

    public static VulkanStaticModelVertex From(RenderStaticModelVertex v) =>
        new((float)v.Position.X, (float)v.Position.Y, (float)v.Position.Z,
            (float)v.Normal.X, (float)v.Normal.Y, (float)v.Normal.Z,
            (float)v.U, (float)v.V);
}

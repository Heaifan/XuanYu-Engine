using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.GroundCursor;

/// <summary>
/// Ground Cursor 诊断信息。
/// </summary>
public sealed record VulkanGroundCursorInfo(
    bool IsVisible,
    Vector3d? WorldPosition,
    int Revision,
    int VertexCount,
    int DrawCalls)
{
    public static readonly VulkanGroundCursorInfo Hidden = new(
        false, null, 0, 0, 0);
}

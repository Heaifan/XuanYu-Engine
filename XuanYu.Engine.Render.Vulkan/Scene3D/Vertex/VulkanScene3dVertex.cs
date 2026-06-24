namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>3D 场景顶点（位置 + 颜色），Stride = 28。</summary>
public readonly record struct VulkanScene3dVertex(
    float X, float Y, float Z, float R, float G, float B, float A);

/// <summary>单位 3D 绘制信息：世界坐标和缩放。</summary>
public readonly record struct VulkanScene3dUnitDrawInfo(
    string? EntityId, float X, float Y, float Z, float Scale);

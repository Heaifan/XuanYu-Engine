using FluidWarfare.Render.ViewportNavigation;

namespace FluidWarfare.Render.Vulkan.Scene3D.Overlay;

/// <summary>Overlay 诊断信息。</summary>
public sealed record VulkanNavigationOverlayInfo(
    bool IsAvailable,
    int VertexCount,
    int VertexCapacity,
    int DrawCallCount,
    ViewportNavigationElement HoveredElement,
    string StatusText);

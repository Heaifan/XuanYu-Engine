using FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost;
using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Resize;

/// <summary>渲染重绘请求的输入数据。Shell 在每次触发重绘时创建。</summary>
public sealed record Scene3dResizeRenderRequest(
    bool BackendAvailable,
    bool DeviceCreated,
    VulkanViewportNativeHostInfo NativeHost,
    bool SessionActive,
    VulkanScene3dSession? Session,
    ViewportCameraRoute CameraRoute,
    RenderScene RenderScene,
    int RenderSeq);

using XuanYu.Engine.Render.Vulkan.Scene3D;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Diagnostics;

/// <summary>Scene3D 诊断快照。Shell 读取后分发到 DebugDockPanel。</summary>
public sealed record Scene3dDiagnosticSnapshot(
    string VulkanStatus,
    string InstanceStatus,
    string DeviceStatus,
    string NativeHostStatus,
    string SurfaceStatus,
    string SwapchainStatus,
    string ClearStatus,
    string Scene3dStatus,
    string ValidationStatus,
    string Scene3dCamera,
    string Scene3dGrid,
    string Scene3dUnits,
    string Scene3dDrawCalls);

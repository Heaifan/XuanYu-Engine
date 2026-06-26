using Avalonia.Threading;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Resize;
using XuanYu.Engine.Editor.Windows.Shell.Startup.Vulkan;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Diagnostics;

namespace XuanYu.Engine.Editor.Windows.ShellV2.Composition;

/// <summary>EditorShellV2 上下文。只持有 V2 需要的路由和控件引用，不引用旧 Shell 的 Inspector/Debug/Project 面板。</summary>
sealed class EditorShellV2Context
{
    // ─── 控件引用 ────────────────────────────────────────
    public VulkanViewportHostPanel? ViewportPanel { get; set; }

    // ─── 路由 ────────────────────────────────────────────
    public VulkanViewportProbeRoute ProbeRoute = null!;
    public EditorStartupVulkanRoute StartupVulkan = null!;
    public Scene3dSessionLifecycle Lifecycle = null!;
    public ViewportRenderSceneStore RenderStore = null!;
    public ViewportCameraRoute CameraRoute = null!;
    public Scene3dResizeRenderRoute ResizeRender = null!;
    public EditorDiagnosticsRefreshRoute Diagnostics = null!;

    // ─── 框架辅助 ────────────────────────────────────────
    public DispatcherTimer? RenderTimer;
    public int RenderSeq;
    public bool SessionActive;
    public string RenderLastMode = "无";
    public Action? RunStartupProbe;

    public void Shutdown()
    {
        RenderTimer?.Stop();
        RenderTimer = null;
    }
}

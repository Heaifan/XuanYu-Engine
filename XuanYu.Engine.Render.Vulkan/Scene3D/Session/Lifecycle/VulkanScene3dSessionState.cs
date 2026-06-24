using FluidWarfare.Render.ViewportNavigation;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session.Lifecycle;

/// <summary>VulkanScene3dSession 运行时状态数据（状态/计数器/标记）。</summary>
sealed record VulkanScene3dSessionState
{
    public VulkanScene3dSessionStatus Status { get; set; } = VulkanScene3dSessionStatus.Inactive;
    public int FrameIndex { get; set; }
    public int InstanceCreateCount { get; set; }
    public int DeviceCreateCount { get; set; }
    public int PipelineCreateCount { get; set; }
    public int BufferCreateCount { get; set; }
    public int SwapchainGeneration { get; set; }
    public int ConsecutiveAcquireTimeouts { get; set; }
    public string? SelectedEntityId { get; set; }
    public bool Rendering { get; set; }
    public bool RecreateRequested { get; set; }

    // Overlay navigaion state
    public ViewportNavigationElement OverlayHovered { get; set; } = ViewportNavigationElement.None;
    public ViewportNavigationElement OverlayActive { get; set; } = ViewportNavigationElement.None;
    public int OverlayRevision { get; set; }

    // Success flags
    public bool InstOk { get; set; }
    public bool SurfOk { get; set; }
    public bool DevOk { get; set; }
    public bool VertModOk { get; set; }
    public bool FragModOk { get; set; }
    public bool LayoutOk { get; set; }
    public bool GridPipeOk { get; set; }
    public bool UnitPipeOk { get; set; }
    public bool GridBufOk { get; set; }
    public bool UnitBufOk { get; set; }
}

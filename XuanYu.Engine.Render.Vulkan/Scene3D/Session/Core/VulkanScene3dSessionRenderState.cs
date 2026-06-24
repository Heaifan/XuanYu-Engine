using FluidWarfare.Render.Vulkan.Camera;
using FluidWarfare.Render.Vulkan.Validation;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Swapchain + 运行时状态 + 验证相关字段。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    // ─── Swapchain 级资源 ───────────────────────────────────────
    private VulkanScene3dSwapchainResources? _swapchainRes;

    // ─── 运行时状态 ─────────────────────────────────────────────
    private VulkanScene3dSessionStatus _status = VulkanScene3dSessionStatus.Inactive;
    private int _frameIndex;
    private int _instanceCreateCount, _deviceCreateCount;
    private int _pipelineCreateCount, _bufferCreateCount;
    private int _swapchainGeneration;
    private int _consecutiveAcquireTimeouts;
    private string? _selectedEntityId;
    private bool _rendering;
    private bool _recreateRequested;
    private PresentedCameraSnapshot _lastPresentedSnapshot = PresentedCameraSnapshot.Empty;

    // Validation
    private readonly VulkanValidationOptions _validationOptions = VulkanValidationOptions.FromEnvironment();
    private readonly VulkanValidationMessageStore _validationMessageStore = new();
    private Validation.VulkanDebugMessengerScope? _debugMessengerScope;
}

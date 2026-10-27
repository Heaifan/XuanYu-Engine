using Silk.NET.Vulkan;
using XuanYu.Core.Diagnostics;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Pipeline;
using XuanYu.Render.Vulkan.Render;
using XuanYu.Render.Vulkan.Render.Readback;

namespace XuanYu.Render.Vulkan.Render.Terrain.Offscreen;

public sealed unsafe partial class VulkanOffscreenTerrainRenderer : IDisposable
{
    readonly Vk _vk;
    VulkanInstanceOwner? _instance;
    VulkanDeviceOwner? _device;
    VulkanOffscreenColorTarget? _target;
    VulkanClearFrameOwner? _frame;
    VulkanGraphicsPipelineOwner? _terrainPipeline;
    bool _disposed;

    public double? LastGpuRenderMilliseconds => _frame?.LastGpuRenderMilliseconds;
    public bool GpuTimestampSupported => _frame?.GpuTimestampSupported ?? false;
    public uint GpuTimestampValidBits => _frame?.GpuTimestampValidBits ?? 0;
    public double GpuTimestampPeriodNanoseconds => _frame?.GpuTimestampPeriodNanoseconds ?? 0;
    public double TerrainUploadMilliseconds => _frame?.TerrainUploadMilliseconds ?? 0;

    public VulkanOffscreenTerrainRenderer(RenderProjection projection, uint width, uint height,
        Action<string>? log = null)
        : this(projection, width, height, log, 256UL * 1024 * 1024)
    {
    }

    public VulkanOffscreenTerrainRenderer(RenderProjection projection, uint width, uint height,
        ulong terrainGpuCacheBudgetBytes)
        : this(projection, width, height, null, terrainGpuCacheBudgetBytes)
    {
    }

    public VulkanOffscreenTerrainRenderer(RenderProjection projection, uint width, uint height,
        Action<string>? log, ulong terrainGpuCacheBudgetBytes)
    {
        if (!projection.HasTerrain) throw new ArgumentException("RenderProjection 必须包含 Terrain", nameof(projection));
        _vk = Vk.GetApi();
        try
        {
            _instance = VulkanInstanceOwner.CreateOffscreen(_vk);
            var selection = VulkanPhysicalDeviceSelector.SelectOffscreen(_vk, _instance.Instance, log);
            if (!selection.Success) throw new InvalidOperationException(selection.Message);
            _device = VulkanDeviceOwner.CreateOffscreen(_vk, selection, log)
                ?? throw new InvalidOperationException("离屏 LogicalDevice 创建失败");
            _target = VulkanOffscreenColorTarget.Create(_vk, _device, width, height);
            _frame = new VulkanClearFrameOwner(_vk, _device, _target,
                selection.Queue!.GraphicsFamily, log);
            _frame.SetTerrainGpuCacheBudgetForProbe(terrainGpuCacheBudgetBytes);
            _frame.SetRenderProjection(projection);
            _terrainPipeline = VulkanTerrainPipelineFactory.Create(_vk, _device, _frame, log)
                ?? throw new InvalidOperationException("Terrain GraphicsPipeline 创建失败");
            _frame.SetPipeline(_terrainPipeline.Pipeline, _terrainPipeline.Layout);
            _frame.SetTerrainPipeline(_terrainPipeline.Pipeline, _terrainPipeline.Layout);
            log?.Invoke($"【VulkanOffscreen】Terrain RenderTarget 就绪；尺寸={width}x{height}；Format={_target.Format}；No Surface/Swapchain/Present");
        }
        catch { Dispose(); throw; }
    }

    public byte[] RenderRgba(CancellationToken cancellationToken = default) =>
        (_frame ?? throw new ObjectDisposedException(nameof(VulkanOffscreenTerrainRenderer)))
            .RenderOffscreenAndReadback(cancellationToken);

    public void UpdateProjection(RenderProjection projection)
    {
        if (_disposed || _frame is null) throw new ObjectDisposedException(nameof(VulkanOffscreenTerrainRenderer));
        if (!projection.HasTerrain) throw new ArgumentException("RenderProjection 必须包含 Terrain", nameof(projection));
        _frame.SetRenderProjection(projection);
    }

    public byte[] RenderPng(CancellationToken cancellationToken = default) => VulkanPngEncoder.EncodeRgba(
        checked((int)_target!.Extent.Width), checked((int)_target.Extent.Height), RenderRgba(cancellationToken));

    public string DeviceName
    {
        get
        {
            if (_device is null) throw new ObjectDisposedException(nameof(VulkanOffscreenTerrainRenderer));
            _vk.GetPhysicalDeviceProperties(_device.PhysicalDevice, out var properties);
            return System.Text.Encoding.UTF8.GetString(properties.DeviceName, 256).TrimEnd('\0');
        }
    }

    public uint DeviceDriverVersion
    {
        get
        {
            if (_device is null) throw new ObjectDisposedException(nameof(VulkanOffscreenTerrainRenderer));
            _vk.GetPhysicalDeviceProperties(_device.PhysicalDevice, out var properties);
            return properties.DriverVersion;
        }
    }

}

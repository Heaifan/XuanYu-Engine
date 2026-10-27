using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    bool FailClosedFrameTargets()
    {
        _commandBuffers = [];
        DestroyFramebuffers();
        foreach (var buffer in _readbackBuffers) buffer.Dispose();
        _readbackBuffers = [];
        _views = [];
        _extent = default;
        _framebufferGeneration++;
        return false;
    }
    void DestroyFramebuffers()
    {
        foreach (var f in _framebuffers)
            if (f.Handle != 0) _vk.DestroyFramebuffer(_deviceOwner.LogicalDevice, f, null);
        _framebuffers = [];
        foreach (var depth in _depthAttachments) depth?.Dispose();
        _depthAttachments = [];
    }

    internal void ReleaseSwapchainGeneration()
    {
        if (_commandBuffers.Length > 0)
            _vk.FreeCommandBuffers(_deviceOwner.LogicalDevice, _commandPool, (uint)_commandBuffers.Length, _commandBuffers);
        _commandBuffers = [];
        DestroyFramebuffers();
        _views = [];
    }
    public void Dispose()
    {
        if (_disposed) return;
        _mapLabelTextures?.EnsureUploadQuarantineSafe();
        _disposed = true;
        _resourceRetirement.Complete(0);
        _resourceRetirement.Complete(1);
        _proceduralVertexBuffer?.Dispose();
        _proceduralVertexBuffer = null;
        _mapSurface.Dispose();
        _staticModels.Dispose();
        _vectorOverlays.Dispose();
        foreach (var buffer in _readbackBuffers) buffer.Dispose();
        _readbackBuffers = [];
        DisposeMapLabels();
        DestroyFramebuffers();
        if (_commandBuffers.Length > 0)
            _vk.FreeCommandBuffers(_deviceOwner.LogicalDevice, _commandPool, (uint)_commandBuffers.Length, _commandBuffers);
        if (_commandPool.Handle != 0) _vk.DestroyCommandPool(_deviceOwner.LogicalDevice, _commandPool, null);
        _timestampQuery?.Dispose(); _timestampQuery = null;
        if (_renderPass.Handle != 0) _vk.DestroyRenderPass(_deviceOwner.LogicalDevice, _renderPass, null);
        Log(VulkanClearFrameLogFormatter.Disposed());
    }

    bool Ok(Result result, string op)
    {
        if (result == Result.Success) return true;
        Log(VulkanClearFrameLogFormatter.PresentError($"{op} 失败：{result}"));
        return false;
    }

    void Log(string message) => _log?.Invoke(message);
}

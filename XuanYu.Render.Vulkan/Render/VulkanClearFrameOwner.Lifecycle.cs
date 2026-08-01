using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void DestroyFramebuffers()
    {
        foreach (var f in _framebuffers)
            if (f.Handle != 0) _vk.DestroyFramebuffer(_deviceOwner.LogicalDevice, f, null);
        _framebuffers = [];
        _depthAttachment?.Dispose();
        _depthAttachment = null;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _proceduralVertexBuffer?.Dispose();
        _proceduralVertexBuffer = null;
        _staticModels.Dispose();
        DestroyFramebuffers();
        if (_commandBuffers.Length > 0)
            _vk.FreeCommandBuffers(_deviceOwner.LogicalDevice, _commandPool, (uint)_commandBuffers.Length, _commandBuffers);
        if (_commandPool.Handle != 0) _vk.DestroyCommandPool(_deviceOwner.LogicalDevice, _commandPool, null);
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

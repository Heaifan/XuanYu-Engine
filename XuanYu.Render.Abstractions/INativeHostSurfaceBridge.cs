using System;

namespace XuanYu.Render.Abstractions;

// NativeHost 生命周期到 Surface 生命周期的交接契约。
// 由组合根（Editor.Win）实现，VK3-C 接 VulkanSurfaceOwner；
// VK3-A 阶段只定义，不创建 Surface、不引用 Silk.NET。
public interface INativeHostSurfaceBridge : IDisposable
{
    void Attach(NativeHostSurfaceHandle handle);
    void Resize(int width, int height);
    void Detach();
}

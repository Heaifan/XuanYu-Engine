namespace XuanYu.Render.Abstractions;

// VK3-A-R1：从 XuanYu.Render.Vulkan 迁入的纯生命周期状态枚举。
// 不含任何 Vulkan / Silk.NET 依赖，仅描述 NativeHost 生命周期阶段。
public enum NativeHostLifecycleState
{
    Created,
    Attached,
    HandleAvailable,
    Resized,
    Detached,
    Disposed,
    Invalidated
}

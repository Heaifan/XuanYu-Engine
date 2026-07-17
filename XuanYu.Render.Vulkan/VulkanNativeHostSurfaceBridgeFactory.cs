using System;
using XuanYu.Core.Scene;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan;

// ARCH-A-R1：Vulkan 侧开始适配抽象装配契约。
public sealed class VulkanNativeHostSurfaceBridgeFactory : INativeHostSurfaceBridgeFactory
{
    public INativeHostSurfaceBridge Create(Action<string>? log = null, ISceneRenderSnapshotSource? sceneSource = null) =>
        new VulkanNativeHostSurfaceBridge(log, sceneSource);
}

using System;
using XuanYu.Core.Scene;

namespace XuanYu.Render.Abstractions;

// ARCH-A-R1：NativeHost 渲染桥的最小装配契约。
// UI 后续只接收该工厂，不直接认识具体 Vulkan 实现。
public interface INativeHostSurfaceBridgeFactory
{
    INativeHostSurfaceBridge Create(Action<string>? log = null, ISceneRenderSnapshotSource? sceneSource = null);
}

using FluidWarfare.Render.Vulkan.Shaders;

namespace FluidWarfare.Tests.Render.Vulkan.Shaders;

/// <summary>
/// 确保修改 CompiledShaders 静态状态的测试串行执行。
/// 防止 VulkanScene3dRunGateTests 和 CompiledShadersTests 并行冲突。
/// </summary>
[CollectionDefinition("CompiledShadersState", DisableParallelization = true)]
public sealed class CompiledShadersCollection
{
}

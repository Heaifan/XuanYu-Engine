namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// 会话级资源创建标记。
/// 每个标记对应一个 Vulkan Handle，标记为 true 表示需要释放。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private bool _instOk, _surfOk, _devOk;
    private bool _vertModOk, _fragModOk, _layoutOk;
    private bool _gridPipeOk, _unitPipeOk;
    private bool _gridBufOk, _unitBufOk;
}

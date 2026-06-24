namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>
/// ShaderModule 释放步骤。
/// 仅在 _ok 标记有效时释放相应资源并清除标记。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private void DisposeFragmentShaderStep()
    {
        if (_fragModOk) { _vk!.DestroyShaderModule(_device, _fragModule, null); _fragModOk = false; }
    }

    private void DisposeVertexShaderStep()
    {
        if (_vertModOk) { _vk!.DestroyShaderModule(_device, _vertModule, null); _vertModOk = false; }
    }
}

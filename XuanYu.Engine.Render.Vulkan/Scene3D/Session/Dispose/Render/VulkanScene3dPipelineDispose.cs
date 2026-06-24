namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Pipeline + PipelineLayout 释放步骤。
/// 仅在 _ok 标记有效时释放相应资源并清除标记。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private void DisposeUnitPipelineStep()
    {
        if (_unitPipeOk) { _vk!.DestroyPipeline(_device, _unitPipeline, null); _unitPipeOk = false; }
    }

    private void DisposeGridPipelineStep()
    {
        if (_gridPipeOk) { _vk!.DestroyPipeline(_device, _gridPipeline, null); _gridPipeOk = false; }
    }

    private void DisposePipelineLayoutStep()
    {
        if (_layoutOk) { _vk!.DestroyPipelineLayout(_device, _pipelineLayout, null); _layoutOk = false; }
    }
}

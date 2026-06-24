using FluidWarfare.Render.Vulkan.Scene3D.GroundCursor;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Shader / PipelineLayout / VertexBuffer / GroundCursor 创建步骤。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private bool CreateShaderModules()
    {
        if (!VulkanScene3dShaderModules.Create(_vk!, _device,
                out _vertModule, out _fragModule, out var err))
            return false;
        _vertModOk = true;
        _fragModOk = true;
        return true;
    }

    private bool CreatePipelineLayout()
    {
        if (!VulkanScene3dPipelineLayout.Create(_vk!, _device, _physicalDevice,
                out _pipelineLayout, out var err))
            return false;
        _layoutOk = true;
        return true;
    }

    private bool CreateVertexBuffers(
        ReadOnlySpan<VulkanScene3dVertex> gridVertices,
        ReadOnlySpan<VulkanScene3dVertex> unitVertices)
    {
        if (!VulkanScene3dVertexBuffers.Create(_vk!, _physicalDevice, _device,
                gridVertices, unitVertices,
                out _gridBuffer, out _gridMemory,
                out _unitBuffer, out _unitMemory,
                out _gridVertexCount, out _unitVertexCount, out var err))
            return false;
        _gridBufOk = true;
        _unitBufOk = true;
        _bufferCreateCount++;
        return true;
    }

    private bool CreateGroundCursorBuffer()
    {
        var verts = VulkanGroundCursorGeometry.Create();
        if (!VulkanScene3dVertexBuffers.CreateCursor(_vk!, _physicalDevice, _device,
                verts, out _cursorBuffer, out _cursorMemory, out _cursorVertexCount, out var err))
            return false;
        _cursorBufOk = true;
        _bufferCreateCount++;
        return true;
    }
}

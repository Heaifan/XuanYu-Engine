namespace XuanYu.Render.Vulkan.Pipeline;

internal sealed unsafe partial class VulkanGraphicsPipelineOwner
{
    static uint[] VertexShaderCode(bool analyticStroke) => analyticStroke
        ? ShaderBytecodeEditorVectorOverlayVert.Code : ShaderBytecodeVert.Code;

    static uint[] FragmentShaderCode(bool analyticStroke) => analyticStroke
        ? ShaderBytecodeEditorVectorOverlayFrag.Code : ShaderBytecodeFrag.Code;
}

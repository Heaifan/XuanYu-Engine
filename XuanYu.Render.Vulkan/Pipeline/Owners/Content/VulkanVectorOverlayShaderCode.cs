namespace XuanYu.Render.Vulkan.Pipeline;

internal static class VulkanVectorOverlayShaderCode
{
    internal static uint[] Vertex(bool analyticStroke) => analyticStroke
        ? ShaderBytecodeEditorVectorOverlayVert.Code : ShaderBytecodeVert.Code;

    internal static uint[] Fragment(bool analyticStroke) => analyticStroke
        ? ShaderBytecodeEditorVectorOverlayFrag.Code : ShaderBytecodeFrag.Code;
}

using XuanYu.Engine.Render.Vulkan.Shaders;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>
/// 使用 CompiledShaders 中已验证的 SPIR-V 字节创建 Vertex / Fragment ShaderModule。
/// 不读取文件系统，不调用 shader 编译工具。
/// </summary>
public static unsafe class VulkanScene3dShaderModules
{
    /// <summary>
    /// 创建顶点和片段着色器模块。
    /// </summary>
    public static bool Create(Vk vk, Silk.NET.Vulkan.Device dev,
        out ShaderModule vertexModule, out ShaderModule fragmentModule,
        out string errorMessage)
    {
        vertexModule = default;
        fragmentModule = default;
        errorMessage = string.Empty;

        if (!CompiledShaders.HasValidatedBasic3dShaders)
        {
            errorMessage = "Scene3D ShaderModule：SPIR-V 未通过 spirv-val 验证。";
            return false;
        }

        if (CompiledShaders.Basic3dVert.Length == 0)
        {
            errorMessage = "Scene3D ShaderModule：basic_3d.vert.spv 字节为空。";
            return false;
        }

        if (CompiledShaders.Basic3dFrag.Length == 0)
        {
            errorMessage = "Scene3D ShaderModule：basic_3d.frag.spv 字节为空。";
            return false;
        }

        vertexModule = CreateOne(vk, dev, CompiledShaders.Basic3dVert);
        if (vertexModule.Handle == 0)
        {
            errorMessage = "Scene3D ShaderModule：vkCreateShaderModule 失败（Vertex）。";
            return false;
        }

        fragmentModule = CreateOne(vk, dev, CompiledShaders.Basic3dFrag);
        if (fragmentModule.Handle == 0)
        {
            vk.DestroyShaderModule(dev, vertexModule, null);
            vertexModule = default;
            errorMessage = "Scene3D ShaderModule：vkCreateShaderModule 失败（Fragment）。";
            return false;
        }

        return true;
    }

    private static ShaderModule CreateOne(Vk vk, Silk.NET.Vulkan.Device dev, uint[] spirv)
    {
        fixed (uint* code = spirv)
        {
            var ci = new ShaderModuleCreateInfo
            {
                SType = StructureType.ShaderModuleCreateInfo,
                CodeSize = (nuint)(spirv.Length * sizeof(uint)),
                PCode = code
            };
            return vk.CreateShaderModule(dev, &ci, null, out var module) == Result.Success
                ? module : default;
        }
    }
}

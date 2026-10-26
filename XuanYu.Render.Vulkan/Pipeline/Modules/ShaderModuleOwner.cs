using System;
using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan.Device;

namespace XuanYu.Render.Vulkan.Pipeline;

// VK5-A：ShaderModule 创建助手。创建后由 GraphicsPipelineOwner 在管道建好后立即释放（短生命周期，不持有到会话结束）。
internal static unsafe class VulkanShaderModuleOwner
{
    internal static ShaderModule Create(Vk vk, VulkanDeviceOwner deviceOwner, uint[] code)
    {
        fixed (uint* pCode = code)
        {
            var info = new ShaderModuleCreateInfo
            {
                SType = StructureType.ShaderModuleCreateInfo,
                CodeSize = (nuint)(code.Length * 4),
                PCode = pCode,
            };
            var result = vk.CreateShaderModule(deviceOwner.LogicalDevice, &info, null, out var module);
            if (result != Result.Success) return default;
            return module;
        }
    }

    internal static void Destroy(Vk vk, VulkanDeviceOwner deviceOwner, ShaderModule module)
    {
        if (module.Handle != 0) vk.DestroyShaderModule(deviceOwner.LogicalDevice, module, null);
    }
}

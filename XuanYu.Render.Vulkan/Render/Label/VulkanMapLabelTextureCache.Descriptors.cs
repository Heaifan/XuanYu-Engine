using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render.Label;

sealed unsafe partial class VulkanMapLabelTextureCache
{
    DescriptorSetLayout CreateDescriptorSetLayout()
    {
        var binding = new DescriptorSetLayoutBinding { Binding = 0, DescriptorCount = 1,
            DescriptorType = DescriptorType.CombinedImageSampler, StageFlags = ShaderStageFlags.FragmentBit };
        var info = new DescriptorSetLayoutCreateInfo { SType = StructureType.DescriptorSetLayoutCreateInfo,
            BindingCount = 1, PBindings = &binding };
        Check(_vk.CreateDescriptorSetLayout(_device.LogicalDevice, &info, null, out var layout), "CreateLabelSetLayout");
        return layout;
    }

    DescriptorPool CreateDescriptorPool()
    {
        var size = new DescriptorPoolSize { Type = DescriptorType.CombinedImageSampler, DescriptorCount = 256 };
        var info = new DescriptorPoolCreateInfo { SType = StructureType.DescriptorPoolCreateInfo,
            MaxSets = 256, PoolSizeCount = 1, PPoolSizes = &size };
        Check(_vk.CreateDescriptorPool(_device.LogicalDevice, &info, null, out var pool), "CreateLabelDescriptorPool");
        return pool;
    }

    Sampler CreateSampler()
    {
        var info = new SamplerCreateInfo { SType = StructureType.SamplerCreateInfo,
            MagFilter = Filter.Linear, MinFilter = Filter.Linear, MipmapMode = SamplerMipmapMode.Linear,
            AddressModeU = SamplerAddressMode.ClampToEdge, AddressModeV = SamplerAddressMode.ClampToEdge,
            AddressModeW = SamplerAddressMode.ClampToEdge, MaxLod = 1 };
        Check(_vk.CreateSampler(_device.LogicalDevice, &info, null, out var sampler), "CreateLabelSampler");
        return sampler;
    }
}

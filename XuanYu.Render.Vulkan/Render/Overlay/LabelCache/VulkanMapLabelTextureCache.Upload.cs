using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render.Label;

sealed unsafe partial class VulkanMapLabelTextureCache
{
    void Upload(Silk.NET.Vulkan.Buffer staging, Image image, uint width, uint height, uint rowLength)
    {
        var allocate = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = _commandPool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
        Check(_vk.AllocateCommandBuffers(_device.LogicalDevice, &allocate, out var command), "AllocateLabelCommand");
        try
        {
            var begin = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo,
                Flags = CommandBufferUsageFlags.OneTimeSubmitBit };
            Check(_vk.BeginCommandBuffer(command, &begin), "BeginLabelCommand");
            Transition(command, image, ImageLayout.Undefined, ImageLayout.TransferDstOptimal,
                PipelineStageFlags.TopOfPipeBit, PipelineStageFlags.TransferBit);
            var copy = new BufferImageCopy { ImageSubresource = new ImageSubresourceLayers
                { AspectMask = ImageAspectFlags.ColorBit, LayerCount = 1 }, BufferRowLength = rowLength,
                BufferImageHeight = height, ImageExtent = new(width, height, 1) };
            _vk.CmdCopyBufferToImage(command, staging, image, ImageLayout.TransferDstOptimal, 1, &copy);
            Transition(command, image, ImageLayout.TransferDstOptimal, ImageLayout.ShaderReadOnlyOptimal,
                PipelineStageFlags.TransferBit, PipelineStageFlags.FragmentShaderBit);
            Check(_vk.EndCommandBuffer(command), "EndLabelCommand");
            var submit = new SubmitInfo { SType = StructureType.SubmitInfo, CommandBufferCount = 1, PCommandBuffers = &command };
            Check(_vk.QueueSubmit(_queue, 1, &submit, default), "SubmitLabelCommand");
            Check(_vk.QueueWaitIdle(_queue), "WaitLabelCommand");
        }
        finally { _vk.FreeCommandBuffers(_device.LogicalDevice, _commandPool, 1, in command); }
    }

    void Transition(CommandBuffer command, Image image, ImageLayout oldLayout, ImageLayout newLayout,
        PipelineStageFlags sourceStage, PipelineStageFlags destinationStage)
    {
        var barrier = new ImageMemoryBarrier { SType = StructureType.ImageMemoryBarrier,
            OldLayout = oldLayout, NewLayout = newLayout, SrcQueueFamilyIndex = Vk.QueueFamilyIgnored,
            DstQueueFamilyIndex = Vk.QueueFamilyIgnored, Image = image,
            SubresourceRange = new ImageSubresourceRange { AspectMask = ImageAspectFlags.ColorBit, LevelCount = 1, LayerCount = 1 } };
        if (newLayout == ImageLayout.TransferDstOptimal) barrier.DstAccessMask = AccessFlags.TransferWriteBit;
        else { barrier.SrcAccessMask = AccessFlags.TransferWriteBit; barrier.DstAccessMask = AccessFlags.ShaderReadBit; }
        _vk.CmdPipelineBarrier(command, sourceStage, destinationStage, DependencyFlags.None,
            0, null, 0, null, 1, &barrier);
    }

    DescriptorSet AllocateDescriptor(ImageView view)
    {
        var layout = DescriptorSetLayout;
        var allocate = new DescriptorSetAllocateInfo { SType = StructureType.DescriptorSetAllocateInfo,
            DescriptorPool = _descriptorPool, DescriptorSetCount = 1, PSetLayouts = &layout };
        Check(_vk.AllocateDescriptorSets(_device.LogicalDevice, &allocate, out var descriptor), "AllocateLabelDescriptor");
        var image = new DescriptorImageInfo { Sampler = Sampler, ImageView = view, ImageLayout = ImageLayout.ShaderReadOnlyOptimal };
        var write = new WriteDescriptorSet { SType = StructureType.WriteDescriptorSet, DstSet = descriptor,
            DstBinding = 0, DescriptorCount = 1, DescriptorType = DescriptorType.CombinedImageSampler, PImageInfo = &image };
        _vk.UpdateDescriptorSets(_device.LogicalDevice, 1, &write, 0, null);
        return descriptor;
    }
}

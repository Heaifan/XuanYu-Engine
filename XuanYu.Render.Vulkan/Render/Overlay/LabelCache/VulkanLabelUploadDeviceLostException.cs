namespace XuanYu.Render.Vulkan.Render.Label;

sealed class VulkanLabelUploadDeviceLostException(VulkanLabelUploadResolution result)
    : InvalidOperationException(
        $"Label upload terminated because Vulkan device was lost: Submit={result.Submit}; " +
        $"QueueWait={result.QueueWait?.ToString() ?? "not called"}; " +
        $"DeviceWait={result.DeviceWait?.ToString() ?? "not called"}.")
{
}

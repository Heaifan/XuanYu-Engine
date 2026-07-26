namespace XuanYu.Render.Vulkan.Pipeline;

internal static class VulkanScenePushConstants
{
    // mat4(64) + vec4(16) + float gizmoMode(4) = 84，按 16 字节对齐补齐到 96。
    public const uint SizeInBytes = 96;
}

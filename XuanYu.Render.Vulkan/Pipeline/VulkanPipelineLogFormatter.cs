namespace XuanYu.Render.Vulkan.Pipeline;

// VK5-A：GraphicsPipeline 资源中文日志格式器。仅生成字符串，经注入的 Action<string> log 回调输出（日志单出口）。
internal static class VulkanPipelineLogFormatter
{
    internal static string ShaderModuleCreated() => "ShaderModule 创建成功（vert+frag）";
    internal static string PipelineLayoutCreated() => "PipelineLayout 创建成功";
    internal static string GraphicsPipelineCreated() => "GraphicsPipeline 创建成功";
    internal static string SkyCreated() => "天空 GraphicsPipeline 创建成功（深度不写）";
    internal static string Created() => "GraphicsPipeline 资源创建完成";
    internal static string Disposed() => "GraphicsPipeline 资源释放完成";
    internal static string Skipped(string reason) => $"Pipeline 跳过：{reason}";
    internal static string Failed(string reason) => $"Pipeline 创建失败：{reason}";
}

using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D.Depth;

/// <summary>
/// 深度附件查询与诊断信息。
/// 不持有 GPU 资源，仅用于传递格式选择结果和诊断文本。
/// </summary>
public sealed record VulkanScene3dDepthAttachmentInfo(
    bool IsSupported,
    Format ChosenFormat,
    bool HasStencil,
    int AttachmentCount,
    string Message)
{
    /// <summary>
    /// 创建不支持状态。
    /// </summary>
    public static VulkanScene3dDepthAttachmentInfo Unsupported(string message)
    {
        return new VulkanScene3dDepthAttachmentInfo(false, Format.Undefined, false, 0, message);
    }

    /// <summary>
    /// 返回格式的简短诊断名称。
    /// </summary>
    public string FormatName => ChosenFormat switch
    {
        Format.D32Sfloat => "D32Sfloat",
        Format.D32SfloatS8Uint => "D32SfloatS8Uint",
        Format.D24UnormS8Uint => "D24UnormS8Uint",
        _ => ChosenFormat.ToString()
    };
}

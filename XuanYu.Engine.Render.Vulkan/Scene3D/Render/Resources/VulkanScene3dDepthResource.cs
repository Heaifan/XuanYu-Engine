using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>Depth 资源字段。按依赖逆序释放（ImageView → Image → Memory）。</summary>
unsafe partial class VulkanScene3dRenderResources
{
    public Image[] DepthImages = [];
    public DeviceMemory[] DepthMemories = [];
    public ImageView[] DepthViews = [];
    public Format DepthFormat;
    public int DepthAttachmentCount;
    public bool DepthOk;
}

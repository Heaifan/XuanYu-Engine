using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session.Swapchain.Images;

/// <summary>RenderPass + Framebuffer 创建。</summary>
internal static unsafe class VulkanScene3dSwapchainFramebuffers
{
    public static RenderPass CreateRenderPass(Vk vk, Silk.NET.Vulkan.Device device,
        Format colorFormat, Format depthFormat)
    {
        var colorAtt = new AttachmentDescription
        {
            Format = colorFormat, Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.Store,
            StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.Undefined, FinalLayout = ImageLayout.PresentSrcKhr
        };
        var depthAtt = new AttachmentDescription
        {
            Format = depthFormat, Samples = SampleCountFlags.Count1Bit,
            LoadOp = AttachmentLoadOp.Clear, StoreOp = AttachmentStoreOp.DontCare,
            StencilLoadOp = AttachmentLoadOp.DontCare, StencilStoreOp = AttachmentStoreOp.DontCare,
            InitialLayout = ImageLayout.Undefined, FinalLayout = ImageLayout.DepthStencilAttachmentOptimal
        };
        var atts = stackalloc[] { colorAtt, depthAtt };
        var colorRef = new AttachmentReference { Attachment = 0, Layout = ImageLayout.ColorAttachmentOptimal };
        var depthRef = new AttachmentReference { Attachment = 1, Layout = ImageLayout.DepthStencilAttachmentOptimal };
        var subpass = new SubpassDescription
        {
            PipelineBindPoint = PipelineBindPoint.Graphics,
            ColorAttachmentCount = 1, PColorAttachments = &colorRef,
            PDepthStencilAttachment = &depthRef
        };
        var rpCI = new RenderPassCreateInfo
        {
            SType = StructureType.RenderPassCreateInfo,
            AttachmentCount = 2, PAttachments = atts,
            SubpassCount = 1, PSubpasses = &subpass
        };
        RenderPass rp = default;
        vk.CreateRenderPass(device, &rpCI, null, out rp);
        return rp;
    }

    public static Framebuffer[] CreateFramebuffers(Vk vk, Silk.NET.Vulkan.Device device,
        RenderPass renderPass, ImageView[] colorViews, ImageView[] depthViews,
        Extent2D extent)
    {
        var count = colorViews.Length;
        var framebuffers = new Framebuffer[count];
        var fba = stackalloc ImageView[2];
        for (var i = 0; i < count; i++)
        {
            fba[0] = colorViews[i];
            fba[1] = depthViews[i];
            var ci = new FramebufferCreateInfo
            {
                SType = StructureType.FramebufferCreateInfo,
                RenderPass = renderPass, AttachmentCount = 2, PAttachments = fba,
                Width = extent.Width, Height = extent.Height, Layers = 1
            };
            Framebuffer fb = default;
            if (vk.CreateFramebuffer(device, &ci, null, out fb) != Result.Success)
            {
                for (var j = 0; j < i; j++) vk.DestroyFramebuffer(device, framebuffers[j], null);
                return [];
            }
            framebuffers[i] = fb;
        }
        return framebuffers;
    }
}

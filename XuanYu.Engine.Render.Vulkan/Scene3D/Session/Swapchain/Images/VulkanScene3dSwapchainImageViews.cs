using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session.Swapchain.Images;

/// <summary>Swapchain Color ImageView 创建。</summary>
internal static unsafe class VulkanScene3dSwapchainImageViews
{
    public static ImageView[] Create(Vk vk, Silk.NET.Vulkan.Device device,
        Image[] swapchainImages, Format format)
    {
        var count = swapchainImages.Length;
        var views = new ImageView[count];
        for (var i = 0; i < count; i++)
        {
            var ci = new ImageViewCreateInfo
            {
                SType = StructureType.ImageViewCreateInfo,
                Image = swapchainImages[i],
                ViewType = ImageViewType.Type2D,
                Format = format,
                Components = new ComponentMapping
                {
                    R = ComponentSwizzle.Identity, G = ComponentSwizzle.Identity,
                    B = ComponentSwizzle.Identity, A = ComponentSwizzle.Identity
                },
                SubresourceRange = new ImageSubresourceRange
                {
                    AspectMask = ImageAspectFlags.ColorBit,
                    BaseMipLevel = 0, LevelCount = 1,
                    BaseArrayLayer = 0, LayerCount = 1
                }
            };
            ImageView newCv = default;
            if (vk.CreateImageView(device, &ci, null, out newCv) != Result.Success)
            {
                for (var j = 0; j < i; j++) vk.DestroyImageView(device, views[j], null);
                return [];
            }
            views[i] = newCv;
        }
        return views;
    }
}

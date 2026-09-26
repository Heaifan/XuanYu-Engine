using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render.Label;

sealed unsafe partial class VulkanMapLabelTextureCache : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _device; readonly CommandPool _commandPool;
    readonly Queue _queue; readonly Action<string>? _log;
    readonly Dictionary<string, VulkanMapLabelTexture> _items = [];
    readonly DescriptorPool _descriptorPool;
    public DescriptorSetLayout DescriptorSetLayout { get; }
    public Sampler Sampler { get; }

    public VulkanMapLabelTextureCache(Vk vk, VulkanDeviceOwner device, CommandPool pool,
        Action<string>? log) : this(vk, device, pool, device.GraphicsQueue, log)
    {
    }

    VulkanMapLabelTextureCache(Vk vk, VulkanDeviceOwner device, CommandPool pool,
        Queue queue, Action<string>? log)
    {
        _vk = vk; _device = device; _commandPool = pool; _queue = queue; _log = log;
        DescriptorSetLayout = CreateDescriptorSetLayout();
        _descriptorPool = CreateDescriptorPool();
        Sampler = CreateSampler();
    }

    public VulkanMapLabelTexture? GetOrCreate(RenderLabelBitmap bitmap)
    {
        if (_items.TryGetValue(bitmap.CacheKey, out var old)) return old;
        try
        {
            var next = CreateTexture(bitmap);
            _items[bitmap.CacheKey] = next;
            return next;
        }
        catch (Exception error)
        {
            _log?.Invoke($"Map Label Texture 创建失败：{error.Message}");
            return null;
        }
    }

    public void RetainOnly(IEnumerable<string> keys)
    {
        var keep = keys.ToHashSet(StringComparer.Ordinal);
        foreach (var key in _items.Keys.Where(x => !keep.Contains(x)).ToArray())
        { _items[key].Dispose(); _items.Remove(key); }
    }

    public void Dispose()
    {
        foreach (var item in _items.Values) item.Dispose();
        _items.Clear();
        if (Sampler.Handle != 0) _vk.DestroySampler(_device.LogicalDevice, Sampler, null);
        if (_descriptorPool.Handle != 0) _vk.DestroyDescriptorPool(_device.LogicalDevice, _descriptorPool, null);
        if (DescriptorSetLayout.Handle != 0) _vk.DestroyDescriptorSetLayout(_device.LogicalDevice, DescriptorSetLayout, null);
    }
}

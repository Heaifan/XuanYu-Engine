using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render.StaticModels;

namespace XuanYu.Render.Vulkan.Render.Label;

sealed unsafe partial class VulkanMapLabelTextureCache : IDisposable
{
    readonly Vk _vk; readonly VulkanDeviceOwner _device; readonly CommandPool _commandPool;
    readonly Queue _queue; readonly Action<string>? _log;
    readonly Action<IDisposable>? _retire;
    readonly Dictionary<string, VulkanMapLabelTexture> _items = [];
    readonly List<VulkanMapLabelUploadQuarantine> _uploadQuarantines = [];
    readonly DescriptorPool _descriptorPool;
    public DescriptorSetLayout DescriptorSetLayout { get; }
    public Sampler Sampler { get; }

    public VulkanMapLabelTextureCache(Vk vk, VulkanDeviceOwner device, CommandPool pool,
        Action<string>? log, Action<IDisposable>? retire = null)
        : this(vk, device, pool, device.GraphicsQueue, log, retire)
    {
    }

    VulkanMapLabelTextureCache(Vk vk, VulkanDeviceOwner device, CommandPool pool,
        Queue queue, Action<string>? log, Action<IDisposable>? retire)
    {
        _vk = vk; _device = device; _commandPool = pool; _queue = queue; _log = log; _retire = retire;
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
        catch (VulkanLabelUploadCompletionUnknownException)
        {
            throw;
        }
        catch (VulkanLabelUploadDeviceLostException error)
        {
            _log?.Invoke($"Map Label Texture 上传进入 DeviceLost 终态：{error.Message}");
            throw;
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
        { Retire(_items[key]); _items.Remove(key); }
    }

    public void Dispose()
    {
        EnsureUploadQuarantineSafe();
        foreach (var item in _items.Values) Retire(item);
        _items.Clear();
        if (Sampler.Handle != 0) _vk.DestroySampler(_device.LogicalDevice, Sampler, null);
        if (_descriptorPool.Handle != 0) _vk.DestroyDescriptorPool(_device.LogicalDevice, _descriptorPool, null);
        if (DescriptorSetLayout.Handle != 0) _vk.DestroyDescriptorSetLayout(_device.LogicalDevice, DescriptorSetLayout, null);
    }

    void Retire(IDisposable resource)
    { if (_retire is null) resource.Dispose(); else _retire(resource); }

    internal void EnsureUploadQuarantineSafe()
    {
        if (_uploadQuarantines.Count == 0) return;
        var upload = _uploadQuarantines[0];
        throw new VulkanLabelUploadCompletionUnknownException(upload.Command, upload.Resolution);
    }
}

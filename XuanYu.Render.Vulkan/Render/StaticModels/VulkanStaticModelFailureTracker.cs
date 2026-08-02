using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render.StaticModels;

// D3-F1：静态模型 GPU 资源创建失败去重。
// 相同 Key + Revision 已失败：不重复创建、不重复输出错误日志；
// Revision 改变或重新导入后允许重新尝试。纯逻辑，不依赖 Vulkan 设备，可单测。
sealed class VulkanStaticModelFailureTracker
{
    readonly Dictionary<RenderStaticModelKey, int> _failed = [];

    public bool ShouldSkip(RenderStaticModelKey key, int revision) =>
        _failed.TryGetValue(key, out var failed) && failed == revision;

    public void Record(RenderStaticModelKey key, int revision) => _failed[key] = revision;

    public void Clear(RenderStaticModelKey key) => _failed.Remove(key);

    public void ClearNotIn(IEnumerable<RenderStaticModelKey> keys)
    {
        var keep = keys.Where(k => k.IsValid).ToHashSet();
        foreach (var key in _failed.Keys.Where(k => !keep.Contains(k)).ToArray())
        {
            _failed.Remove(key);
        }
    }

    public int Count => _failed.Count;
}

namespace FluidWarfare.Render.Vulkan.Validation;

/// <summary>
/// 保存最近 N 条 Vulkan Validation 消息。
/// 避免 Validation 日志刷爆内存。
/// </summary>
public sealed class VulkanValidationMessageStore
{
    private const int MaxMessages = 20;
    private readonly List<VulkanValidationMessageInfo> _messages = [];

    public void Add(VulkanValidationMessageInfo message)
    {
        lock (_messages)
        {
            _messages.Add(message);
            if (_messages.Count > MaxMessages)
                _messages.RemoveRange(0, _messages.Count - MaxMessages);
        }
    }

    public IReadOnlyList<VulkanValidationMessageInfo> Snapshot()
    {
        lock (_messages)
        {
            return [.. _messages];
        }
    }

    public int Count
    {
        get { lock (_messages) { return _messages.Count; } }
    }

    public void Clear()
    {
        lock (_messages) { _messages.Clear(); }
    }
}

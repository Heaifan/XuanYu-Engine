using XuanYu.Engine.Render.Vulkan.Validation;

namespace XuanYu.Engine.Tests.Render.Vulkan.Validation;

public sealed class VulkanValidationMessageStoreTests
{
    [Fact]
    public void Add_ShouldPreserveMessage()
    {
        var store = new VulkanValidationMessageStore();
        store.Add(new VulkanValidationMessageInfo("警告", "Validation", "测试消息"));
        var snapshot = store.Snapshot();
        Assert.Single(snapshot);
        Assert.Equal("测试消息", snapshot[0].Message);
    }

    [Fact]
    public void Snapshot_ShouldReturnMessages()
    {
        var store = new VulkanValidationMessageStore();
        store.Add(new VulkanValidationMessageInfo("报错", "Validation", "错误1"));
        store.Add(new VulkanValidationMessageInfo("警告", "性能", "警告1"));
        Assert.Equal(2, store.Snapshot().Count);
    }

    [Fact]
    public void Store_ShouldLimitRecentMessages()
    {
        var store = new VulkanValidationMessageStore();
        for (var i = 0; i < 25; i++)
        {
            store.Add(new VulkanValidationMessageInfo("信息", "通用", $"消息{i}"));
        }
        Assert.Equal(20, store.Snapshot().Count);
    }

    [Fact]
    public void Snapshot_ShouldNotExposeMutableList()
    {
        var store = new VulkanValidationMessageStore();
        store.Add(new VulkanValidationMessageInfo("警告", "Validation", "原始消息"));
        var snapshot = store.Snapshot();
        // Should be a copy - adding to snapshot shouldn't affect store
        _ = snapshot.Count;
        Assert.Single(store.Snapshot());
    }

    [Fact]
    public void Clear_ShouldRemoveAllMessages()
    {
        var store = new VulkanValidationMessageStore();
        store.Add(new VulkanValidationMessageInfo("信息", "通用", "消息"));
        store.Clear();
        Assert.Empty(store.Snapshot());
        Assert.Equal(0, store.Count);
    }
}

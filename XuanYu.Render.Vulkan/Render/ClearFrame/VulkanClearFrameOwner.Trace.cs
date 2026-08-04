namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void TraceRecordCommands(int viewCount)
    {
        _recordCommandTraceCount++;
        var entityCount = _hasRenderProjection ? _renderProjection.EntityCount : 0;
        if (_recordCommandTraceCount != 1 &&
            entityCount == _lastLoggedCommandEntityCount &&
            viewCount == _lastLoggedCommandViewCount &&
            _recordCommandTraceCount % 100 != 0) return;
        _lastLoggedCommandEntityCount = entityCount;
        _lastLoggedCommandViewCount = viewCount;
        var time = DateTime.Now.ToString("HH:mm:ss");
        Console.Error.WriteLine(
            $"{time} 【调试】【命令缓冲】命令缓冲录制摘要；次数={_recordCommandTraceCount}；线程编号={Environment.CurrentManagedThreadId}；实体数={entityCount}；视图数={viewCount}");
    }
}

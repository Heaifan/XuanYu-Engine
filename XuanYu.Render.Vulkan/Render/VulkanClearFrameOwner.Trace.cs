namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void TraceRecordCommands(string stage, int depth, int viewCount)
    {
        if (depth <= 3 || depth % 10 == 0)
        {
            var time = DateTime.Now.ToString("HH:mm:ss");
            Console.Error.WriteLine(
                $"{time} 【调试】【命令缓冲】命令缓冲录制诊断；阶段={stage}；深度={depth}；线程编号={Environment.CurrentManagedThreadId}；实体数={_sceneSnapshot.Entities.Count}；视图数={viewCount}");
        }
    }
}

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void TraceRecordCommands(string stage, int depth, int viewCount)
    {
        if (depth <= 3 || depth % 10 == 0)
        {
            Console.Error.WriteLine(
                $"[DIAG Vulkan] {stage}; Depth={depth}; ThreadId={Environment.CurrentManagedThreadId}; EntityCount={_sceneSnapshot.Entities.Count}; ViewCount={viewCount}");
        }
    }
}

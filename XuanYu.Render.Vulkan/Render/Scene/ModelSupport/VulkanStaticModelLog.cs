using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render.StaticModels;

static class VulkanStaticModelLog
{
    public static string Created(RenderStaticModelResource m) =>
        $"【VulkanStaticModel】静态模型 GPU 资源创建完成；Resource={m.Key}；Vertices={m.Vertices.Count}；Indices={m.Indices.Count}；Primitives={m.Primitives.Count}";

    public static string Disposed(RenderStaticModelKey key) =>
        $"【VulkanStaticModel】静态模型 GPU 资源释放完成；Resource={key}";

    public static string Failed(RenderStaticModelKey key, string stage, string reason) =>
        $"【VulkanStaticModel】静态模型 GPU 资源创建失败；Resource={key}；Stage={stage}；Reason={reason}";
}

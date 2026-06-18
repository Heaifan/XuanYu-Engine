using FluidWarfare.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Viewport.Transform.Application;

/// <summary>Scene3D 帧请求能力。包装 ScheduleScene3dFrame 调用。</summary>
public sealed class Scene3dFrameRequest
{
    readonly Action<VulkanScene3dFrameReason> _request;

    public Scene3dFrameRequest(Action<VulkanScene3dFrameReason> request) => _request = request;

    public void Request(VulkanScene3dFrameReason reason) => _request(reason);
}

using FluidWarfare.Render.Vulkan.Scene3D.Session;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Frame;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Submit;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;

/// <summary>Scene3D 会话运行时状态。三个字段的单一所有权容器。</summary>
public sealed class Scene3dSessionState
{
    public VulkanScene3dSession? Session { get; private set; }
    public Scene3dFrameRoute? FrameRoute { get; private set; }
    public Scene3dFrameSubmitRoute? FrameSubmitRoute { get; private set; }
    public bool IsRunning => Session?.Status == VulkanScene3dSessionStatus.Active;
    public int Generation { get; private set; }

    public void Set(VulkanScene3dSession session, Scene3dFrameRoute route, Scene3dFrameSubmitRoute submit)
    {
        Session = session; FrameRoute = route; FrameSubmitRoute = submit;
    }

    public void Clear()
    {
        Session?.Dispose();
        Session = null; FrameRoute = null; FrameSubmitRoute = null;
        NextGeneration();
    }

    public void NextGeneration() => Generation++;
}

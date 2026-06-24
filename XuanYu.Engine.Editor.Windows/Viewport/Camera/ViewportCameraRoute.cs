using XuanYu.Engine.Render.Camera;
using XuanYu.Engine.Render.Camera.Navigation;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
namespace FluidWarfare.Editor.Windows.Viewport.Camera;

/// <summary>相机状态和命令路由。拥有 _lastCameraState 和 _cameraRevision，通过 Apply(command) 处理所有相机操作。</summary>
public sealed class ViewportCameraRoute
{
    private SceneOrbitCameraState _lastCameraState = SceneOrbitCameraMotion.CreateDefault();
    private int _cameraRevision;
    public SceneOrbitCameraState LastCameraState => _lastCameraState;
    public int CameraRevision => _cameraRevision;

    public ViewportCameraResult Apply(ViewportCameraCommand command)
    {
        return command switch
        {
            ViewportCameraCommand.Orbit o => ApplyOrbit(o.DeltaYaw, o.DeltaPitch),
            ViewportCameraCommand.Pan p => ApplyPan(p.DeltaX, p.DeltaY, p.ViewportHeight),
            ViewportCameraCommand.Dolly d => ApplyDolly(d.DeltaPixels),
            ViewportCameraCommand.Zoom z => ApplyZoom(z.WheelNotches),
            ViewportCameraCommand.FrameAll => ApplyFrameAll(),
            ViewportCameraCommand.FrameSelected fs => ApplyFrameSelected(fs.CenterX, fs.CenterY, fs.CenterZ, fs.Radius),
            ViewportCameraCommand.ToggleProjection => ApplyToggleProjection(),
            ViewportCameraCommand.SnapToView stv => ApplySnapToView(stv.View),
            _ => ViewportCameraResult.NoChange
        };
    }

    /// <summary>从当前状态创建 SceneCameraPose，递增修订号。用于会话启动、Resize。</summary>
    public SceneCameraPose CreatePose()
    {
        _cameraRevision++;
        return SceneCameraPose.FromOrbitState(_lastCameraState, _cameraRevision);
    }
    /// <summary>重置相机到默认状态（会话启动时调用）。</summary>
    public void Reset()
    {
        _lastCameraState = SceneOrbitCameraMotion.CreateDefault();
        _cameraRevision++;
    }
    /// <summary>从外部覆盖相机状态（用于导航拖拽等连续操作）。</summary>
    public void SetState(SceneOrbitCameraState state)
    {
        _lastCameraState = state;
        _cameraRevision++;
    }

    private ViewportCameraResult ApplyOrbit(float deltaYaw, float deltaPitch)
    {
        if (deltaYaw == 0 && deltaPitch == 0) return ViewportCameraResult.NoChange;
        _lastCameraState = SceneOrbitCameraMotion.Orbit(_lastCameraState, deltaYaw, deltaPitch);
        return ViewportCameraResult.Frame(VulkanScene3dFrameReason.CameraPan);
    }
    private ViewportCameraResult ApplyPan(int deltaX, int deltaY, int viewportHeight)
    {
        if (deltaX == 0 && deltaY == 0) return ViewportCameraResult.NoChange;
        _lastCameraState = SceneOrbitCameraMotion.Pan(_lastCameraState, deltaX, deltaY, Math.Max(1, viewportHeight));
        return ViewportCameraResult.Frame(VulkanScene3dFrameReason.CameraPan);
    }
    private ViewportCameraResult ApplyDolly(float deltaPixels)
    {
        if (deltaPixels == 0) return ViewportCameraResult.NoChange;
        _lastCameraState = SceneOrbitCameraMotion.Dolly(_lastCameraState, deltaPixels);
        return ViewportCameraResult.Frame(VulkanScene3dFrameReason.CameraZoom);
    }
    private ViewportCameraResult ApplyZoom(float wheelNotches)
    {
        if (wheelNotches == 0) return ViewportCameraResult.NoChange;
        _lastCameraState = SceneOrbitCameraMotion.Zoom(_lastCameraState, wheelNotches);
        return ViewportCameraResult.Frame(VulkanScene3dFrameReason.CameraZoom);
    }
    private ViewportCameraResult ApplyFrameAll()
    {
        _lastCameraState = SceneOrbitCameraMotion.FrameAll();
        return ViewportCameraResult.Frame(VulkanScene3dFrameReason.CameraReset);
    }
    private ViewportCameraResult ApplyFrameSelected(float centerX, float centerY, float centerZ, float radius)
    {
        _lastCameraState = SceneOrbitCameraMotion.FrameSelected(_lastCameraState, centerX, centerY, centerZ, radius);
        return ViewportCameraResult.Frame(VulkanScene3dFrameReason.CameraReset);
    }
    private ViewportCameraResult ApplyToggleProjection()
    {
        _lastCameraState = SceneNavigationCameraMotion.ToggleProjection(_lastCameraState);
        return ViewportCameraResult.Frame(VulkanScene3dFrameReason.CameraReset);
    }
    private ViewportCameraResult ApplySnapToView(SceneNavigationView view)
    {
        if (view == SceneNavigationView.Free) return ViewportCameraResult.NoChange;
        _lastCameraState = SceneNavigationCameraMotion.SnapToView(_lastCameraState, view);
        return ViewportCameraResult.Frame(VulkanScene3dFrameReason.CameraReset);
    }
}

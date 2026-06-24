using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Selection;
using XuanYu.Engine.Render.Selection.Ground;
using XuanYu.Engine.Render.Selection.Presented;
using XuanYu.Engine.Render.Vulkan.Camera;

namespace FluidWarfare.Editor.Windows.Viewport.Picking;

/// <summary>
/// ViewportPointerPickRoute 的输入请求。
/// 所有参数来自 EditorShell 状态，路由不持有 Shell 引用。
/// </summary>
public sealed record ViewportPickRequest(
    int PixelX,
    int PixelY,
    PresentedCameraSnapshot CameraSnapshot,
    PresentedScenePickSnapshot PickSnapshot,
    RenderScene RenderScene,
    SceneGroundPlane Ground);

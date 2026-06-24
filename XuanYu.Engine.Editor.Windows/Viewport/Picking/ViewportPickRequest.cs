using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Selection.Ground;
using FluidWarfare.Render.Selection.Presented;
using FluidWarfare.Render.Vulkan.Camera;

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

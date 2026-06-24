using FluidWarfare.Editor.EntityTransform;
using FluidWarfare.Editor.Input.Runtime;
using FluidWarfare.Editor.Windows.Panels.Viewport.Input;
using FluidWarfare.Editor.Windows.Panels.Viewport.Tools;
using FluidWarfare.Editor.Windows.Viewport.Camera;
using FluidWarfare.Editor.Windows.Viewport.Picking;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;
using FluidWarfare.Editor.Windows.Viewport.Selection.Route;
using FluidWarfare.Editor.Windows.Viewport.Transform.Application;
using FluidWarfare.Editor.Windows.Viewport.Transform.Drag;
using FluidWarfare.Editor.Windows.Viewport.Transform.Interaction;
using XuanYu.Engine.Project.World.Transform;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Shell.Input;

public sealed record EditorViewportInputRequest(
    EditorViewportInputKind Kind,
    int KeyCode, int ButtonCode, int X, int Y, int WheelDelta,
    EditorViewportInputState State,
    TransformPointerRoute PointerRoute,
    EditorSelectionRoute SelectionRoute,
    ViewportToolPalette ToolPalette,
    ViewportCameraRoute CameraRoute,
    Scene3dSessionLifecycle Lifecycle,
    ViewportPointerPickRoute PickRoute,
    ViewportRenderSceneStore RenderSceneStore,
    EditorGroundPlacementState GroundPlacementState,
    EditorWorldDirtyState WorldDirtyState,
    Action<string> InfoLog,
    Action<string> WarnLog,
    Action<VulkanScene3dFrameReason> ScheduleFrame,
    Func<TransformStartSnapshot?> BuildTransformSnapshot,
    Action<SceneTransform, EditorEntityTransformOrigin> ApplyTransform,
    Action<TransformInteractionResult> CancelTransform,
    Action ApplyPreviewPosition,
    Action ExecuteFrameSelected);

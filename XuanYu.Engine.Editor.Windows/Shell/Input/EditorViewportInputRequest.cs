using XuanYu.Engine.Editor.EntityTransform;
using XuanYu.Engine.Editor.Input.Runtime;
using XuanYu.Engine.Editor.Windows.Panels.Viewport.Input;
using XuanYu.Engine.Editor.Windows.Panels.Viewport.Tools;
using XuanYu.Engine.Editor.Windows.Viewport.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Picking;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction;
using XuanYu.Engine.Project.World.Transform;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace XuanYu.Engine.Editor.Windows.Shell.Input;

public sealed record EditorViewportInputRequest(
    EditorViewportInputKind Kind,
    int KeyCode, int ButtonCode, int X, int Y, int WheelDelta,
    EditorViewportInputState State,
    TransformPointerRoute PointerRoute,
    EditorSelectionRoute SelectionRoute,
    ViewportToolPalette? ToolPalette,
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

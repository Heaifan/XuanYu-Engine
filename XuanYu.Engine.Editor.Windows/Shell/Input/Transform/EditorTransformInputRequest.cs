using XuanYu.Engine.Editor.EntityTransform;
using XuanYu.Engine.Editor.Windows.Panels.Viewport.Tools;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;
using XuanYu.Engine.Editor.Windows.Viewport.Selection.Route;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Interaction;
using XuanYu.Engine.Project.World.Transform;

namespace XuanYu.Engine.Editor.Windows.Shell.Input.Transform;

/// <summary>Transform 输入路由的专用请求，比全量 InputRequest 更轻量。</summary>
public sealed record EditorTransformInputRequest(
    int KeyCode, int ButtonCode, int X, int Y,
    EditorViewportInputState InputState,
    TransformPointerRoute PointerRoute,
    EditorSelectionRoute SelectionRoute,
    ViewportToolPalette? ToolPalette,
    Scene3dSessionLifecycle Lifecycle,
    Action<string> InfoLog,
    Action<SceneTransform, EditorEntityTransformOrigin> ApplyTransform,
    Action<TransformInteractionResult> CancelTransform,
    Action ApplyPreviewPosition,
    Func<TransformStartSnapshot?> BuildTransformSnapshot);

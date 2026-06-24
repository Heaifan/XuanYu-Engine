using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Render.Camera.Navigation;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Vulkan.Scene3D;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Frame;
using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Submit;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;

namespace XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Lifecycle;

/// <summary>
/// Scene3D 会话生命周期管理。单一职责：启动/停止/重启/Resize 会话。
/// 不持有 Shell/面板引用，外部依赖通过 StartRequest 和构造函数注入。
/// </summary>
public sealed class Scene3dSessionLifecycle
{
    readonly ViewportRenderSceneStore _renderSceneStore;
    readonly Scene3dSessionState _state = new();

    public Scene3dSessionState State => _state;

    public Scene3dSessionLifecycle(ViewportRenderSceneStore renderSceneStore)
    {
        _renderSceneStore = renderSceneStore;
    }

    public Scene3dSessionStartResult Start(Scene3dSessionStartRequest request)
    {
        var grid = VulkanScene3dVertices.BuildGrid(20, 2);
        var axes = VulkanScene3dVertices.BuildAxes(20, 8);
        var combined = new VulkanScene3dVertex[grid.Length + axes.Length];
        Array.Copy(grid, 0, combined, 0, grid.Length);
        Array.Copy(axes, 0, combined, grid.Length, axes.Length);
        var unitVerts = VulkanScene3dVertices.BuildCube(0, 0, 0, 1.0f);

        var unitDraws = new List<VulkanScene3dUnitDrawInfo>();
        foreach (var obj in _renderSceneStore.Current.Objects)
        {
            if (obj.VisualKind != RenderObjectVisualKind.UnitMarker) continue;
            var c = obj.Placement?.VisualCenter ?? new Vector3d(obj.Position.X, obj.Position.Y, obj.Position.Z + 0.5);
            unitDraws.Add(new VulkanScene3dUnitDrawInfo(
                obj.EntityId.Value.ToString(), (float)c.X, (float)c.Y, (float)c.Z, (float)RenderUnitPlacement.Scale));
        }

        var session = new VulkanScene3dSession();
        var result = session.Start(
            request.InstanceHandle, request.WindowHandle,
            request.Width, request.Height, request.CameraPose,
            combined.AsSpan(), unitVerts.AsSpan(), [.. unitDraws]);

        if (!result.Success)
        { session.Dispose(); return Scene3dSessionStartResult.Failed(result.Message); }

        var frameRoute = new Scene3dFrameRoute(session);
        var submitRoute = new Scene3dFrameSubmitRoute(frameRoute, session, _renderSceneStore);
        _state.Set(session, frameRoute, submitRoute);
        return new Scene3dSessionStartResult(true, session, frameRoute, submitRoute, result.Message);
    }

    public void Stop()
    {
        var session = _state.Session;
        if (session is null) return;
        _state.Clear();
    }

    public Scene3dSessionStartResult Restart(Scene3dSessionStartRequest request)
    {
        Stop();
        return Start(request);
    }
}

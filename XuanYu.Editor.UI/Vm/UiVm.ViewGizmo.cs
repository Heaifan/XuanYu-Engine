using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.UI;

// EDITOR-VIEW-R1：视口右上角视角 Gizmo 的相机命令。
// Z-Up 固定：顶=+Z 看向 -Z；底=-Z 看向 +Z；前=-Y 看向 +Y；后=+Y 看向 -Y；
// 右=+X 看向 -X；左=-X 看向 +X。保持观察中心与距离，只改朝向。
public sealed partial class UiVm
{
    string _activeViewFace = "前";

    public string ActiveViewFace => _activeViewFace;

    bool TryApplyViewFaceCommand(string name)
    {
        if (!name.StartsWith("视角-", StringComparison.Ordinal)) return false;
        ApplyViewFaceCommand(name["视角-".Length..]);
        return true;
    }

    void ApplyViewFaceCommand(string name)
    {
        if (!TryResolveViewFace(name, out var forward, out var up)) return;
        var center = ResolveViewCenter();
        var distance = _camera.Position.DistanceTo(center);
        var position = center - (forward * distance);
        _camera = new CameraState(
            position, forward, up,
            _camera.VerticalFovDegrees,
            _camera.NearPlane,
            _camera.FarPlane,
            ++_cameraRevision);
        _activeViewFace = name;
        OnPropertyChanged(nameof(ActiveViewFace));
        PublishSceneRenderSnapshot();
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            "视角已切换", $"朝向={name}；观察中心={EditorDisplayText.Position(center)}");
        RefreshLogBindings();
    }

    // 观察中心优先级：选中实体中心 → 地图中心 → 世界原点。
    Vector3d ResolveViewCenter()
    {
        if (TrySelectedEntityKey(out var key) && _sceneState.TryGetEntity(key, out var entity))
            return entity.Transform.Position;
        if (_mapWorld.HasMap) return Vector3d.Zero;
        return DefaultEditorCamera.Target;
    }

    static bool TryResolveViewFace(string name, out Vector3d forward, out Vector3d up)
    {
        switch (name)
        {
            case "顶": forward = new Vector3d(0, 0, -1); up = new Vector3d(0, 1, 0); return true;
            case "底": forward = new Vector3d(0, 0, 1); up = new Vector3d(0, 1, 0); return true;
            case "前": forward = new Vector3d(0, 1, 0); up = Vector3d.UnitZ; return true;
            case "后": forward = new Vector3d(0, -1, 0); up = Vector3d.UnitZ; return true;
            case "右": forward = new Vector3d(-1, 0, 0); up = Vector3d.UnitZ; return true;
            case "左": forward = new Vector3d(1, 0, 0); up = Vector3d.UnitZ; return true;
            default: forward = default; up = default; return false;
        }
    }
}

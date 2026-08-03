using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.UI;

// F3-D3：六方向标准视角命令（计划 8.1 命名；复用现有 ApplyViewFaceCommand 相机逻辑）。
// 保留 Pivot 与距离，只改观察方向；顶/底视图 Up=+Y 防滚转。
public sealed partial class UiVm
{
    string _activeViewFace = "默认视角";

    public string ActiveViewFace => _activeViewFace;

    bool TryApplyViewFaceCommand(string name)
    {
        if (!name.StartsWith("视角-", StringComparison.Ordinal)) return false;
        ApplyViewFaceCommand(name["视角-".Length..]);
        return true;
    }

    void ApplyViewFaceCommand(string name)
    {
        if (!StandardViewResolver.TryResolve(name, out var forward, out var up)) return;
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
        OnPropertyChanged(nameof(NavigationCamera));
        PublishSceneRenderSnapshot();
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command,
            $"切换 {name}", $"朝向={name}；观察中心={EditorDisplayText.Position(center)}");
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
}

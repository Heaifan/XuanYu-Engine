using XuanYu.World.Map;
using XuanYu.Core.Space;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D3：World 地图查询状态持有者（高度查询/边界判断权威，由会话 ContentChanged 同步）。
// 渲染快照不再经由本类型（改由 MapSession → MapRenderSnapshotProjection 直出）。
public sealed partial class UiVm
{
    readonly WorldMapStateOwner _mapWorld = new();

    public WorldMapStateOwner MapWorld => _mapWorld;

    // 地图取景：读会话当前尺寸与基础高度，45° 斜上方俯视完整容纳四角。
    void ApplyMapViewFraming(string source = "聚焦地图")
    {
        var map = MapSession.CurrentMap;
        var halfW = map.SizeMeters.Width / 2.0;
        var halfD = map.SizeMeters.Depth / 2.0;
        var z = map.Surface.BaseHeightMeters;
        FrameMapCamera(source,
            new Core.Math.Vector3d(-halfW, -halfD, z),
            new Core.Math.Vector3d(halfW, -halfD, z),
            new Core.Math.Vector3d(-halfW, halfD, z),
            new Core.Math.Vector3d(halfW, halfD, z));
    }

    // 地图取景：45° 斜上方俯视完整容纳地图，复用 EditorCameraFraming。
    void FrameMapCamera(string source, params Core.Math.Vector3d[] corners)
    {
        var frame = _camera.Mode == ProjectionMode.Orthographic
            ? XuanYu.Editor.Camera.EditorCameraFraming.FrameMapOrthographicWithCenter(
                corners, _viewportAspect,
                _camera.Position.DistanceTo(_observationCenter), ++_cameraRevision)
            : XuanYu.Editor.Camera.EditorCameraFraming.FrameMapAllWithCenter(
                corners, _viewportAspect, ++_cameraRevision);
        _camera = frame.Camera;
        _observationCenter = frame.ObservationCenter;
        _viewportCameraFramed = true;
        PublishSceneRenderSnapshot(); // F1：取景后必须发布新相机（与 FrameSelectedCamera 同模式）
        FooterMessage = $"{source}：整张地图已进入视野。";
    }
}

using XuanYu.Core.Map;
using XuanYu.Editor.MapDocument;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// MAP-A-R1-D4：UiVm 地图世界状态持有者（最小程序化入口，D4 不做 UI 对话框）。
// 提供默认地图加载/卸载与查询转发；D5 将接入地图编辑器 UI。
public sealed partial class UiVm
{
    readonly WorldMapStateOwner _mapWorld = new();

    public WorldMapStateOwner MapWorld => _mapWorld;
    public bool HasMap => _mapWorld.HasMap;

    public void LoadDefaultMap()
    {
        var doc = XuanYu.Editor.MapDocument.MapDocument.CreateNew("TestBattlefield", 2000, 2000);
        _mapWorld.Load(MapDocumentWorldBridge.ToWorldState(doc));
        OnPropertyChanged(nameof(HasMap));
        ApplyMapViewFraming();
        FooterMessage = $"地图已加载：{_mapWorld.CurrentMap!.Name}（2000×2000 米）。";
        PublishSceneRenderSnapshot();
    }

    public void UnloadMap()
    {
        if (!_mapWorld.HasMap) return;
        _mapWorld.Unload();
        OnPropertyChanged(nameof(HasMap));
        ApplyMapViewFraming();
        FooterMessage = "地图已卸载。";
        PublishSceneRenderSnapshot();
    }

    void ApplyMapViewFraming()
    {
        if (!_mapWorld.HasMap) return;
        var map = _mapWorld.CurrentMap!;
        var halfW = map.WidthMeters / 2.0;
        var halfD = map.DepthMeters / 2.0;
        FrameMapCamera(
            new Core.Math.Vector3d(-halfW, -halfD, 0),
            new Core.Math.Vector3d(halfW, -halfD, 0),
            new Core.Math.Vector3d(-halfW, halfD, 0),
            new Core.Math.Vector3d(halfW, halfD, 0));
    }

    // 地图取景：复用 EditorCameraFraming 以地图四角取景整张地图。
    void FrameMapCamera(params Core.Math.Vector3d[] corners)
    {
        var frame = XuanYu.Editor.Camera.EditorCameraFraming.FrameAllWithCenter(
            corners, _viewportAspect, ++_cameraRevision);
        _camera = frame.Camera;
        _observationCenter = frame.ObservationCenter;
        _viewportCameraFramed = true;
        FooterMessage = "相机已取景整张地图。";
    }
}

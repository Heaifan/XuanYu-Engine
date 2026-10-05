using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool CloseRoadDraft()
    {
        if (!RoadDrawingController.CanComplete)
        { FooterMessage = "道路至少需要两个节点才能完成。"; return true; }
        var result = RoadDrawingController.Complete();
        if (!result.IsSuccess) { FooterState = "状态：错误"; FooterMessage = result.ErrorMessage ?? "道路创建失败"; return true; }
        var road = result.Road!;
        _roadDrawing.Clear();
        SelectTool("选择", logTool: false);
        SelectMapGeometry(new(MapGeometryFeatureKind.Road, road.RoadId.ToString()));
        FooterState = "状态：就绪"; FooterMessage = "道路已创建，已进入选择状态";
        LogRoadDrawingCreated(); PublishSceneRenderSnapshot(); return true;
    }
}

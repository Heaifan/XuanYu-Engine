using XuanYu.Editor.MapEditing;
using XuanYu.Editor.Input;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool CloseRegionDraft()
    {
        if (!TryRequireCurrentMapManifestIdentity())
        {
            TerminateRegionDrawing(EditorPointerEventKind.Cancel, FooterMessage);
            return false;
        }
        ReleaseRegionPointerCapture(EditorPointerEventKind.Released);
        var request = _regionDrawing.Controller.Complete();
        if (request is null)
        {
            FooterMessage = "区域至少需要三个顶点才能闭合。";
            LogRegionDrawingError(FooterMessage);
            return true;
        }
        var draft = RegionDrawingAdapter.ToDraft(request, _regionDrawing.Controller.LayerId,
            MapObjectNameAllocator.Region(MapSession.CurrentMap, _regionDrawing.Controller.DisplayName),
            _regionDrawing.Controller.Kind, _regionDrawing.Controller.SurfaceBinding);
        var result = MapSession.CreateRegion(draft);
        if (!result.IsSuccess)
        {
            var message = result.Error?.Message ?? "区域闭合失败";
            TerminateRegionDrawing(EditorPointerEventKind.Cancel, message);
            return true;
        }
        _regionDrawing.Controller.ClearAfterCommit();
        TerminateRegionDrawing(EditorPointerEventKind.Released);
        SelectTool("选择", logTool: false); FooterState = "状态：就绪"; FooterMessage = "区域已创建";
        LogRegionDrawingCreated();
        PublishSceneRenderSnapshot(); return true;
    }
}

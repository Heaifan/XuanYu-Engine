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
        var draft = _regionDrawing.TakeDraftForClose();
        if (draft is null)
        {
            FooterMessage = "区域至少需要三个顶点才能闭合。";
            LogRegionDrawingError(FooterMessage);
            return true;
        }
        draft = draft with { DisplayName = MapObjectNameAllocator.Region(MapSession.CurrentMap, draft.DisplayName) };
        var result = MapSession.CreateRegion(draft);
        if (!result.IsSuccess)
        {
            var message = result.Error?.Message ?? "区域闭合失败";
            TerminateRegionDrawing(EditorPointerEventKind.Cancel, message);
            return true;
        }
        TerminateRegionDrawing(EditorPointerEventKind.Released);
        SelectTool("选择", logTool: false); FooterState = "状态：就绪"; FooterMessage = "区域已创建";
        LogRegionDrawingCreated();
        PublishSceneRenderSnapshot(); return true;
    }
}

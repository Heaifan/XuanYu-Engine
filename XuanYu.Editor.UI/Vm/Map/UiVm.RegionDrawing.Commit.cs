namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool CloseRegionDraft()
    {
        var draft = _regionDrawing.TakeDraftForClose();
        if (draft is null)
        {
            FooterMessage = "区域至少需要三个顶点才能闭合。";
            LogRegionDrawingError(FooterMessage);
            return true;
        }
        var result = MapSession.CreateRegion(draft);
        if (!result.IsSuccess)
        {
            FooterState = "状态：错误";
            FooterMessage = result.Error?.Message ?? "区域闭合失败";
            LogRegionDrawingError(FooterMessage);
            return true;
        }
        _regionDrawing.Cancel(); RaiseRegionDrawingBindings(); FooterState = "状态：就绪"; FooterMessage = "区域已创建";
        LogRegionDrawingCreated();
        PublishSceneRenderSnapshot(); return true;
    }
}

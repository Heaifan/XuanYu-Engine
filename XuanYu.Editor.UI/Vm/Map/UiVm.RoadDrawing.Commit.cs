namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool CloseRoadDraft()
    {
        var draft = _roadDrawing.TakeDraft();
        if (draft is null) { FooterMessage = "道路至少需要两个节点才能完成。"; return true; }
        var result = MapSession.CreateRoad(draft);
        if (!result.IsSuccess) { FooterState = "状态：错误"; FooterMessage = result.Error?.Message ?? "道路创建失败"; return true; }
        _roadDrawing.Cancel(); RaiseRoadDrawingBindings(); FooterState = "状态：就绪"; FooterMessage = "道路已创建"; LogRoadDrawingCreated(); PublishSceneRenderSnapshot(); return true;
    }
}

namespace XuanYu.Editor.UI;

// D4：新建场景（5+100 拆分自 UiVm.SceneDocument.cs）。
public sealed partial class UiVm
{
    public void NewBlankScene()
    {
        CancelActiveInput("新建场景");
        _historyOwner.Clear();
        _documentSession.MarkNew();
        _staticModelCatalog.Clear();
        _staticModelResources.Clear();
        _sceneState.ReplaceEntities([]);
        ResetCameraForSceneReplacement();
        ApplySelectionCommand(new ClearEditorSelectionCommand(), "新建场景");
        FooterMessage = "已创建空白未命名场景。";
        RaiseDocumentChanged();
        RefreshWorldProjectionBindings();
    }
}

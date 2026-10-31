using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// D4：新建场景（5+100 拆分自 UiVm.SceneDocument.cs）。
public sealed partial class UiVm
{
    public void NewBlankScene() => _sceneDocumentCoordinator.StartNew(
        () => CancelActiveInput("新建场景"), _historyOwner.Clear,
        () => { _staticModelCatalog.Clear(); _staticModelResources.Clear(); },
        () => _sceneState.ReplaceEntities([]),
        () =>
        {
            _mapWorkingStorage.Discard();
            MapSession.ResetToEmptyWorld();
            ResetMapManifestFromCurrentMap();
        }, () => ResetCameraForSceneReplacement(),
        () => ApplySelectionCommand(new ClearEditorSelectionCommand(), "新建场景"), CompleteNewScene);

    void CompleteNewScene()
    {
        FooterMessage = "已创建空白未命名场景。";
        RaiseDocumentChanged();
        RefreshWorldProjectionBindings();
    }
}

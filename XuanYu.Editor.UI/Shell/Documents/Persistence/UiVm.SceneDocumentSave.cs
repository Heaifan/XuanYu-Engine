using XuanYu.Editor.SceneDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public Task<bool> SaveSceneAsync(string? path = null, bool saveAs = false) =>
        _sceneDocumentCoordinator.SaveAsync(path, saveAs, () => CommitActiveInteractionForSave(),
            target => WithMapReference(SceneDocumentWorldBridge.Capture(
                _sceneState, DocumentSession.SceneId, DocumentSession.SceneName, _staticModelCatalog), target),
            () => _staticModelCatalog.Snapshot, () => _historyOwner.CurrentRevision,
            StartSceneSave, outcome => _staticModelCatalog.RebindSourcePaths(outcome.HostedSourcePaths),
            CompleteSceneSave, FailSave);

    void StartSceneSave(string _, bool __)
    {
        SetSceneBusy(true);
        FooterState = "状态：正在保存";
    }

    void CompleteSceneSave(string target, bool saveAs)
    {
        FooterMessage = saveAs ? "场景已另存为。" : "场景已保存。";
        LogSceneSaveSuccess(target, saveAs);
        SetSceneBusy(false);
        RaiseDocumentChanged();
        ShowTemporaryDocumentStatus(saveAs ? "状态：另存为成功" : "状态：保存成功");
    }

    void FailSave(string path, SceneDocumentResult<SceneSaveOutcome> result)
    {
        SetSceneBusy(false);
        FooterMessage = result.Message;
        FooterState = "状态：保存失败";
        LogSceneSaveFailure(path, result);
        RaiseDocumentChanged();
    }
}

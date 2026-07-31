using XuanYu.Editor.SceneDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public async Task<bool> SaveSceneAsync(string? path = null, bool saveAs = false)
    {
        CommitActiveInteractionForSave();
        var target = path ?? _documentSession.CurrentPath;
        if (string.IsNullOrWhiteSpace(target)) return false;
        var snapshot = SceneDocumentWorldBridge.Capture(
            _sceneState, _documentSession.SceneId, _documentSession.SceneName);
        SetSceneBusy(true);
        FooterState = "状态：正在保存";
        var result = await _sceneStorage.SaveAsync(target, snapshot);
        if (!result.Succeeded) return FailSave(target, result);
        _documentSession.MarkSaved(target, snapshot, _historyOwner.Count);
        FooterMessage = saveAs ? "场景已另存为。" : "场景已保存。";
        LogSceneSaveSuccess(target, saveAs);
        SetSceneBusy(false);
        RaiseDocumentChanged();
        ShowTemporaryDocumentStatus(saveAs ? "状态：另存为成功" : "状态：保存成功");
        return true;
    }

    bool FailSave<T>(string path, SceneDocumentResult<T> result)
    {
        SetSceneBusy(false);
        _documentSession.MarkError(result.Message);
        FooterMessage = result.Message;
        FooterState = "状态：保存失败";
        LogSceneSaveFailure(path, result);
        RaiseDocumentChanged();
        return false;
    }
}

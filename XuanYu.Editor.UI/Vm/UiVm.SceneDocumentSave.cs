using XuanYu.Editor.SceneDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public async Task<bool> SaveSceneAsync(string? path = null)
    {
        CommitActiveInteractionForSave();
        var target = path ?? _documentSession.CurrentPath;
        if (string.IsNullOrWhiteSpace(target)) return false;
        var snapshot = SceneDocumentWorldBridge.Capture(
            _sceneState, _documentSession.SceneId, _documentSession.SceneName);
        FooterState = "状态：正在保存";
        var result = await _sceneStorage.SaveAsync(target, snapshot);
        if (!result.Succeeded) return FailSave(target, result);
        _documentSession.MarkSaved(target, snapshot, _historyOwner.Count);
        FooterMessage = "场景已保存。";
        FooterState = "状态：就绪";
        RaiseDocumentChanged();
        return true;
    }

    bool FailSave<T>(string path, SceneDocumentResult<T> result)
    {
        _documentSession.MarkError(result.Message);
        FooterMessage = result.Message;
        FooterState = "状态：保存失败";
        LogSceneSaveFailure(path, result);
        RaiseDocumentChanged();
        return false;
    }
}

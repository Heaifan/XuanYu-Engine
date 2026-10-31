using XuanYu.Editor.SceneDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void LogSceneLoadStart(string path) =>
        LogScene(EditorLogLevel.Info, "场景加载开始", $"Path={path}");

    void LogSceneLoadStage(string stage) =>
        LogScene(EditorLogLevel.Info, "场景加载阶段", $"Stage={stage}");

    void LogSceneLoadSuccess(string path, int count) =>
        LogScene(EditorLogLevel.Info, "场景加载成功", $"Path={path}；Entities={count}");

    void LogSceneLoadFailure<T>(string path, SceneDocumentResult<T> result) =>
        LogScene(EditorLogLevel.Error, "场景加载失败",
            $"Path={path}；Stage={result.Stage}；Code={result.ErrorCode}；Message={result.Message}；Detail={result.Detail}；CurrentScenePreserved=True");

    void LogSceneRetry(string path) =>
        LogScene(EditorLogLevel.Warning, "场景加载重试", $"Path={path}；用户选择重试加载");

    void StartSceneLoad(string path)
    {
        SetSceneBusy(true);
        FooterState = "状态：正在加载";
        LogSceneLoadStart(path);
    }

    void FailSceneLoad(string path, SceneDocumentResult<SceneLoadCandidate> result)
    {
        SetSceneBusy(false);
        FooterMessage = result.Message;
        FooterState = "状态：加载失败";
        LogSceneLoadFailure(path, result);
        RaiseDocumentChanged();
    }

    async Task<bool> RetrySceneLoadAsync(string path)
    {
        var retry = await _dialogService.ShowRetryAsync("打开场景失败",
            "无法打开所选场景。\n场景文件内容无效或版本不受支持。是否重试？");
        if (retry) LogSceneRetry(path);
        return retry;
    }

    void CompleteSceneLoad(SceneLoadCandidate value)
    {
        if (string.IsNullOrEmpty(MapReferenceError))
            FooterMessage = value.HasUnavailableAssets ? "场景已打开，部分资源不可用。" : "场景已打开。";
        LogSceneLoadSuccess(DocumentSession.CurrentPath ?? "", value.Entities.Count);
        SetSceneBusy(false);
        RaiseDocumentChanged();
        ShowTemporaryDocumentStatus("状态：场景加载成功");
        RefreshWorldProjectionBindings();
    }

    Task ReportUnavailableAssetsAsync(SceneLoadCandidate value) => ShowAssetSummaryAsync(value);

    void LogSceneSaveFailure<T>(string path, SceneDocumentResult<T> result) =>
        LogScene(EditorLogLevel.Error, EditorLogCategory.Save, "场景保存失败",
            $"Path={path}；Stage={result.Stage}；Code={result.ErrorCode}；Message={result.Message}；Detail={result.Detail}");

    void LogSceneSaveSuccess(string path, bool saveAs)
    {
        var name = Path.GetFileName(path);
        var message = saveAs ? $"场景另存为成功：{name}" : $"场景保存成功：{name}";
        LogScene(EditorLogLevel.Info, EditorLogCategory.Save, message, $"Path={path}");
    }

    bool FailCandidateBuild(string path, Exception ex)
    {
        var result = SceneDocumentResult<string>.Fail(
            "CandidateBuildFailed", "场景候选构建失败。", "BuildCandidate", ex.Message);
        DocumentSession.MarkError(result.Message);
        FooterMessage = result.Message;
        FooterState = "状态：加载失败";
        LogSceneLoadFailure(path, result);
        RaiseDocumentChanged();
        return false;
    }

    void LogScene(EditorLogLevel level, string message, string detail) =>
        LogScene(level, EditorLogCategory.Load, message, detail);

    void LogScene(EditorLogLevel level, EditorLogCategory category, string message, string detail)
    {
        if (level == EditorLogLevel.Error)
            _logBus.Error(EditorLogSource.Project, category, message, detail);
        else _logBus.Info(EditorLogSource.Project, category, message, detail);
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} [{level}] 场景文档 {message}；{detail}");

    }
}

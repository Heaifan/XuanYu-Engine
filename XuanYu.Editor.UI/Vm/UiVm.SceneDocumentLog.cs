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

    void LogSceneSaveFailure<T>(string path, SceneDocumentResult<T> result) =>
        LogScene(EditorLogLevel.Error, "场景保存失败",
            $"Path={path}；Stage={result.Stage}；Code={result.ErrorCode}；Message={result.Message}；Detail={result.Detail}");

    bool FailCandidateBuild(string path, Exception ex)
    {
        var result = SceneDocumentResult<string>.Fail(
            "CandidateBuildFailed", "场景候选构建失败。", "BuildCandidate", ex.Message);
        _documentSession.MarkError(result.Message);
        FooterMessage = result.Message;
        FooterState = "状态：加载失败";
        LogSceneLoadFailure(path, result);
        RaiseDocumentChanged();
        return false;
    }

    void LogScene(EditorLogLevel level, string message, string detail)
    {
        if (level == EditorLogLevel.Error)
            _logBus.Error(EditorLogSource.Project, EditorLogCategory.Load, message, detail);
        else _logBus.Info(EditorLogSource.Project, EditorLogCategory.Load, message, detail);
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} [{level}] 场景文档 {message}；{detail}");
        RefreshLogBindings();
    }
}

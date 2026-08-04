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
        _documentSession.MarkError(result.Message);
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
        RefreshLogBindings();
    }
}

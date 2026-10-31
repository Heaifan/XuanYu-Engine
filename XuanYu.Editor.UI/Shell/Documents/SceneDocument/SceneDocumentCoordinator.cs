using XuanYu.Editor.Assets;
using XuanYu.Editor.SceneDocument;

namespace XuanYu.Editor.UI;

internal sealed class SceneDocumentCoordinator(
    SceneDocumentSaveTransaction save,
    SceneDocumentLoadTransaction load)
{
    public SceneDocumentSession Session { get; } = new();

    public void StartNew(Action cancelInteraction, Action clearHistory, Action clearAssets,
        Action clearEntities, Action resetMap, Action resetCamera, Action clearSelection, Action complete)
    {
        cancelInteraction();
        clearHistory();
        Session.MarkNew();
        clearAssets();
        clearEntities();
        resetMap();
        resetCamera();
        clearSelection();
        complete();
    }

    public async Task<bool> SaveAsync(string? path, bool saveAs, Action prepare,
        Func<string, SceneDocumentSnapshot> capture,
        Func<IReadOnlyList<SceneStaticModelBinding>> bindings, Func<long> revision,
        Action<string, bool> start, Action<SceneSaveOutcome> rebind,
        Action<string, bool> complete, Action<string, SceneDocumentResult<SceneSaveOutcome>> fail)
    {
        prepare();
        var target = path ?? Session.CurrentPath;
        if (string.IsNullOrWhiteSpace(target)) return false;
        var snapshot = capture(target);
        start(target, saveAs);
        var result = await save.ExecuteAsync(target, snapshot, bindings());
        if (!result.Succeeded || result.Value is null)
        {
            Session.MarkError(result.Message);
            fail(target, result);
            return false;
        }
        rebind(result.Value);
        Session.MarkSaved(target, result.Value.SavedSnapshot, revision());
        complete(target, saveAs);
        return true;
    }

    public async Task<bool> OpenAsync(string path, Action<string> start,
        Action<string, SceneDocumentResult<SceneLoadCandidate>> fail,
        Func<string, Task<bool>> retry, Action<string> beforeReplace,
        Action<SceneLoadCandidate> replace,
        Func<SceneLoadCandidate, Task> completeReplace,
        Action<SceneLoadCandidate> complete, Func<SceneLoadCandidate, Task> report)
    {
        while (true)
        {
            start(path);
            var result = await load.BuildCandidateAsync(path);
            if (!result.Succeeded || result.Value is null)
            {
                Session.MarkError(result.Message);
                fail(path, result);
                if (!await retry(path)) return false;
                continue;
            }
            var candidate = result.Value;
            beforeReplace(path);
            replace(candidate);
            Session.MarkLoaded(path, candidate.Snapshot);
            await completeReplace(candidate);
            complete(candidate);
            if (candidate.HasUnavailableAssets) await report(candidate);
            return true;
        }
    }
}

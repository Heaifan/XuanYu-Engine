using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;
using XuanYu.Editor.SceneDocument;
using XuanYu.Editor.UI;
using XuanYu.World;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.Assets;

// D4 测试辅助：独立临时目录 + 保存/加载事务 + Fake Dialog 计数。
public sealed class ScenePersistenceEnv : IDisposable
{
    public string Dir { get; } = Path.Combine(Path.GetTempPath(), "xy-d4-" + Guid.NewGuid().ToString("N"));
    public FakeDialogService Dialogs { get; } = new();
    public SceneStorageService Storage { get; } = new();
    public SceneStateOwner Scene { get; }
    public SceneStaticModelCatalog Catalog { get; } = new();
    public SceneDocumentSaveTransaction SaveTx { get; }
    public SceneDocumentLoadTransaction LoadTx { get; }

    public ScenePersistenceEnv()
    {
        Directory.CreateDirectory(Dir);
        Scene = new SceneStateOwner(new GridWorldPartitionStrategy(regionSize: 5), seedInitialEntity: false);
        SaveTx = new SceneDocumentSaveTransaction(Storage);
        LoadTx = new SceneDocumentLoadTransaction(Storage, new GlbImportService(),
            new GridWorldPartitionStrategy(regionSize: 5));
    }

    public string NewScenePath(string name = "Battle01") => Path.Combine(Dir, name + ".xyscene");

    public string ImportGlb(string glbPath, string entityName = "soldier")
    {
        var result = new StaticModelAuthoringService().Import(glbPath, Scene, Catalog);
        if (!result.Succeeded) throw new InvalidOperationException(result.UserMessage);
        return result.AssetId.Value;
    }

    public string NewGlb(string name = "a", byte[]? content = null)
    {
        var path = Path.Combine(Dir, name + ".glb");
        File.WriteAllBytes(path, content ?? [0x67, 0x6C, 0x62, 0x46, 0x00]);
        return path;
    }

    public async Task<SceneDocumentResult<SceneSaveOutcome>> SaveAsync(string scenePath) =>
        await SaveTx.ExecuteAsync(scenePath,
            SceneDocumentWorldBridge.Capture(Scene, "scene-id", "场景", Catalog), Catalog.Snapshot);

    public async Task<SceneDocumentResult<SceneLoadCandidate>> LoadAsync(string scenePath) =>
        await LoadTx.BuildCandidateAsync(scenePath);

    public AssetId Asset(string hex) =>
        AssetId.TryParse("asset_" + hex.PadLeft(32, '0'), out var id) ? id : default;

    public void Dispose()
    {
        try { if (Directory.Exists(Dir)) Directory.Delete(Dir, true); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}

public sealed class FakeDialogService : IEditorDialogService
{
    public List<(string Title, string Message)> Shown { get; } = [];
    public int ErrorCount => Shown.Count(x => x.Item1.Contains("失败"));
    public int WarningCount => Shown.Count(x => x.Item1.Contains("资源不可用"));
    public Task ShowErrorAsync(string title, string message)
    {
        Shown.Add((title, message));
        return Task.CompletedTask;
    }

    public Task ShowWarningAsync(string title, string message)
    {
        Shown.Add((title, message));
        return Task.CompletedTask;
    }
}

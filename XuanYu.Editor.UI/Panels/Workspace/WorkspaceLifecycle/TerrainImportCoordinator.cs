using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Terrain.Source;

namespace XuanYu.Editor.UI;

public sealed class TerrainImportCoordinator
{
    CancellationTokenSource? _cancellation;
    public TerrainImportState State { get; private set; }
    public double Percentage { get; private set; }
    public string Message { get; private set; } = "";
    public event Action? Changed;

    public async Task<bool> RunAsync(IReadOnlyList<string> paths, Action<TerrainTileSet> activate)
    {
        if (State == TerrainImportState.Importing) return false;
        State = TerrainImportState.Importing; Percentage = 0; Message = "准备导入"; Changed?.Invoke();
        var cancellation = new CancellationTokenSource(); _cancellation = cancellation;
        var progress = new Progress<TerrainImportProgress>(Report);
        try
        {
            var tiles = await Task.Run(() => TerrainSourceImport.ReadMany(paths, progress,
                cancellation.Token), cancellation.Token);
            cancellation.Token.ThrowIfCancellationRequested();
            activate(tiles);
            Report(new(TerrainImportStage.Activating, 1, 1, 100, "导入完成"));
            State = TerrainImportState.Completed; Changed?.Invoke();
            return true;
        }
        catch (OperationCanceledException) { Fail(TerrainImportState.Cancelled, "已取消导入。"); }
        catch (TerrainSourceReadException error) { Fail(TerrainImportState.Error, error.Message); }
        catch (Exception error) { Fail(TerrainImportState.Error, $"Runtime 激活失败：{error.Message}"); }
        finally
        {
            if (ReferenceEquals(_cancellation, cancellation)) _cancellation = null;
            cancellation.Dispose(); Changed?.Invoke();
        }
        return false;
    }

    public void Cancel() => _cancellation?.Cancel();

    void Report(TerrainImportProgress progress)
    {
        if (State != TerrainImportState.Importing) return;
        Percentage = Math.Max(Percentage, Math.Clamp(progress.Percentage, 0, 100));
        Message = progress.Message; Changed?.Invoke();
    }

    void Fail(TerrainImportState state, string message)
    {
        State = state; Message = message; Changed?.Invoke();
    }
}

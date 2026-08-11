using System.Text;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public sealed partial class MapDatasetRegistry
{
    public async Task<MapDocumentResult<string>> SaveRegionContentAsync(MapDefinition map)
    {
        var candidates = BuildRegionSaveCandidates(map);
        if (!candidates.Succeeded || candidates.Value is null)
            return MapDocumentResult<string>.Fail(candidates.ErrorCode, candidates.Message, candidates.Stage);
        foreach (var (_, document) in candidates.Value)
        {
            var valid = MapDatasetDocumentValidator.Validate(document);
            if (!valid.Succeeded) return MapDocumentResult<string>.Fail(valid.ErrorCode, valid.Message, valid.Stage);
        }
        var originals = new Dictionary<string, byte[]?>(); var temps = new List<(string Temp, string Path)>();
        try
        {
            foreach (var (path, document) in candidates.Value)
            {
                originals[path] = File.Exists(path) ? await File.ReadAllBytesAsync(path) : null;
                var temp = Path.Combine(Path.GetDirectoryName(path)!, $".{Path.GetFileName(path)}.{Guid.NewGuid():N}.tmp");
                await File.WriteAllTextAsync(temp, MapDatasetDocumentSerializer.Serialize(document), Encoding.UTF8);
                temps.Add((temp, path));
            }
            foreach (var (temp, path) in temps) File.Move(temp, path, true);
            return MapDocumentResult<string>.Ok(MapPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            foreach (var (path, original) in originals)
            {
                if (original is null) TryDelete(path);
                else await File.WriteAllBytesAsync(path, original);
            }
            return MapDocumentResult<string>.Fail("RegionSaveFailed", $"Region Dataset 保存失败：{ex.Message}", "Save", ex.Message);
        }
        finally { foreach (var (temp, _) in temps) TryDelete(temp); }
    }
}

using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

public sealed partial class MapDatasetRegistry
{
    public async Task<MapDocumentResult<MapDatasetDescriptor>> CreateAutoAsync(
        string type, Func<string>? suffixFactory = null)
    {
        var generated = MapDatasetIdGenerator.Generate(type, IsDatasetIdTaken, suffixFactory);
        return generated.Succeeded
            ? await CreateAsync(generated.Value!, type)
            : MapDocumentResult<MapDatasetDescriptor>.Fail(
                generated.ErrorCode, generated.Message, generated.Stage, generated.Detail);
    }

    public async Task<MapDocumentResult<MapDatasetDescriptor>> CreateAsync(string id, string type)
    {
        var descriptor = new MapDatasetDescriptor(id, type, $"data/{id}.json");
        var candidate = PrepareCandidate(descriptor);
        if (!candidate.Succeeded || candidate.Value is null) return Fail< MapDatasetDescriptor>(candidate);
        if (!MapDatasetPathPolicy.TryResolve(MapRoot, descriptor.Source, out var datasetPath))
            return Fail<MapDatasetDescriptor>("InvalidDatasetSource", "Dataset source 不安全。", "Validate");
        if (File.Exists(datasetPath))
            return Fail<MapDatasetDescriptor>("SourceCollision", "Dataset source 文件已存在，拒绝覆盖。", "Validate");
        var document = MapDatasetDocument.CreateNew(descriptor);
        var documentValid = MapDatasetDocumentValidator.Validate(document);
        if (!documentValid.Succeeded)
            return Fail<MapDatasetDescriptor>(documentValid.ErrorCode, documentValid.Message, "Validate");
        var committed = await CommitCreateAsync(candidate.Value, document, datasetPath);
        if (!committed.Succeeded) return Fail<MapDatasetDescriptor>(committed);
        CurrentManifest = candidate.Value;
        return MapDocumentResult<MapDatasetDescriptor>.Ok(descriptor);
    }

    public async Task<MapDocumentResult<MapDatasetDescriptor>> RegisterAsync(
        MapDatasetDescriptor descriptor)
    {
        var candidate = PrepareCandidate(descriptor);
        if (!candidate.Succeeded || candidate.Value is null) return Fail< MapDatasetDescriptor>(candidate);
        if (!MapDatasetPathPolicy.TryResolve(MapRoot, descriptor.Source, out var path))
            return Fail<MapDatasetDescriptor>("InvalidDatasetSource", "Dataset source 不安全。", "Validate");
        var loaded = await _datasetStorage.LoadAsync(path, descriptor);
        if (loaded.Status != MapDatasetStatus.Normal)
            return Fail<MapDatasetDescriptor>(loaded.ErrorCode, loaded.Message, "Read");
        var saved = await _manifestStorage.SaveAsync(MapPath, candidate.Value);
        if (!saved.Succeeded) return Fail<MapDatasetDescriptor>(saved);
        CurrentManifest = candidate.Value;
        return MapDocumentResult<MapDatasetDescriptor>.Ok(descriptor);
    }

    MapDocumentResult<MapManifest> PrepareCandidate(MapDatasetDescriptor descriptor)
    {
        var candidate = CurrentManifest with
        {
            Datasets = CurrentManifest.Datasets.Append(descriptor).ToImmutableArray(),
            DatasetLayerStates = CurrentManifest.DatasetLayerStates.Append(
                DatasetLayerState.CreateDefault(descriptor.Id, CurrentManifest.DatasetLayerStates.Length)).ToImmutableArray()
        };
        var valid = MapManifestValidator.Validate(candidate);
        return valid.Succeeded
            ? valid
            : MapDocumentResult<MapManifest>.Fail(valid.ErrorCode, valid.Message, valid.Stage, valid.Detail);
    }

    bool IsDatasetIdTaken(string id)
    {
        if (CurrentManifest.Datasets.Any(item =>
                string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase))) return true;
        return MapDatasetPathPolicy.TryResolve(MapRoot, $"data/{id}.json", out var path) &&
            (File.Exists(path) || Directory.Exists(path));
    }

    static MapDocumentResult<T> Fail<T>(MapDocumentResult<MapManifest> result) =>
        MapDocumentResult<T>.Fail(result.ErrorCode, result.Message, result.Stage, result.Detail);

    static MapDocumentResult<T> Fail<T>(MapDatasetLoadResult result, string stage = "") =>
        MapDocumentResult<T>.Fail(result.ErrorCode, result.Message, stage);

    static MapDocumentResult<T> Fail<T>(MapDocumentResult<string> result) =>
        MapDocumentResult<T>.Fail(result.ErrorCode, result.Message, result.Stage, result.Detail);

    static MapDocumentResult<T> Fail<T>(string code, string message, string stage) =>
        MapDocumentResult<T>.Fail(code, message, stage);
}

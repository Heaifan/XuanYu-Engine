using System.Collections.Immutable;
using System.Text.Json;

namespace XuanYu.Editor.MapDocument;

public sealed record MapDatasetDocument(
    string Format,
    string Version,
    string Id,
    string Type,
    ImmutableArray<JsonElement> Features)
{
    public const string CurrentFormat = "xuanyu-map-dataset";
    public const string LegacyVersion = "0.1.0";
    public const string CurrentVersion = "0.3.0";

    public static MapDatasetDocument CreateNew(MapDatasetDescriptor descriptor) => new(
        CurrentFormat,
        CurrentVersion,
        descriptor.Id,
        descriptor.Type,
        ImmutableArray<JsonElement>.Empty);
}

public enum MapDatasetStatus
{
    Normal,
    Missing,
    Invalid
}

public sealed record MapDatasetLoadResult(
    MapDatasetStatus Status,
    MapDatasetDocument? Document,
    string ErrorCode = "",
    string Message = "")
{
    public static MapDatasetLoadResult Normal(MapDatasetDocument document) =>
        new(MapDatasetStatus.Normal, document);

    public static MapDatasetLoadResult Missing(string message) =>
        new(MapDatasetStatus.Missing, null, "MissingFile", message);

    public static MapDatasetLoadResult Invalid(string code, string message) =>
        new(MapDatasetStatus.Invalid, null, code, message);
}

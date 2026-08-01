using XuanYu.Core.Spatial;

namespace XuanYu.Editor.Assets;

public sealed record StaticModelData(
    IReadOnlyList<StaticModelVertex> Vertices,
    IReadOnlyList<uint> Indices,
    IReadOnlyList<StaticModelPrimitive> Primitives,
    SpatialAabb LocalBounds,
    StaticModelImportMetadata Metadata,
    IReadOnlyList<StaticModelImportWarning> Warnings);

public sealed record StaticModelImportMetadata(
    string DisplayName,
    string ImporterVersion,
    int SourceByteLength);

using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public sealed record MapMarkerDatasetFeature(MapMarkerId MarkerId, MapPoint Position, string Name);

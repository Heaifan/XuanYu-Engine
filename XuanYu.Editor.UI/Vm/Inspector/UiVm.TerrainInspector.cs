using System.Globalization;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Source;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    const double MinVerticalExaggeration = 0.1;
    const double MaxVerticalExaggeration = 20.0;
    TerrainMetadata? _terrainInspectorMetadata;
    double _verticalExaggeration = 1.0;

    public bool IsTerrainInspector => InspectorIdentity == InspectorObjectKind.Terrain;
    public TerrainMetadata? TerrainInspectorMetadata => _terrainInspectorMetadata;
    public double VerticalExaggeration
    {
        get => _verticalExaggeration;
        set
        {
            if (!double.IsFinite(value)) return;
            var next = Math.Clamp(value, MinVerticalExaggeration, MaxVerticalExaggeration);
            if (!Set(ref _verticalExaggeration, next)) return;
            OnPropertyChanged(nameof(VerticalExaggerationText));
            OnPropertyChanged(nameof(InspectorProperties));
            OnPropertyChanged(nameof(InspectorFields));
            PublishSceneRenderSnapshot();
        }
    }

    public string VerticalExaggerationText =>
        $"{VerticalExaggeration.ToString("0.###", CultureInfo.InvariantCulture)}×";

    public void SetTerrainInspectorSource(TerrainSourceData source)
    {
        _terrainInspectorMetadata = TerrainWorld.FromSource(source).Metadata;
        RaiseInspectorSelectionBindings();
        OnPropertyChanged(nameof(TerrainInspectorMetadata));
        OnPropertyChanged(nameof(InspectorProperties));
        OnPropertyChanged(nameof(InspectorFields));
    }

    internal bool CommitTerrainVerticalExaggeration(InspectorEditTarget target, string text)
    {
        if (!IsTerrainInspector || target.ObjectKind != InspectorObjectKind.Terrain ||
            target.ObjectId != InspectorObjectId() || !TryParseInspectorNumber(text, out var value))
            return false;
        VerticalExaggeration = value;
        FooterMessage = $"垂直夸张已更新：{VerticalExaggerationText}。";
        return true;
    }

    internal string TerrainValue(string key) => key switch
    {
        "Terrain.Data.Grid" => $"{TerrainInspectorMetadata!.Width} × {TerrainInspectorMetadata.Height}",
        "Terrain.Data.Resolution" => ResolutionText(TerrainInspectorMetadata!),
        "Terrain.Data.MinElevation" => $"{FormatTerrainNumber(TerrainInspectorMetadata!.MinElevation)} m",
        "Terrain.Data.MaxElevation" => $"{FormatTerrainNumber(TerrainInspectorMetadata!.MaxElevation)} m",
        "Terrain.Data.NoData" => TerrainInspectorMetadata!.NoData?.ToString("0.###", CultureInfo.InvariantCulture) ?? "—",
        "Terrain.Display.VerticalExaggeration" => VerticalExaggerationText,
        _ => ""
    };

    static string FormatTerrainNumber(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    static string ResolutionText(TerrainMetadata metadata) =>
        metadata.ResolutionX == metadata.ResolutionY
            ? $"{FormatTerrainNumber(metadata.ResolutionX)} m"
            : $"{FormatTerrainNumber(metadata.ResolutionX)} / {FormatTerrainNumber(metadata.ResolutionY)} m";
}

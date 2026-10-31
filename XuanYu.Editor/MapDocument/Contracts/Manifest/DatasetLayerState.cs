namespace XuanYu.Editor.MapDocument;

public sealed record DatasetLayerState(string DatasetId, bool IsVisible, bool IsLocked, int Order)
{
    public static DatasetLayerState CreateDefault(string datasetId, int order) =>
        new(datasetId, true, false, order);
}

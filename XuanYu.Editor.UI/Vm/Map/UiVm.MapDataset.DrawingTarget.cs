using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool CanStartRegionDrawing =>
        IsRegionEditMode &&
        SelectedDataset is { Type: "区域", Status: "正常", IsLocked: false };

    void SetDatasetDrawingTarget(string? id)
    {
        var dataset = _datasetItems.FirstOrDefault(item => item.Id == id);
        if (dataset is null || dataset.Type != "区域" || dataset.Status != "正常" || dataset.IsLocked)
        {
            if (_regionDrawing.IsActive) CancelRegionDrawingFromEscape();
            if (IsRegionDrawingTool) SelectTool("选择");
            return;
        }
        MapSession.SetActiveRegionLayer(MapDatasetLayerIdProjection.Project(dataset.Id));
    }
}

namespace XuanYu.Editor.UI;

readonly record struct RegionFillColorPreview(
    InspectorEditTarget Target,
    uint OriginalRgb,
    uint PreviewRgb);

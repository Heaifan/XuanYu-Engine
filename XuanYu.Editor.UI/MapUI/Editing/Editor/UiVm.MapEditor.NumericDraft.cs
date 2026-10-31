using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    double _mapWidthDraft = 10000;
    double _mapDepthDraft = 10000;
    double _mapBaseHeightDraft;

    public double MapWidthDraft { get => _mapWidthDraft; set => SetNumericDraft(ref _mapWidthDraft, value, nameof(MapWidthDraft), "宽度"); }
    public double MapDepthDraft { get => _mapDepthDraft; set => SetNumericDraft(ref _mapDepthDraft, value, nameof(MapDepthDraft), "深度"); }
    public double MapBaseHeightDraft { get => _mapBaseHeightDraft; set => SetNumericDraft(ref _mapBaseHeightDraft, value, nameof(MapBaseHeightDraft), "基础高度"); }

    public double MapSizeMinimum => MapDefinitionValidator.MinSizeMeters;
    public double MapSizeMaximum => MapDefinitionValidator.MaxSizeMeters;
    public double MapHeightMinimum => double.MinValue;
    public double MapHeightMaximum => double.MaxValue;

    void SetNumericDraft(ref double target, double value, string property, string field)
    {
        if (target == value) return;
        target = value;
        OnPropertyChanged(property);
        OnPropertyChanged(nameof(MapStatusText));
        SetNumericDraftText(field, value);
    }

    void SetNumericDraftText(string field, double value)
    {
        var text = value.ToString("0.####", System.Globalization.CultureInfo.InvariantCulture);
        if (field == "宽度") _mapWidthText = text;
        else if (field == "深度") _mapDepthText = text;
        else _mapBaseHeightText = text;
        OnPropertyChanged(field == "宽度" ? nameof(MapWidthText) : field == "深度" ? nameof(MapDepthText) : nameof(MapBaseHeightText));
    }
}

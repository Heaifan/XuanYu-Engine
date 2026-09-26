using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class InspectorPanel
{
    void RegionColorPicker_EditStarted(object? sender, EventArgs e)
    {
        if (!TryRegionColorContext(sender, out _, out var row, out var vm)) return;
        vm.BeginRegionFillColorPreview(row.EditTarget);
    }

    void RegionColorPicker_ColorChanged(object? sender, EventArgs e)
    {
        if (!TryRegionColorContext(sender, out var picker, out var row, out var vm)) return;
        vm.PreviewRegionFillColor(row.EditTarget, InspectorColorValue.ToRgb(picker.Color));
    }

    void RegionColorPicker_EditCommitted(object? sender, EventArgs e)
    {
        if (!TryRegionColorContext(sender, out var picker, out var row, out var vm)) return;
        vm.CommitRegionFillColorPreview(row.EditTarget, InspectorColorValue.ToRgb(picker.Color));
    }

    void RegionColorPicker_EditCanceled(object? sender, EventArgs e)
    {
        if (!TryRegionColorContext(sender, out _, out var row, out var vm)) return;
        vm.CancelRegionFillColorPreview(row.EditTarget);
    }

    bool TryRegionColorContext(
        object? sender,
        out XYColorPicker picker,
        out InspectorPropertyRow row,
        out UiVm vm)
    {
        picker = sender as XYColorPicker ?? null!;
        row = picker?.DataContext as InspectorPropertyRow ?? null!;
        vm = DataContext as UiVm ?? null!;
        return picker is not null && row is not null && vm is not null && row.IsColorEditor;
    }
}

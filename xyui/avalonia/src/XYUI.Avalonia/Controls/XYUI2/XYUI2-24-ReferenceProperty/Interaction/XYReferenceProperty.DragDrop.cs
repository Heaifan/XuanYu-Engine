using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYReferenceProperty
{
    public static readonly DataFormat<XYReferenceValue> ReferenceDataFormat = DataFormat.CreateInProcessFormat<XYReferenceValue>("XYUI.Reference");
    internal void OnDragOver(object? sender, DragEventArgs e) => e.DragEffects = e.DataTransfer.TryGetValue(ReferenceDataFormat) is not null && IsEnabled && !IsReadOnly ? DragDropEffects.Copy : DragDropEffects.None;
    internal void OnDrop(object? sender, DragEventArgs e) { if (e.DataTransfer.TryGetValue(ReferenceDataFormat) is { } reference) { TryAssignReference(reference); e.Handled = true; } }
}

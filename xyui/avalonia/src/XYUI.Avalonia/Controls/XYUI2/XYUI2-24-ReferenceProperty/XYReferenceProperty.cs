using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public enum XYReferenceState { Empty, Resolved, Missing, TypeMismatch }
public sealed record XYReferenceValue(string Name, string Type, string Id)
{
    public override string ToString() => Name;
}

public partial class XYReferenceProperty : TemplatedControl
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYReferenceProperty, string>(nameof(Label), "引用");
    public static readonly StyledProperty<XYReferenceValue?> ReferenceProperty = AvaloniaProperty.Register<XYReferenceProperty, XYReferenceValue?>(nameof(Reference));
    public static readonly StyledProperty<string?> ExpectedTypeProperty = AvaloniaProperty.Register<XYReferenceProperty, string?>(nameof(ExpectedType));
    public static readonly StyledProperty<XYReferenceState> ReferenceStateProperty = AvaloniaProperty.Register<XYReferenceProperty, XYReferenceState>(nameof(ReferenceState), XYReferenceState.Empty);
    public static readonly StyledProperty<Control?> ReferencePickerContentProperty = AvaloniaProperty.Register<XYReferenceProperty, Control?>(nameof(ReferencePickerContent));
    public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<XYReferenceProperty, bool>(nameof(IsReadOnly));
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public XYReferenceValue? Reference { get => GetValue(ReferenceProperty); set => SetValue(ReferenceProperty, value); }
    public string? ExpectedType { get => GetValue(ExpectedTypeProperty); set => SetValue(ExpectedTypeProperty, value); }
    public XYReferenceState ReferenceState { get => GetValue(ReferenceStateProperty); set => SetValue(ReferenceStateProperty, value); }
    public Control? ReferencePickerContent { get => GetValue(ReferencePickerContentProperty); set => SetValue(ReferencePickerContentProperty, value); }
    public bool IsReadOnly { get => GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }
    public string ReferenceName => Reference?.Name ?? "";
    public string ReferenceType => Reference?.Type ?? "";
    public string ReferenceId => Reference?.Id ?? "";
    public bool IsPickerOpen { get; private set; }
    public event EventHandler? LocateRequested;
    public event EventHandler? BrowseRequested;
    public event EventHandler? ClearRequested;
    public event EventHandler? ReferenceChanged;
    internal TextBlock? LabelPart { get; set; }
    internal Border? ReferenceFieldPart { get; set; }
    internal TextBlock? NamePart { get; set; }
    internal TextBlock? IdentityPart { get; set; }
    internal XYIconButton? LocatePart { get; set; }
    internal XYIconButton? BrowsePart { get; set; }
    internal XYIconButton? ClearPart { get; set; }
    internal Popup? PopupPart { get; set; }
    internal bool Syncing { get; set; }

    public XYReferenceProperty() { Classes.Add("xyui-reference-property"); Focusable = true; }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ReferencePickerContentProperty) AttachPicker();
        if (change.Property == LabelProperty || change.Property == ReferenceProperty || change.Property == ExpectedTypeProperty || change.Property == ReferenceStateProperty || change.Property == IsReadOnlyProperty || change.Property == IsEnabledProperty) SyncParts();
        if (change.Property == IsEnabledProperty && !IsEnabled) ClosePicker();
    }
    public bool TryAssignReference(XYReferenceValue candidate)
    {
        if (!IsEnabled || IsReadOnly) return false;
        if (!string.IsNullOrWhiteSpace(ExpectedType) && !string.Equals(ExpectedType, candidate.Type, StringComparison.OrdinalIgnoreCase)) { ReferenceState = XYReferenceState.TypeMismatch; return false; }
        Reference = candidate; ReferenceState = XYReferenceState.Resolved; ReferenceChanged?.Invoke(this, EventArgs.Empty); ClosePicker(); return true;
    }
    public void ClearReference() { if (!IsEnabled || IsReadOnly) return; Reference = null; ReferenceState = XYReferenceState.Empty; ClearRequested?.Invoke(this, EventArgs.Empty); ReferenceChanged?.Invoke(this, EventArgs.Empty); }
    internal void SyncParts()
    {
        if (LabelPart is not null) LabelPart.Text = Label; if (NamePart is null || IdentityPart is null) return;
        NamePart.Text = Reference?.Name ?? (ReferenceState == XYReferenceState.Empty ? "未选择引用" : "当前引用"); IdentityPart.Text = IdentityText(); Classes.Set("xyui-reference-missing", ReferenceState == XYReferenceState.Missing); Classes.Set("xyui-reference-mismatch", ReferenceState == XYReferenceState.TypeMismatch);
        if (LocatePart is not null) LocatePart.IsEnabled = IsEnabled && Reference is not null && ReferenceState == XYReferenceState.Resolved;
        if (BrowsePart is not null) BrowsePart.IsEnabled = IsEnabled && !IsReadOnly; if (ClearPart is not null) ClearPart.IsEnabled = IsEnabled && !IsReadOnly && Reference is not null;
    }
    string IdentityText() => ReferenceState switch { XYReferenceState.Empty => "未设置引用", XYReferenceState.Missing => $"引用丢失 · {ReferenceType} · #{ReferenceId}", XYReferenceState.TypeMismatch => $"类型不匹配 · 需要 {ExpectedType ?? "指定类型"}", _ => $"{ReferenceType} · #{ReferenceId}" };
}

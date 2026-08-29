using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace XYUI.Avalonia.Controls;

public enum XYTextAreaMode { Standard, Editor }

public partial class XYTextArea : XyuiEditableTextBox
{
    public static readonly StyledProperty<XYTextAreaMode> ModeProperty = AvaloniaProperty.Register<XYTextArea, XYTextAreaMode>(nameof(Mode));
    public static readonly StyledProperty<string?> PlaceholderProperty = TextBox.PlaceholderTextProperty.AddOwner<XYTextArea>();
    public static readonly StyledProperty<bool> AutoGrowProperty = AvaloniaProperty.Register<XYTextArea, bool>(nameof(AutoGrow), true);
    public static readonly StyledProperty<string> EditorTypeProperty = AvaloniaProperty.Register<XYTextArea, string>(nameof(EditorType), "Plain Text");
    public static readonly StyledProperty<bool> IsErrorProperty = AvaloniaProperty.Register<XYTextArea, bool>(nameof(IsError));
    public XYTextAreaMode Mode { get => GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
    public string? Placeholder { get => GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public bool AutoGrow { get => GetValue(AutoGrowProperty); set => SetValue(AutoGrowProperty, value); }
    public string EditorType { get => GetValue(EditorTypeProperty); set => SetValue(EditorTypeProperty, value); }
    public bool IsError { get => GetValue(IsErrorProperty); set => SetValue(IsErrorProperty, value); }
    public int LineCount => string.IsNullOrEmpty(Text) ? 1 : Text!.Split('\n').Length;
    public int CharacterCount => Text?.Length ?? 0;
    internal TextPresenter? TextPresenterPart { get; private set; }
    internal ScrollViewer? ScrollViewerPart { get; private set; }
    internal Border? EditorBarPart { get; private set; }
    internal TextBlock? LineCountPart { get; private set; }
    internal TextBlock? CharacterCountPart { get; private set; }
    bool _layoutQueued;
    protected override bool SelectAllOnPointerActivation => false;
    public XYTextArea() { Classes.Add("xyui-text-area"); AcceptsReturn = true; TextWrapping = TextWrapping.Wrap; MinHeight = 54; TextChanged += OnTextChanged; }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ModeProperty) { PseudoClasses.Set(":editor", change.GetNewValue<XYTextAreaMode>() == XYTextAreaMode.Editor); QueueLayout(); }
        if (change.Property == IsErrorProperty) PseudoClasses.Set(":error", change.GetNewValue<bool>());
        if (change.Property == AutoGrowProperty || change.Property == MaxHeightProperty || change.Property == MinHeightProperty) QueueLayout();
    }

    void OnTextChanged(object? sender, TextChangedEventArgs e) { UpdateEditorBar(); QueueLayout(); }
    internal void QueueLayout() { if (VisualRoot is null || _layoutQueued) return; _layoutQueued = true; Dispatcher.UIThread.Post(UpdateTextAreaLayout); }
    void UpdateTextAreaLayout() { _layoutQueued = false; UpdateEditorBar(); if (AutoGrow && TextPresenterPart is not null) GrowToContent(); }
    void GrowToContent() { var contentHeight = TextPresenterPart!.DesiredSize.Height + 16 + (Mode == XYTextAreaMode.Editor ? 24 : 0); var target = Math.Max(MinHeight, Math.Min(MaxHeight, contentHeight)); if (double.IsFinite(target) && Math.Abs(Height - target) > 0.5) Height = target; }
    void UpdateEditorBar() { if (EditorBarPart is not null) EditorBarPart.IsVisible = Mode == XYTextAreaMode.Editor; if (LineCountPart is not null) LineCountPart.Text = $"Lines: {LineCount}"; if (CharacterCountPart is not null) CharacterCountPart.Text = $"Chars: {CharacterCount}"; }
}

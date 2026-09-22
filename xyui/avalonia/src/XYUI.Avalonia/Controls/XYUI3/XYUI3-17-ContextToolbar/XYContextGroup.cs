using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYContextGroup : ContentControl
{
    readonly StackPanel _stack = new() { Spacing = 1 };
    readonly TextBlock _headerText = new() { Classes = { "xyui-context-group-header" } };
    object? _content;
    public static readonly StyledProperty<string?> HeaderProperty = AvaloniaProperty.Register<XYContextGroup, string?>(nameof(Header));
    public static readonly StyledProperty<int> PriorityProperty = AvaloniaProperty.Register<XYContextGroup, int>(nameof(Priority));
    public static readonly StyledProperty<bool> CompactProperty = AvaloniaProperty.Register<XYContextGroup, bool>(nameof(Compact));
    public string? Header { get => GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    public int Priority { get => GetValue(PriorityProperty); set => SetValue(PriorityProperty, value); }
    public bool Compact { get => GetValue(CompactProperty); set => SetValue(CompactProperty, value); }
    public new object? Content { get => _content; set { _content = value; Refresh(); } }
    public XYContextGroup() { Classes.Add("xyui-context-group"); base.Content = _stack; }
    public XYContextGroup(string header, Control content) : this() { Header = header; Content = content; }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == HeaderProperty) Refresh();
    }
    void Refresh()
    {
        _stack.Children.Clear(); _headerText.Text = Header ?? ""; _stack.Children.Add(_headerText);
        if (_content is Control content) _stack.Children.Add(content);
    }
}

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYPagination : Border
{
    public static readonly StyledProperty<int> CurrentPageProperty = AvaloniaProperty.Register<XYPagination, int>(nameof(CurrentPage), 1);
    public static readonly StyledProperty<int> TotalPagesProperty = AvaloniaProperty.Register<XYPagination, int>(nameof(TotalPages), 1);
    public static readonly StyledProperty<int> TotalItemsProperty = AvaloniaProperty.Register<XYPagination, int>(nameof(TotalItems));
    public int CurrentPage { get => GetValue(CurrentPageProperty); set => SetValue(CurrentPageProperty, Math.Max(1, value)); }
    public int TotalPages { get => GetValue(TotalPagesProperty); set => SetValue(TotalPagesProperty, Math.Max(1, value)); }
    public int TotalItems { get => GetValue(TotalItemsProperty); set => SetValue(TotalItemsProperty, Math.Max(0, value)); }
    public bool ShowTotalItems { get; set; }
    public XYNumberField JumpInput { get; private set; } = null!;
    public XYIconButton PreviousButton { get; private set; } = null!;
    public XYIconButton NextButton { get; private set; } = null!;
    public event EventHandler<int>? PageChanged;
    public XYPagination() { Classes.Add("xyui-pagination"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property is var p && (p == CurrentPageProperty || p == TotalPagesProperty || p == TotalItemsProperty)) Build(); }
    void Build()
    {
        PreviousButton = Action(XyuiVectorIcon.ChevronLeft); NextButton = Action(XyuiVectorIcon.ChevronRight);
        PreviousButton.IsEnabled = CurrentPage > 1; NextButton.IsEnabled = CurrentPage < TotalPages;
        PreviousButton.Click += (_, _) => GoTo(CurrentPage - 1); NextButton.Click += (_, _) => GoTo(CurrentPage + 1);
        var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, VerticalAlignment = VerticalAlignment.Center };
        panel.Children.Add(PreviousButton); foreach (var page in Neighbors()) panel.Children.Add(PageButton(page)); panel.Children.Add(NextButton);
        panel.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Height = 24, Margin = new Thickness(8, 0) });
        panel.Children.Add(new TextBlock { Text = "跳至", VerticalAlignment = VerticalAlignment.Center });
        JumpInput = new XYNumberField { Width = 52, Height = 34, Minimum = 1, Maximum = TotalPages, DecimalPlaces = 0, Value = CurrentPage };
        JumpInput.KeyDown += (_, e) => { if (e.Key == global::Avalonia.Input.Key.Enter) { GoTo((int)JumpInput.Value); e.Handled = true; } }; panel.Children.Add(JumpInput);
        panel.Children.Add(new TextBlock { Text = $"页 · 共 {TotalPages} 页{(ShowTotalItems ? $" · {TotalItems} 条" : "")}", VerticalAlignment = VerticalAlignment.Center }); Child = panel;
    }
    IEnumerable<int> Neighbors() { var start = Math.Max(1, CurrentPage - 1); var end = Math.Min(TotalPages, CurrentPage + 1); return Enumerable.Range(start, end - start + 1); }
    Button PageButton(int page) { var b = new XYIconButton { Content = new TextBlock { Text = page.ToString(), HorizontalAlignment = HorizontalAlignment.Center }, Width = 38, Height = 34, IsSelected = page == CurrentPage, Classes = { "xyui-pagination-page" } }; b.Classes.Set("xyui-pagination-current", page == CurrentPage); b.Click += (_, _) => GoTo(page); return b; }
    static XYIconButton Action(XyuiVectorIcon icon) => new() { Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Small }, Width = 34, Height = 34, Classes = { "xyui-pagination-action" } };
}

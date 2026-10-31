using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Input;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYPagination : Border
{
    bool _hasExplicitPageCount;
    public static readonly StyledProperty<int> CurrentPageProperty = AvaloniaProperty.Register<XYPagination, int>(nameof(CurrentPage), 1);
    public static readonly StyledProperty<int> TotalPagesProperty = AvaloniaProperty.Register<XYPagination, int>(nameof(TotalPages), 1);
    public static readonly StyledProperty<int> TotalItemsProperty = AvaloniaProperty.Register<XYPagination, int>(nameof(TotalItems));
    public int CurrentPage { get => GetValue(CurrentPageProperty); set => SetValue(CurrentPageProperty, Math.Max(1, value)); }
    public int TotalPages { get => GetValue(TotalPagesProperty); set { _hasExplicitPageCount = true; SetValue(TotalPagesProperty, Math.Max(1, value)); } }
    public int PageCount { get => TotalPages; set => TotalPages = value; }
    public int TotalItems { get => GetValue(TotalItemsProperty); set => SetValue(TotalItemsProperty, Math.Max(0, value)); }
    public bool ShowTotalItems { get; set; }
    public XYNumberField JumpInput { get; private set; } = null!;
    public XYIconButton FirstButton { get; private set; } = null!;
    public XYIconButton PreviousButton { get; private set; } = null!;
    public XYIconButton NextButton { get; private set; } = null!;
    public XYIconButton LastButton { get; private set; } = null!;
    public IReadOnlyList<XYIconButton> PageButtons { get; private set; } = [];
    public event EventHandler<int>? PageChanged;
    public event EventHandler<int>? InvalidPageRequested;
    public XYPagination() { Classes.Add("xyui-pagination"); Focusable = true; KeyDown += OnKeyDown; Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property == TotalPagesProperty || (_hasExplicitPageCount && e.Property == CurrentPageProperty)) if (CurrentPage > TotalPages) SetValue(CurrentPageProperty, TotalPages); if (e.Property is var changed && (changed == CurrentPageProperty || changed == TotalPagesProperty || changed == TotalItemsProperty)) Build(); }
    void Build()
    {
        FirstButton = Action(XyuiVectorIcon.ChevronLeft, "xyui-pagination-first"); PreviousButton = Action(XyuiVectorIcon.ChevronLeft, "xyui-pagination-previous"); NextButton = Action(XyuiVectorIcon.ChevronRight, "xyui-pagination-next"); LastButton = Action(XyuiVectorIcon.ChevronRight, "xyui-pagination-last");
        FirstButton.IsEnabled = PreviousButton.IsEnabled = CurrentPage > 1; NextButton.IsEnabled = LastButton.IsEnabled = CurrentPage < TotalPages;
        FirstButton.Click += (_, _) => First(); PreviousButton.Click += (_, _) => Previous(); NextButton.Click += (_, _) => Next(); LastButton.Click += (_, _) => Last();
        var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, VerticalAlignment = VerticalAlignment.Center };
        panel.Children.Add(FirstButton); panel.Children.Add(PreviousButton); var buttons = new List<XYIconButton>(); foreach (var page in VisiblePages()) { if (page is null) panel.Children.Add(EllipsisSlot()); else { var button = PageButton(page.Value, PageSlotWidth()); buttons.Add(button); panel.Children.Add(button); } } PageButtons = buttons; panel.Children.Add(NextButton); panel.Children.Add(LastButton);
        panel.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Height = 24, Margin = new Thickness(8, 0) });
        panel.Children.Add(new TextBlock { Text = "跳至", VerticalAlignment = VerticalAlignment.Center });
        JumpInput = new XYNumberField { Width = 52, Height = 34, Minimum = 1, Maximum = TotalPages, DecimalPlaces = 0, Value = CurrentPage };
        JumpInput.KeyDown += (_, e) => { if (e.Key == global::Avalonia.Input.Key.Enter) { GoTo((int)JumpInput.Value); e.Handled = true; } }; panel.Children.Add(JumpInput);
        panel.Children.Add(new TextBlock { Text = $"页 · 共 {TotalPages} 页{(ShowTotalItems ? $" · {TotalItems} 条" : "")}", VerticalAlignment = VerticalAlignment.Center }); Child = panel;
    }
    IEnumerable<int?> VisiblePages()
    {
        if (TotalPages <= 7) { foreach (var page in Enumerable.Range(1, TotalPages)) yield return page; yield break; }
        if (CurrentPage <= 3) { foreach (var page in new int?[] { 1, 2, 3, 4, 5, null, TotalPages }) yield return page; yield break; }
        if (CurrentPage >= TotalPages - 2) { foreach (var page in new int?[] { 1, null, TotalPages - 4, TotalPages - 3, TotalPages - 2, TotalPages - 1, TotalPages }) yield return page; yield break; }
        foreach (var page in new int?[] { 1, null, CurrentPage - 1, CurrentPage, CurrentPage + 1, null, TotalPages }) yield return page;
    }
    double PageSlotWidth() => Math.Max(38, 20 + TotalPages.ToString().Length * 10);
    XYIconButton PageButton(int page, double width) { var b = new XYIconButton { Content = new TextBlock { Text = page.ToString(), HorizontalAlignment = HorizontalAlignment.Center }, Width = width, Height = 34, IsSelected = page == CurrentPage, Classes = { "xyui-pagination-page" } }; b.Classes.Set("xyui-pagination-current", page == CurrentPage); b.Click += (_, _) => GoTo(page); return b; }
    Control EllipsisSlot() => new Border { Width = PageSlotWidth(), Height = 34, Child = new TextBlock { Text = "…", Classes = { "xyui-pagination-ellipsis" }, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } };
    static XYIconButton Action(XyuiVectorIcon icon, string @class) => new() { Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Small }, Width = 34, Height = 34, Classes = { "xyui-pagination-action", @class } };
    void OnKeyDown(object? sender, KeyEventArgs e) { if (e.Key == Key.Left) Previous(); else if (e.Key == Key.Right) Next(); else if (e.Key == Key.Home) First(); else if (e.Key == Key.End) Last(); else return; e.Handled = true; }
}

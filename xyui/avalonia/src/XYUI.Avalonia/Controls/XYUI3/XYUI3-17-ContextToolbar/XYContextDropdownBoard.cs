using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed class XYContextDropdownBoard : Border
{
    readonly IReadOnlyDictionary<string, IReadOnlyList<XYContextAction>> _actions;
    readonly Border _surface = new() { Classes = { "xyui-context-board-surface" } };
    readonly Grid _grid = new();
    public Popup Popup { get; } = new() { Placement = PlacementMode.Bottom, IsLightDismissEnabled = true };
    public XYContextCategoryList CategoryList { get; }
    public XYContextActionPane ActionPane { get; } = new();
    public XYContextHintBar HintBar { get; } = new();
    public string Header { get; }
    public bool IsOpen { get => Popup.IsOpen; set => Popup.IsOpen = value; }
    public XYContextCategory? SelectedCategory => CategoryList.SelectedCategory;
    public XYContextAction? SelectedAction => ActionPane.SelectedAction;
    public event EventHandler<XYContextAction>? ActionExecuted;
    public XYContextDropdownBoard(string header, IEnumerable<XYContextCategory> categories, IReadOnlyDictionary<string, IReadOnlyList<XYContextAction>> actions)
    {
        Header = header; _actions = actions; Classes.Add("xyui-context-dropdown-board"); CategoryList = new XYContextCategoryList(categories); CategoryList.SelectionChanged += (_, category) => RefreshActions(category); ActionPane.ActionExecuted += (_, action) => ActionExecuted?.Invoke(this, action); Popup.Child = _surface; Child = new Border { Width = 1, Height = 1 }; Refresh(); CategoryList.Select(CategoryList.Categories.FirstOrDefault()?.Id ?? "");
    }
    public void Open(Control? target = null) { Popup.PlacementTarget = target ?? this; Popup.IsOpen = true; Focus(); }
    public void Close() => Popup.IsOpen = false;
    public void Toggle(Control? target = null) { if (IsOpen) Close(); else Open(target); }
    public void DismissOutside() => Close();
    public void SelectCategory(string id) => CategoryList.Select(id);
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape) Close(); else if (e.Key == Key.Down) ActionPane.Move(1); else if (e.Key == Key.Up) ActionPane.Move(-1); else if (e.Key == Key.Left) SelectRelative(-1); else if (e.Key == Key.Right) SelectRelative(1); else if (e.Key == Key.Enter) ActionPane.ExecuteSelected(); else { base.OnKeyDown(e); return; } e.Handled = true;
    }
    void SelectRelative(int delta) { var categories = CategoryList.Categories; var index = Math.Max(0, Array.IndexOf(categories.ToArray(), SelectedCategory) + delta); index = Math.Min(index, categories.Count - 1); CategoryList.Select(categories[index].Id); }
    void RefreshActions(XYContextCategory category) => ActionPane.SetActions(_actions.TryGetValue(category.Id, out var actions) ? actions : []);
    void Refresh()
    {
        _grid.Children.Clear(); _grid.RowDefinitions = new RowDefinitions("Auto,*,Auto"); _grid.Children.Add(new TextBlock { Text = Header, Classes = { "xyui-context-board-header" } }); var body = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,1,*"), Children = { CategoryList, new XYSeparator { Variant = XyuiSeparatorVariant.VerticalSplit, Height = 92 }, ActionPane } }; Grid.SetColumn(body.Children[1], 1); Grid.SetColumn(ActionPane, 2); Grid.SetRow(body, 1); _grid.Children.Add(body); Grid.SetRow(HintBar, 2); _grid.Children.Add(HintBar); _surface.Child = _grid;
    }
}

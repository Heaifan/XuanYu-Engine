using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYContextCategoryList : Border
{
    readonly StackPanel _panel = new();
    public IReadOnlyList<XYContextCategory> Categories { get; }
    public XYContextCategory? SelectedCategory { get; private set; }
    public event EventHandler<XYContextCategory>? SelectionChanged;
    public XYContextCategoryList(IEnumerable<XYContextCategory> categories)
    {
        Categories = categories.ToArray(); Classes.Add("xyui-context-category-list"); Child = _panel; Refresh();
    }
    public void Select(string id)
    {
        var category = Categories.FirstOrDefault(x => x.Id == id); if (category is null || category == SelectedCategory) return;
        SelectedCategory = category; Refresh(); SelectionChanged?.Invoke(this, category);
    }
    void Refresh()
    {
        _panel.Children.Clear();
        foreach (var category in Categories)
        {
            var button = new XYButton { Content = category.Label, Tag = category.Id, Variant = XyuiButtonVariant.Secondary, Classes = { "xyui-context-category" }, Height = 32, HorizontalContentAlignment = HorizontalAlignment.Left };
            if (category == SelectedCategory) button.Classes.Add("xyui-context-category-selected"); button.Click += (_, _) => Select(category.Id); _panel.Children.Add(button);
        }
    }
}

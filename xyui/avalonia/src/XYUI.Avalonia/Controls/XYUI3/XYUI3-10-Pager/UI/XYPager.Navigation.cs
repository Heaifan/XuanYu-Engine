using Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYPager
{
    public void Select(string id)
    {
        var index = Pages.ToList().FindIndex(x => x.Id == id);
        if (index >= 0) Select(index, true);
    }

    public void Previous() { if (IsPreviousEnabled) Select(_selectedIndex - 1, true); }
    public void Next() { if (IsNextEnabled) Select(_selectedIndex + 1, true); }

    void Select(int index, bool raise)
    {
        var page = PageAt(index); if (page?.Content is null || index == _selectedIndex) return;
        _selectedIndex = index; _tabs.Children.Clear(); _content.Children.Clear();
        foreach (var candidate in Pages)
        {
            var content = candidate.Content; if (content is null) continue;
            var button = new XYButton { Content = candidate.Label, Classes = { "xyui-pager-page" }, IsEnabled = candidate != page };
            button.Click += (_, _) => Select(candidate.Id); _tabs.Children.Add(button);
            content.IsVisible = candidate == page; _content.Children.Add(content);
        }
        _indicator.Text = $"{index + 1} / {Pages.Count}";
        if (raise) SelectionChanged?.Invoke(this, page);
    }
}

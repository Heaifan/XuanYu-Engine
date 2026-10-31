using Avalonia.Controls;
using Avalonia.Metadata;

namespace XYUI.Avalonia.Controls;

public sealed class XYPagerPage
{
    public string Id { get; set; } = "";
    public string Label { get; set; } = "";
    [Content] public Control? Content { get; set; }

    public XYPagerPage() { }
    public XYPagerPage(string id, string label, Control content)
    {
        Id = id; Label = label; Content = content;
    }
}

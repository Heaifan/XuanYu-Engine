using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public partial class XYSearchField
{
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Enter) { RequestSearch(); e.Handled = true; }
        else if (e.Key == Key.Escape && !string.IsNullOrEmpty(Text)) { ClearSearch(); e.Handled = true; }
        base.OnKeyDown(e);
    }
}

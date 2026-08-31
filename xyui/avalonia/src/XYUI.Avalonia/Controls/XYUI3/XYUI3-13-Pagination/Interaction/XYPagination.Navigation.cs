namespace XYUI.Avalonia.Controls;

public sealed partial class XYPagination
{
    public void GoTo(int page)
    {
        var next = Math.Clamp(page, 1, Math.Max(1, TotalPages));
        if (next == CurrentPage) return;
        CurrentPage = next; PageChanged?.Invoke(this, next);
    }
}

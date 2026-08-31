namespace XYUI.Avalonia.Controls;

public sealed partial class XYPagination
{
    public void GoTo(int page)
    {
        if (page < 1 || page > TotalPages) { InvalidPageRequested?.Invoke(this, page); return; }
        var next = page;
        if (next == CurrentPage) return;
        CurrentPage = next; PageChanged?.Invoke(this, next);
    }
}

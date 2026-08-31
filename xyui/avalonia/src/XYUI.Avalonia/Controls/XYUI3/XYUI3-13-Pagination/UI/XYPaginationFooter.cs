using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYPaginationFooter : Border
{
    public XYPagination Pagination { get; }
    public XYSelect PageSize { get; }
    public XYPaginationFooter(int totalItems = 468, int totalPages = 24)
    {
        Classes.Add("xyui-pagination-footer"); Pagination = new XYPagination { TotalItems = totalItems, TotalPages = totalPages, CurrentPage = 3, ShowTotalItems = false };
        PageSize = new XYSelect { Width = 68, Height = 34, ItemsSource = new[] { 25, 50, 100 }, SelectedIndex = 0 };
        Child = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,Auto,*,Auto"), Children = { new TextBlock { Text = $"共 {totalItems} 条", VerticalAlignment = VerticalAlignment.Center }, new TextBlock { Text = "每页", VerticalAlignment = VerticalAlignment.Center, Margin = new global::Avalonia.Thickness(16, 0, 4, 0), [Grid.ColumnProperty] = 1 }, PageSize, Pagination } };
        Grid.SetColumn(PageSize, 2); Grid.SetColumn(Pagination, 3);
    }
}

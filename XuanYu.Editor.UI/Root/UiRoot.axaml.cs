using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class UiRoot : UserControl
{
    bool _clamping;

    public UiRoot()
    {
        InitializeComponent();
        SizeChanged += (_, _) => ClampLayout();
        LeftColumn.PropertyChanged += (_, _) => ClampLayout();
        RightColumn.PropertyChanged += (_, _) => ClampLayout();
        ClampLayout();
    }

    ColumnDefinition LeftColumn => MainLayoutGrid.ColumnDefinitions[0];
    ColumnDefinition RightColumn => MainLayoutGrid.ColumnDefinitions[4];

    void ClampLayout()
    {
        if (_clamping) return;
        _clamping = true;
        ClampColumn(LeftColumn, 270, 200, 420);
        ClampColumn(RightColumn, 340, 260, 480);
        _clamping = false;
    }

    static void ClampColumn(ColumnDefinition column, double fallback, double min, double max)
    {
        var width = column.Width.IsStar ? fallback : column.Width.Value;
        var clamped = Math.Clamp(width, min, max);
        if (Math.Abs(width - clamped) < 0.5) return;
        column.Width = new GridLength(clamped);
    }
}

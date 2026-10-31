using Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery.Views;

public partial class IconographyOpticalSection : UserControl
{
    public IconographyOpticalSection()
    {
        InitializeComponent();
        SetCard(TxtChevronStatus, XyuiVectorIcon.ChevronRight);
        SetCard(TxtEyeStatus, XyuiVectorIcon.Eye);
        SetCard(TxtLocateStatus, XyuiVectorIcon.Locate);
        SetCard(TxtCodeStatus, XyuiVectorIcon.Code);
    }

    static void SetCard(TextBlock tb, XyuiVectorIcon icon)
    {
        var m = XyuiVectorIcons.GetMetrics(icon);
        var center = $"{m.LogicalViewport / 2:0},{m.LogicalViewport / 2:0}";
        var offset = $"{m.OpticalOffset.X:0},{m.OpticalOffset.Y:0}";
        tb.Text = $"{center} · Offset {offset} · ✓";
    }
}

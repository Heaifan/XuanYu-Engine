using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery.Views;

public partial class SpacingLiveLabSection : UserControl
{
    public SpacingLiveLabSection()
    {
        InitializeComponent();
        ToolbarHost.Children.Add(new XYToolbar(
            new XYToolGroup(
                new XYButton { Content = "新建", Variant = XyuiButtonVariant.Secondary },
                new XYButton { Content = "保存", Variant = XyuiButtonVariant.Secondary }),
            new XYToolGroup(
                new XYButton { Content = "撤销", Variant = XyuiButtonVariant.Secondary },
                new XYButton { Content = "重做", Variant = XyuiButtonVariant.Secondary })));
    }
}
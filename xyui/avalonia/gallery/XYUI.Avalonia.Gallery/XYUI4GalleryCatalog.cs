using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI4GalleryCatalog
{
    public static Control CreatePreview(string id) => id switch
    {
        "XYUI-4-4.14" => LoadingPreview(),
        "XYUI-4-4.15" => SpinnerPreview(),
        "XYUI-4-4.16" => ProgressBarPreview(),
        _ => new TextBlock { Text = "未注册组件" }
    };

    public static Control CreateLiveExamples(string id) => id switch
    {
        "XYUI-4-4.14" => LoadingLiveExample(),
        "XYUI-4-4.15" => SpinnerLiveExample(),
        "XYUI-4-4.16" => ProgressBarLiveExample(),
        _ => new TextBlock { Text = "未注册组件" }
    };

    static Control LoadingPreview() => new StackPanel
    {
        Spacing = 10,
        Children = { new XYLoadingIndicator { Text = "正在初始化渲染视口…", Size = XyuiSpinnerSize.Compact },
            new XYLoadingIndicator { Text = "正在读取场景资源", SecondaryText = "Vulkan 后端", Variant = XyuiLoadingIndicatorVariant.Detail } }
    };

    static Control SpinnerPreview() => new StackPanel
    {
        Orientation = Orientation.Horizontal, Spacing = 22,
        Children = { Sample(XyuiSpinnerSize.Compact, "Compact"), Sample(XyuiSpinnerSize.Standard, "Standard"), Sample(XyuiSpinnerSize.Large, "Large") }
    };

    static Control Sample(XyuiSpinnerSize size, string label) => new StackPanel
    {
        Spacing = 6, HorizontalAlignment = HorizontalAlignment.Center,
        Children = { new XYSpinner { Size = size }, new XYCaption { Text = label } }
    };

    static Control LoadingLiveExample()
    {
        var indicator = new XYLoadingIndicator { Text = "正在初始化渲染视口…", SecondaryText = "Indeterminate · Area C", Variant = XyuiLoadingIndicatorVariant.Detail };
        var state = new TextBlock { Text = "活动状态：运行中", Classes = { "xyui-text-caption" } };
        var toggle = new XYButton { Content = "切换活动状态", Variant = XyuiButtonVariant.Secondary };
        toggle.Click += (_, _) => { indicator.IsActive = !indicator.IsActive; state.Text = $"活动状态：{(indicator.IsActive ? "运行中" : "已暂停")}"; };
        return new StackPanel { Spacing = 10, Children = { indicator, state, toggle } };
    }

    static Control SpinnerLiveExample()
    {
        var spinner = new XYSpinner { Size = XyuiSpinnerSize.Standard };
        var state = new TextBlock { Text = "动效：旋转中", Classes = { "xyui-text-caption" } };
        var toggle = new XYButton { Content = "切换 Reduced Motion", Variant = XyuiButtonVariant.Secondary };
        toggle.Click += (_, _) => { spinner.IsReducedMotion = !spinner.IsReducedMotion; state.Text = $"动效：{(spinner.IsReducedMotion ? "静态弧" : "旋转中")}"; };
        return new StackPanel { Spacing = 10, Children = { spinner, state, toggle } };
    }
}

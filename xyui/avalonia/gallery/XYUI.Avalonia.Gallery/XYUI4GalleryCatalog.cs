using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI4GalleryCatalog
{
    public static Control CreatePreview(string id) => id switch
    {
        "XYUI-4-4.01" => HoverPreview(), "XYUI-4-4.02" => SelectedPreview(), "XYUI-4-4.03" => ActivePreview(), "XYUI-4-4.04" => FocusPreview(), "XYUI-4-4.05" => MultiSelectionPreview(), "XYUI-4-4.06" => SelectionGroupPreview(), "XYUI-4-4.07" => MarqueePreview(), "XYUI-4-4.08" => LassoPreview(), "XYUI-4-4.09" => SelectionOutlinePreview(), "XYUI-4-4.10" => BoundingBoxPreview(), "XYUI-4-4.11" => DragFeedbackPreview(), "XYUI-4-4.12" => DropIndicatorPreview(), "XYUI-4-4.13" => InsertionIndicatorPreview(),
        "XYUI-4-4.14" => LoadingPreview(),
        "XYUI-4-4.15" => SpinnerPreview(),
        "XYUI-4-4.16" => ProgressBarPreview(),
        _ => new TextBlock { Text = "未注册组件" }
    };

    public static Control CreateLiveExamples(string id) => id switch
    {
        "XYUI-4-4.01" => HoverLiveExample(), "XYUI-4-4.02" => SelectedLiveExample(), "XYUI-4-4.03" => ActiveLiveExample(), "XYUI-4-4.04" => FocusLiveExample(), "XYUI-4-4.05" => MultiSelectionLiveExample(), "XYUI-4-4.06" => SelectionGroupLiveExample(), "XYUI-4-4.07" => MarqueeLiveExample(), "XYUI-4-4.08" => LassoLiveExample(), "XYUI-4-4.09" => SelectionOutlineLiveExample(), "XYUI-4-4.10" => BoundingBoxLiveExample(), "XYUI-4-4.11" => DragFeedbackLiveExample(), "XYUI-4-4.12" => DropIndicatorLiveExample(), "XYUI-4-4.13" => InsertionIndicatorLiveExample(),
        "XYUI-4-4.14" => LoadingLiveExample(),
        "XYUI-4-4.15" => SpinnerLiveExample(),
        "XYUI-4-4.16" => ProgressBarLiveExample(),
        _ => new TextBlock { Text = "未注册组件" }
    };

    static Control HoverPreview() => new StackPanel { Spacing = 8, Children =
    {
        SampleHover(XyuiHoverStateVariant.Surface, "Surface"), SampleHover(XyuiHoverStateVariant.Border, "Border"),
        SampleHover(XyuiHoverStateVariant.Outline, "Outline"), SampleHover(XyuiHoverStateVariant.Handle, "Handle")
    } };

    static Control SampleHover(XyuiHoverStateVariant variant, string label) => new XYHoverState
    {
        Variant = variant, IsHovered = true, Child = new XYCaption { Text = $"{label} · Hover" }, Width = 220, Height = 28
    };

    static Control HoverLiveExample()
    {
        var state = new XYHoverState { Variant = XyuiHoverStateVariant.Surface, Child = new XYCaption { Text = "移动指针观察 Hover" }, Width = 220, Height = 32 };
        var selected = new XYHoverState { Variant = XyuiHoverStateVariant.Surface, IsSelected = true, Child = new XYCaption { Text = "Selected + Hover" }, Width = 220, Height = 32 };
        var disabled = new XYHoverState { Variant = XyuiHoverStateVariant.Surface, IsEnabled = false, Child = new XYCaption { Text = "Disabled" }, Width = 220, Height = 32 };
        return new StackPanel { Spacing = 8, Children = { state, selected, disabled } };
    }

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

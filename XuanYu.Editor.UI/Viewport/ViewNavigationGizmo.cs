using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace XuanYu.Editor.UI;

// F3-D2/D3：Blender 风格导航 Gizmo 主控件（88×88 透明覆盖层，无白色底板）。
// 数据源：UiVm.NavigationCamera（相机姿态快照）；命令：UiVm.RunCommand("视角-+X 视图")；
// Orbit：UiVm.BeginCameraNavigation / PreviewCameraNavigation / EndCameraNavigation。
// 拆分职责：Layout（投影数学）/ Render（绘制）/ HitTest（命中）/ Input（指针与命令）。
// 控件自身 88×88 边界内截获输入（中心球/空白 = Orbit 拖动）；边界外点击自然落入视口。
public sealed partial class ViewNavigationGizmo : Control
{
    public static readonly StyledProperty<NavigationCameraSnapshot?> NavigationCameraProperty =
        AvaloniaProperty.Register<ViewNavigationGizmo, NavigationCameraSnapshot?>(nameof(NavigationCamera));

    public NavigationCameraSnapshot? NavigationCamera
    {
        get => GetValue(NavigationCameraProperty);
        set => SetValue(NavigationCameraProperty, value);
    }

    public ViewNavigationGizmo()
    {
        Width = NavigationGizmoLayout.GizmoSize;
        Height = NavigationGizmoLayout.GizmoSize;
        ClipToBounds = true;
        IsHitTestVisible = true;
        Cursor = new Cursor(StandardCursorType.Hand);
        PropertyChanged += (_, e) =>
        {
            if (e.Property == NavigationCameraProperty) InvalidateVisual();
        };
    }

    Point Center => new(Width * 0.5, Height * 0.5);

    string? HoverEndpoint => _hoverEndpoint;
}

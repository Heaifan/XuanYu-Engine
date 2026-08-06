using System;
using System.ComponentModel;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class UiRoot : UserControl
{
    const double MainRowMinHeight = 320;   // Row1 主工作区最低高度（与 axaml MinHeight 一致）
    const double SplitterHeight = 6;       // Row2 分隔条
    const double RootMargin = 12;          // 根 Grid Margin 6*2（D3：与最小窗口 1024×640 对齐）
    const double LogRowPreferred = 420;    // 日志区期望高度上限（与 axaml MaxHeight 一致）
    const double LogRowFloor = 120;        // 矮窗口下日志区最低可压缩高度（展开态最小 120，规范 §7.1）

    bool _clamping;
    bool _vmHooked;

    public UiRoot()
    {
        InitializeComponent();
        SizeChanged += (_, _) => ClampLayout();
        LeftColumn.PropertyChanged += (_, _) => ClampLayout();
        RightColumn.PropertyChanged += (_, _) => ClampLayout();
        DataContextChanged += (_, _) => HookVm();
        ClampLayout();
    }

    void HookVm()
    {
        if (DataContext is not UiVm vm || _vmHooked) return;
        _vmHooked = true;
        vm.PropertyChanged += OnVmPropertyChanged;
        ClampLayout(); // DataContext 就位时 IsLogOpen 可能已是 true（初始展开态）
    }

    // F4：日志展开/折叠 → 重新分配日志行高度（窗口尺寸变化由 SizeChanged 覆盖）。
    void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(UiVm.IsLogOpen)) ClampLayout();
    }

    ColumnDefinition LeftColumn => MainLayoutGrid.ColumnDefinitions[0];
    ColumnDefinition RightColumn => MainLayoutGrid.ColumnDefinitions[4];

    void ClampLayout()
    {
        if (_clamping) return;
        _clamping = true;
        ClampColumn(LeftColumn, 270, 220, 420);
        ClampColumn(RightColumn, 340, 300, 480);
        ClampLogRow();
        _clamping = false;
    }

    // F4：日志区垂直尺寸自适应——展开时按窗口可用高度 Clamp 为像素行，
    // 折叠时回 Auto（只占标题栏）。修复 A4：Auto+MaxHeight=420 与主区 MinHeight=320
    // 最小和超过矮窗口可用高度 → 日志区被底部裁切（外部布局边界，滚动控制器救不了）。
    void ClampLogRow()
    {
        var rows = RootGrid.RowDefinitions;
        if (DataContext is not UiVm { IsLogOpen: true })
        {
            rows[3].Height = GridLength.Auto;
            return;
        }
        if (Bounds.Height <= 0) return;
        var available = Bounds.Height - RootMargin - SplitterHeight - MainRowMinHeight
            - rows[0].ActualHeight;
        var logHeight = Math.Clamp(LogRowPreferred, LogRowFloor, available);
        if (logHeight <= 0) return; // 极端矮窗口：保持现状，由 MinHeight=32 兜底
        rows[3].Height = new GridLength(logHeight);
    }

    static void ClampColumn(ColumnDefinition column, double fallback, double min, double max)
    {
        var width = column.Width.IsStar ? fallback : column.Width.Value;
        var clamped = Math.Clamp(width, min, max);
        if (Math.Abs(width - clamped) < 0.5) return;
        column.Width = new GridLength(clamped);
    }
}

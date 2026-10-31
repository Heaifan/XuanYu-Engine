using System;
using System.Collections.Generic;
using System.Linq;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D3：顶层页签条纯布局状态机（无 Avalonia 依赖，可直接单测）。
// 合同：UI Spec 1.0 §10.1（单行/箭头/渐隐/当前页签可见/滚轮横向路由/一次性提示门）。
// 所有计算均为纯函数；Avalonia 接线在 TopTabStripController。
public sealed class TopTabStripModel
{
    public const double Epsilon = 0.5;
    public const double ScrollStep = 96;   // 与 Token Size.Width.96 对齐：箭头单击步进

    public double ExtentWidth { get; private set; }
    public double ViewportWidth { get; private set; }
    public double OffsetX { get; private set; }

    public bool Overflowing => ExtentWidth - ViewportWidth > Epsilon;
    public bool CanScrollLeft => OffsetX > Epsilon;
    public bool CanScrollRight => Overflowing && OffsetX < MaxOffset - Epsilon;
    public bool FadeLeft => CanScrollLeft;    // 合同：隐藏方向显示轻微渐隐
    public bool FadeRight => CanScrollRight;
    public double MaxOffset => Math.Max(0, ExtentWidth - ViewportWidth);

    public void Update(double extent, double viewport, double offset)
    {
        ExtentWidth = Math.Max(0, extent);
        ViewportWidth = Math.Max(0, viewport);
        OffsetX = Clamp(offset);
    }

    public double Clamp(double offset) => Math.Clamp(offset, 0, MaxOffset);
    public double ScrollLeft() => Clamp(OffsetX - ScrollStep);
    public double ScrollRight() => Clamp(OffsetX + ScrollStep);

    // 滚轮垂直增量 → 横向位移（1:1 像素；上滚 = 向左看，Windows 水平滚动条惯例）。
    public double WheelDelta(double deltaY) => deltaY;

    public bool IsFullyVisible(double itemLeft, double itemWidth) =>
        itemLeft >= OffsetX - Epsilon
        && itemLeft + itemWidth <= OffsetX + ViewportWidth + Epsilon;

    // 使指定页签完整可见所需的目标偏移；已完全可见时保持不动。
    public double OffsetForVisible(double itemLeft, double itemWidth)
    {
        if (IsFullyVisible(itemLeft, itemWidth)) return OffsetX;
        if (itemLeft < OffsetX) return Clamp(itemLeft);
        return Clamp(itemLeft + itemWidth - ViewportWidth);
    }

    // 一次性提示门：仅当「当前溢出 ∧ 本次会话未显示 ∧ 该用户环境未持久化」时返回 true。
    public static bool ShouldShowHint(bool overflowing, bool shownThisSession, bool persisted) =>
        overflowing && !shownThisSession && !persisted;

    // 「全部页签」列表：真实页签集合 + 当前项标记（合同：列出所有真实页签、标明当前项）。
    public sealed record TabListItem(string Header, int Index, bool IsSelected);

    public static IReadOnlyList<TabListItem> BuildTabList(string[] headers, int selectedIndex) =>
        headers.Select((h, i) => new TabListItem(h, i, i == selectedIndex)).ToArray();
}

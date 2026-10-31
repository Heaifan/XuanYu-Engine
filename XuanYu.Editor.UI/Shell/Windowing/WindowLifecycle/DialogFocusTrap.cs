namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5（纠偏）：弹窗焦点陷阱的纯逻辑（可脱离 UI 测试）。
// 给定可聚焦控件数量与当前索引，返回 Tab/Shift+Tab 的下一个索引（环形循环，不离开弹窗）。
public static class DialogFocusTrap
{
    public static int NextIndex(int count, int currentIndex, bool reverse)
    {
        if (count <= 0) return -1;
        if (currentIndex < 0 || currentIndex >= count) return reverse ? count - 1 : 0;
        if (reverse) return (currentIndex - 1 + count) % count;
        return (currentIndex + 1) % count;
    }
}

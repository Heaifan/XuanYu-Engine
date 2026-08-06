namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5（纠偏）：日志空状态——严格互斥的两类空态：
//  ShowInitialLogEmpty（「全部」筛选且无日志 → 暂无日志）
//  ShowNoFilterResults（非「全部」筛选且无结果 → 没有匹配的日志）
public sealed partial class UiVm
{
    public bool HasNoLogItems => LogItems.Count == 0;
    public bool ShowInitialLogEmpty => IsLogFilterAll && HasNoLogItems;
    public bool ShowNoFilterResults => !IsLogFilterAll && HasNoLogItems;
}

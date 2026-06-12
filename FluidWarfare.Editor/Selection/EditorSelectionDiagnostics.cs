using System.Text;

namespace FluidWarfare.Editor.Selection;

/// <summary>
/// 选择系统诊断计数器，用于验证反馈循环和数据流正确性。
/// </summary>
public sealed class EditorSelectionDiagnostics
{
    public int SelectionRequestCount { get; set; }
    public int SelectionChangeCount { get; set; }
    public int SelectionNoOpCount { get; set; }
    public int HierarchyRevealCount { get; set; }
    public int HierarchyRebuildCount { get; set; }
    public int SceneSelectionFrameCount { get; set; }
    public int FeedbackLoopBlockedCount { get; set; }
    public long LastRevision { get; set; }

    public string GetReport()
    {
        var sb = new StringBuilder();
        sb.AppendLine("选择系统诊断报告");
        sb.AppendLine($"  SelectionRequestCount:     {SelectionRequestCount}");
        sb.AppendLine($"  SelectionChangeCount:      {SelectionChangeCount}");
        sb.AppendLine($"  SelectionNoOpCount:        {SelectionNoOpCount}");
        sb.AppendLine($"  HierarchyRevealCount:      {HierarchyRevealCount}");
        sb.AppendLine($"  HierarchyRebuildCount:     {HierarchyRebuildCount}");
        sb.AppendLine($"  SceneSelectionFrameCount:  {SceneSelectionFrameCount}");
        sb.AppendLine($"  FeedbackLoopBlockedCount:  {FeedbackLoopBlockedCount}");
        sb.AppendLine($"  LastRevision:              {LastRevision}");
        return sb.ToString();
    }
}

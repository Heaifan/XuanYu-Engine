using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4/D4-F1 纠偏：双模型并存——
//  MapEditorLayoutModel（<320 面板紧凑密度：根 Padding/分组间距/字段行距）。
// 只读资产摘要保持单行双列，不参与任何切换；标准/窄模式共享同一 UiVm 状态。
public partial class MapPagePanel : UserControl
{
    readonly IReadOnlyList<Grid> _summaryRows;

    public MapPagePanel()
    {
        InitializeComponent();
        _summaryRows = SummaryGroup.Children.OfType<Grid>().ToList();
        SizeChanged += (_, _) => ApplyModes();
        ApplyModes();
    }

    void ApplyModes()
    {
        var width = Bounds.Width;
        var compact = MapEditorLayoutModel.ModeFor(width) == MapEditorDensityMode.Compact;

        // 密度合同（纠偏 v2）：根水平留白 12/8、分组间距 12/8、字段行距 6/4
        Root.Margin = new Thickness(compact ? 8 : 12, 6, compact ? 8 : 12, 0);
        Root.Spacing = compact ? 8 : 12;
        foreach (var row in _summaryRows)
            row.Margin = new Thickness(0, compact ? 2 : 3);
    }

}

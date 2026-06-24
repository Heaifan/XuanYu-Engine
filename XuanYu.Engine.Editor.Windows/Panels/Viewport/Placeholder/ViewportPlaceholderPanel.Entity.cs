namespace XuanYu.Engine.Editor.Windows.Panels.Viewport;

/// <summary>Partial：实体摘要与空状态显示。</summary>
sealed partial class ViewportPlaceholderPanel
{
    public void ShowNoWorldEntity() => ShowSinglePanel(_defaultContent);
    public void ShowEmptyWorld() => ShowSinglePanel(_emptyWorldContent);

    public void ShowEntitySummary(ViewportEntitySummary summary)
    {
        if (_entityTypeLabel is not null)
            _entityTypeLabel.Text = summary.SourcePath is not null ? "当前选中 World 占位实体：" : "当前选中 World 实体：";
        if (_entityNameText is not null) _entityNameText.Text = summary.DisplayName;
        if (_entityIdText is not null) _entityIdText.Text = summary.EntityIdText;
        if (_entitySourceText is not null) _entitySourceText.Text = summary.SourcePath ?? "无";
        if (_entityPositionText is not null) _entityPositionText.Text = summary.PositionText;
        if (_entityVisualKindText is not null) _entityVisualKindText.Text = summary.VisualKindText;
        ShowSinglePanel(_entitySummaryContent);
    }
}

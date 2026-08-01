using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _showGrid = true;
    bool _showOrigin = true;
    bool _showWorldAxes;
    bool _showEditorBackground = true;

    public bool ShowGrid => _showGrid;
    public bool ShowOrigin => _showOrigin;
    public bool ShowWorldAxes => _showWorldAxes;
    public bool ShowEditorBackground => _showEditorBackground;
    public string GridDisplayText => AssistText("构造网格", _showGrid);
    public string OriginDisplayText => AssistText("世界原点", _showOrigin);
    public string WorldAxesDisplayText => AssistText("世界坐标轴", _showWorldAxes);
    public string EditorBackgroundDisplayText => AssistText("编辑器背景", _showEditorBackground);

    EditorViewportAssistState ViewportAssistState => new(
        _showGrid, _showOrigin, _showWorldAxes, _showEditorBackground);

    bool TryToggleViewportAssist(string name)
    {
        return name switch
        {
            "显示构造网格" => ToggleAssist(ref _showGrid, nameof(ShowGrid)),
            "显示世界原点" => ToggleAssist(ref _showOrigin, nameof(ShowOrigin)),
            "显示世界坐标轴" => ToggleAssist(ref _showWorldAxes, nameof(ShowWorldAxes)),
            "显示编辑器背景" => ToggleAssist(ref _showEditorBackground, nameof(ShowEditorBackground)),
            _ => false
        };
    }

    bool ToggleAssist(ref bool field, string propertyName)
    {
        field = !field;
        OnPropertyChanged(propertyName);
        RaiseAssistTextChanged();
        PublishSceneRenderSnapshot();
        FooterMessage = "显示辅助已更新。";
        return true;
    }

    static string AssistText(string name, bool enabled) => $"{(enabled ? "✓" : " ")}  {name}";

    void RaiseAssistTextChanged()
    {
        OnPropertyChanged(nameof(GridDisplayText));
        OnPropertyChanged(nameof(OriginDisplayText));
        OnPropertyChanged(nameof(WorldAxesDisplayText));
        OnPropertyChanged(nameof(EditorBackgroundDisplayText));
    }
}

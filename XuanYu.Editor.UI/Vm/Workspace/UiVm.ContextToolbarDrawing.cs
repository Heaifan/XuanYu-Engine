using System.Windows.Input;
using XuanYu.Editor.Workspace;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    string? _contextDrawingKind;
    public string? LastDrawTool { get; private set; }
    public string DrawButtonLabel => LastDrawTool is null ? "✎ 绘制" : $"✎ {LastDrawTool switch { "区域面" => "区域", _ => LastDrawTool }}";
    public bool IsDrawingTransactionActive => _contextDrawingKind is not null;
    public string DrawingTransactionLabel => _contextDrawingKind == "道路"
        ? $"道路绘制中 · {RoadDrawingDraftPointCount}"
        : _contextDrawingKind == "区域面" ? $"区域绘制中 · {RegionDrawingDraftVertexCount}" : "";
    public ICommand BeginLastDrawToolCommand => _beginLastDrawToolCommand ??=
        new RelayCommand(_ => _ = BeginLastDrawToolAsync());
    public ICommand SelectDrawToolCommand => _selectDrawToolCommand ??= new RelayCommand(
        value => _ = BeginContextDrawingAsync(value?.ToString()));
    ICommand? _beginLastDrawToolCommand, _selectDrawToolCommand;

    public async Task<bool> BeginLastDrawToolAsync() => await BeginContextDrawingAsync(LastDrawTool);

    public async Task<bool> BeginContextDrawingAsync(string? tool)
    {
        if (IsDrawingTransactionActive || tool is null) return false;
        if (!IsRegionWorkspace) SwitchWorkspace(EditorWorkspaceId.RegionEditor);
        if (!IsEditMode) ToggleEditorMode();
        if (tool == "道路")
        {
            SelectRegionAuthoringMode("道路");
            var started = await BeginRoadDrawingAsync();
            if (started) { SetLastDrawTool("道路"); BeginDrawingTransaction("道路"); }
            return started;
        }
        if (tool == "区域面")
        {
            SelectRegionAuthoringMode("区域面");
            var started = await BeginRegionDrawingAsync();
            if (started) { SetLastDrawTool("区域面"); BeginDrawingTransaction("区域面"); }
            return started;
        }
        if (tool == "地图标记")
        {
            SelectRegionAuthoringMode("地图标记");
            return await BeginMarkerPlacementAsync();
        }
        return false;
    }

    void SetLastDrawTool(string value)
    {
        if (LastDrawTool == value) return;
        LastDrawTool = value;
        OnPropertyChanged(nameof(LastDrawTool));
        OnPropertyChanged(nameof(DrawButtonLabel));
    }

    public bool CanUndoDrawingVertex => _contextDrawingKind == "道路" ? CanUndoRoadDrawingVertex : CanUndoRegionDrawingVertex;
    public bool CanCompleteDrawing => _contextDrawingKind == "道路" ? CanCompleteRoadDrawing : CanCompleteRegionDrawing;
    public bool CanCancelDrawing => _contextDrawingKind == "道路" ? CanCancelRoadDrawing : CanCancelRegionDrawing;

    void BeginDrawingTransaction(string kind)
    {
        _contextDrawingKind = kind;
        RaiseContextToolbarDrawingBindings();
    }

    void EndDrawingTransaction()
    {
        if (_contextDrawingKind is null) return;
        _contextDrawingKind = null;
        RaiseContextToolbarDrawingBindings();
    }

    void RaiseContextToolbarDrawingBindings()
    {
        OnPropertyChanged(nameof(IsDrawingTransactionActive));
        OnPropertyChanged(nameof(DrawingTransactionLabel));
        OnPropertyChanged(nameof(CanUndoDrawingVertex));
        OnPropertyChanged(nameof(CanCompleteDrawing));
        OnPropertyChanged(nameof(CanCancelDrawing));
    }
}

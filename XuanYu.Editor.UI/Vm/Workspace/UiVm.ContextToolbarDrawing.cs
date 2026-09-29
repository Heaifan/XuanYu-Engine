using System.Windows.Input;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.Workspace;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly AuthoringInputSession _authoringSession = new();
    public string? LastDrawTool { get; private set; }
    public string DrawButtonLabel => LastDrawTool is null ? "绘制" : LastDrawTool switch { "地图标记" => "点标记", "区域面" => "区域", _ => LastDrawTool };
    public bool IsDrawingTransactionActive => _authoringSession.IsActive;
    public bool CanOpenContextSelector => !IsDrawingTransactionActive;
    public string DrawingTransactionLabel => _authoringSession.Kind == AuthoringInputKind.Road
        ? $"道路绘制中 · {RoadDrawingDraftPointCount}"
        : _authoringSession.Kind == AuthoringInputKind.Region
            ? $"区域绘制中 · {RegionDrawingDraftVertexCount}" : "";
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
        EnterRegionContext();
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
            var started = await BeginMarkerPlacementAsync();
            if (started) SetLastDrawTool("地图标记");
            return started;
        }
        return false;
    }

    void SetLastDrawTool(string value)
    {
        if (LastDrawTool == value) return;
        LastDrawTool = value;
        OnPropertyChanged(nameof(LastDrawTool));
        OnPropertyChanged(nameof(DrawButtonLabel));
        OnPropertyChanged(nameof(ContextToolbarButtonLabel));
    }

    public bool CanUndoDrawingVertex => _authoringSession.Kind == AuthoringInputKind.Road
        ? CanUndoRoadDrawingVertex : CanUndoRegionDrawingVertex;
    public bool CanCompleteDrawing => _authoringSession.Kind == AuthoringInputKind.Road
        ? CanCompleteRoadDrawing : CanCompleteRegionDrawing;
    public bool CanCancelDrawing => _authoringSession.IsActive;

    void BeginDrawingTransaction(string kind)
    {
        var inputKind = kind == "道路" ? AuthoringInputKind.Road : AuthoringInputKind.Region;
        if (!_authoringSession.Begin(inputKind)) return;
        RaiseContextToolbarDrawingBindings();
    }

    void EndDrawingTransaction()
    {
        if (!_authoringSession.End()) return;
        RaiseContextToolbarDrawingBindings();
    }

    void RaiseContextToolbarDrawingBindings()
    {
        OnPropertyChanged(nameof(IsDrawingTransactionActive));
        OnPropertyChanged(nameof(CanOpenContextSelector));
        OnPropertyChanged(nameof(DrawingTransactionLabel));
        OnPropertyChanged(nameof(CanUndoDrawingVertex));
        OnPropertyChanged(nameof(CanCompleteDrawing));
        OnPropertyChanged(nameof(CanCancelDrawing));
    }
}

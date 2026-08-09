using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Threading;
using XuanYu.Core.History;
using XuanYu.Editor.Assets;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.SceneDocument;
using XuanYu.Render.Abstractions;
using XuanYu.World;
using XuanYu.World.Scene;
namespace XuanYu.Editor.UI;
public sealed partial class UiVm : INotifyPropertyChanged, XuanYu.Core.Scene.ISceneRenderSnapshotSource,
    XuanYu.Render.Abstractions.IRenderProjectionSource
{
    readonly EditorStateOwner _editorState;
    readonly EditorHistoryOwner _historyOwner = new();
    readonly IEditorDialogService _dialogService = new NullEditorDialogService();
    readonly Dictionary<string, EditorTreeNode> _hierarchyNodeCache = new();
    readonly HashSet<string> _collapsedProjectKeys = new(StringComparer.Ordinal);
    readonly HashSet<string> _collapsedHierarchyKeys = new(StringComparer.Ordinal);
    int _leftTabIndex; EditorTreeNode? _selectedProjectItem, _selectedHierarchyItem;
    string _selectedNodeKey = EditorSelectionSnapshot.Initial.SelectionKey;
    string _footerMessage = "已就绪。当前为空白未命名场景。", _footerState = "状态：就绪";
    bool _isLogOpen;
    public UiVm() : this(null) { }
    public UiVm(
        INativeHostSurfaceBridgeFactory? surfaceBridgeFactory,
        Func<bool>? isWriteThread = null,
        bool seedInitialScene = true,
        IEditorDialogService? dialogService = null)
    {
        _editorState = new EditorStateOwner(isWriteThread ?? (() => Dispatcher.UIThread.CheckAccess()));
        _sceneState = new SceneStateOwner(_partitionStrategy, seedInitialScene);
        _saveTransaction = new SceneDocumentSaveTransaction(_sceneStorage);
        _loadTransaction = new SceneDocumentLoadTransaction(_sceneStorage, new GlbImportService(), _partitionStrategy);
        _sceneState.RenderSnapshotChanged += _ => RefreshWorldProjectionBindings();
        if (seedInitialScene) _sceneState.EnsureEntityCount(10);
        SurfaceBridgeFactory = surfaceBridgeFactory;
        if (dialogService is not null) _dialogService = dialogService;
        RunCommand = new RelayCommand(name => Run(name?.ToString() ?? string.Empty));
        SelectToolCommand = new RelayCommand(name => SelectTool(name?.ToString() ?? string.Empty));
        ToggleSnapCommand = new RelayCommand(_ => ToggleSnap());
        InteractionCommand = new RelayCommand(name => RunInteraction(name?.ToString() ?? string.Empty));
        ToggleLogCommand = new RelayCommand(_ => IsLogOpen = !IsLogOpen);
        SelectLogFilterCommand = new RelayCommand(name => SetLogFilter(name?.ToString() ?? "全部"));
        MapSession = new MapEditSession(isWriteThread: isWriteThread ?? (() => Dispatcher.UIThread.CheckAccess()));
        // D5 二次纠偏（用户方案）：默认地图建立内存基线——初始未修改不误判为有未保存修改
        MapSession.MarkBaseline();
        AttachMapSession(MapSession); InitLogs(); LogBuildProvenance();
    }
    public event PropertyChangedEventHandler? PropertyChanged;
    public INativeHostSurfaceBridgeFactory? SurfaceBridgeFactory { get; }
    public ICommand RunCommand { get; }
    public ICommand SelectToolCommand { get; }
    public ICommand ToggleSnapCommand { get; }
    public ICommand InteractionCommand { get; }
    public ICommand ToggleLogCommand { get; }
    public ICommand SelectLogFilterCommand { get; } public MapEditSession MapSession { get; }
    public IReadOnlyList<EditorTreeNode> ProjectItems => TreeGuideBuilder.Visible(UiText.ProjectTreeItems, _collapsedProjectKeys);
    public IReadOnlyList<EditorTreeNode> HierarchyItems => BuildHierarchyItems();
    public IReadOnlyList<InspectorFieldRow> InspectorFields => BuildInspectorFields();
    public IReadOnlyList<string> EmptyHints => UiText.EmptyHints; public IReadOnlyList<string> DebugItems => UiText.DebugItems;
    public IReadOnlyList<string> ToolItems => UiText.ToolItems; public IReadOnlyList<InspectorFieldRow> DebugContextItems => DebugText.ContextItems; public IReadOnlyList<InspectorFieldRow> DebugObjectItems => BuildDebugObjectItems();
    public IReadOnlyList<InspectorFieldRow> DebugToolItems => DebugText.ToolItems; public IReadOnlyList<InspectorFieldRow> DebugInputItems => BuildDebugInputItems();
    public string ActiveTool => _editorState.ToolSnapshot.ActiveToolText;
    public bool IsSelectTool => IsTool(EditorToolId.Select);
    public bool IsMoveTool => IsTool(EditorToolId.Move);
    public bool IsRotateTool => IsTool(EditorToolId.Rotate);
    public bool IsScaleTool => IsTool(EditorToolId.Scale);
    public bool IsBoxSelectTool => IsTool(EditorToolId.BoxSelect);
    public bool IsRegionDrawingTool => IsTool(EditorToolId.RegionDrawing);
    public bool IsSnapEnabled => _editorState.ToolSnapshot.IsSnapEnabled;
    public string SnapMode => _editorState.ToolSnapshot.SnapText; // F2：模式页已删，仅保留状态读取
    public string SelectionTitle => _editorState.Snapshot.SelectionTitle;
    public string SelectionKey => _editorState.Snapshot.SelectionKey;
    public string SelectionSubtitle => _editorState.Snapshot.SelectionSubtitle;
    public string SelectionPath => _editorState.Snapshot.SelectionPath;
    public string SelectedNodeKey => _selectedNodeKey;
    public string FooterMessage { get => _footerMessage; private set => Set(ref _footerMessage, value); }
    public string FooterMode => $"工具：{ActiveTool}";
    public string FooterState { get => _footerState; private set => SetFooterState(value); }
    public bool HasSelection => _editorState.Snapshot.HasSelection;
    public bool IsLogOpen { get => _isLogOpen; set => Set(ref _isLogOpen, value); }
    public bool IsEmptySelection => !HasSelection;
    public int LeftTabIndex { get => _leftTabIndex; set => Set(ref _leftTabIndex, value); }
    public EditorTreeNode? SelectedProjectItem { get => _selectedProjectItem; set => SetProjectSelection(value); }
    public EditorTreeNode? SelectedHierarchyItem { get => _selectedHierarchyItem; set => SetHierarchySelection(value); }
    void Run(string name)
    {
        if (TryRequestFileCommand(name)) return;
        if (name == "添加立方体") { AddCubeEntity(); return; }
        if (name == "删除") { DeleteSelectedEntity(); return; }
        if (name == "撤销") { TryUndoFromCommand(); return; }
        if (name == "重做") { TryRedoFromCommand(); return; }
        ApplyRunCommand(name);
    }
    bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null) { if (EqualityComparer<T>.Default.Equals(field, value)) return false; field = value; OnPropertyChanged(name); return true; }
    void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

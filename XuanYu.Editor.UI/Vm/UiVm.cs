using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Threading;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm : INotifyPropertyChanged, XuanYu.Core.Scene.ISceneRenderSnapshotSource
{
    readonly EditorStateOwner _editorState;
    int _leftTabIndex; EditorTreeNode? _selectedProjectItem, _selectedHierarchyItem;
    string _footerMessage = "已就绪。SampleProject 已选中。", _footerState = "状态：就绪";
    bool _isLogOpen;

    public UiVm() : this(null) { }

    public UiVm(INativeHostSurfaceBridgeFactory? surfaceBridgeFactory)
    {
        _editorState = new EditorStateOwner(() => Dispatcher.UIThread.CheckAccess());
        _sceneState.RenderSnapshotChanged += _ => PublishSceneRenderSnapshot();
        SurfaceBridgeFactory = surfaceBridgeFactory;
        RunCommand = new RelayCommand(name => Run(name?.ToString() ?? string.Empty));
        SelectToolCommand = new RelayCommand(name => SelectTool(name?.ToString() ?? string.Empty));
        InteractionCommand = new RelayCommand(name => RunInteraction(name?.ToString() ?? string.Empty));
        ToggleLogCommand = new RelayCommand(_ => IsLogOpen = !IsLogOpen);
        SelectLogFilterCommand = new RelayCommand(name => SetLogFilter(name?.ToString() ?? "全部"));
        InitLogs();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public INativeHostSurfaceBridgeFactory? SurfaceBridgeFactory { get; }
    public ICommand RunCommand { get; }
    public ICommand SelectToolCommand { get; }
    public ICommand InteractionCommand { get; }
    public ICommand ToggleLogCommand { get; }
    public ICommand SelectLogFilterCommand { get; }
    public IReadOnlyList<EditorTreeNode> ProjectItems => UiText.ProjectTreeItems; public IReadOnlyList<EditorTreeNode> HierarchyItems => UiText.HierarchyTreeItems; public IReadOnlyList<string> InspectorFields => UiText.ProjectInspectorFields;
    public IReadOnlyList<string> EmptyHints => UiText.EmptyHints; public IReadOnlyList<string> DebugItems => UiText.DebugItems; public IReadOnlyList<string> PropertyItems => UiText.PropertyItems;
    public IReadOnlyList<string> ToolItems => UiText.ToolItems;
    public IReadOnlyList<string> DebugContextItems => DebugText.ContextItems; public IReadOnlyList<string> DebugObjectItems => BuildDebugObjectItems();
    public IReadOnlyList<string> DebugToolItems => DebugText.ToolItems; public IReadOnlyList<string> DebugInputItems => BuildDebugInputItems();
    public string ActiveTool => _editorState.ToolSnapshot.ActiveToolText;
    public bool IsSelectTool => IsTool(EditorToolId.Select);
    public bool IsMoveTool => IsTool(EditorToolId.Move);
    public bool IsRotateTool => IsTool(EditorToolId.Rotate);
    public bool IsScaleTool => IsTool(EditorToolId.Scale);
    public bool IsBoxSelectTool => IsTool(EditorToolId.BoxSelect);
    public bool IsFocusTool => IsTool(EditorToolId.Focus);
    public bool IsPanTool => IsTool(EditorToolId.Pan);
    public bool IsOrbitTool => IsTool(EditorToolId.Orbit);
    public bool IsSnapTool => IsTool(EditorToolId.Snap);
    public string SelectionTitle => _editorState.Snapshot.SelectionTitle;
    public string SelectionKey => _editorState.Snapshot.SelectionKey;
    public string SelectionSubtitle => _editorState.Snapshot.SelectionSubtitle;
    public string SelectionPath => _editorState.Snapshot.SelectionPath;
    public string FooterMessage { get => _footerMessage; private set => Set(ref _footerMessage, value); }
    public string FooterMode => $"工具：{ActiveTool}";
    public string FooterState { get => _footerState; private set => Set(ref _footerState, value); }
    public bool HasSelection => _editorState.Snapshot.HasSelection;
    public bool IsLogOpen { get => _isLogOpen; set => Set(ref _isLogOpen, value); }
    public bool IsEmptySelection => !HasSelection;

    public int LeftTabIndex { get => _leftTabIndex; set => Set(ref _leftTabIndex, value); }

    public EditorTreeNode? SelectedProjectItem { get => _selectedProjectItem; set => SetProjectSelection(value); }

    public EditorTreeNode? SelectedHierarchyItem { get => _selectedHierarchyItem; set => SetHierarchySelection(value); }

    void Run(string name) => ApplyRunCommand(name);

    bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null) { if (EqualityComparer<T>.Default.Equals(field, value)) return false; field = value; OnPropertyChanged(name); return true; }

    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

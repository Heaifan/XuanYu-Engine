using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm : INotifyPropertyChanged
{
    string _activeTool = "选择"; int _leftTabIndex; string? _selectedProjectItem, _selectedHierarchyItem;
    bool _isSelectTool = true, _isMoveTool, _isRotateTool, _isScaleTool, _isBoxSelectTool, _isFocusTool, _isPanTool, _isOrbitTool, _isSnapTool;
    string _selectionTitle = "SampleProject", _selectionSubtitle = "项目";
    string _footerMessage = "已就绪。SampleProject 已选中。", _footerMode = "工具：选择", _footerState = "状态：就绪";
    bool _hasSelection = true, _isLogOpen;

    public UiVm()
    {
        RunCommand = new RelayCommand(name => Run(name?.ToString() ?? string.Empty));
        SelectToolCommand = new RelayCommand(name => SelectTool(name?.ToString() ?? string.Empty));
        ToggleLogCommand = new RelayCommand(_ => IsLogOpen = !IsLogOpen);
        SelectLogFilterCommand = new RelayCommand(name => SetLogFilter(name?.ToString() ?? "全部"));
        InitLogs();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public ICommand RunCommand { get; }
    public ICommand SelectToolCommand { get; }
    public ICommand ToggleLogCommand { get; }
    public ICommand SelectLogFilterCommand { get; }
    public IReadOnlyList<string> ProjectItems => UiText.ProjectItems; public IReadOnlyList<string> HierarchyItems => UiText.HierarchyItems; public IReadOnlyList<string> InspectorFields => UiText.ProjectInspectorFields;
    public IReadOnlyList<string> EmptyHints => UiText.EmptyHints; public IReadOnlyList<string> DebugItems => UiText.DebugItems; public IReadOnlyList<string> PropertyItems => UiText.PropertyItems;
    public IReadOnlyList<string> ToolItems => UiText.ToolItems;
    public IReadOnlyList<string> DebugContextItems => DebugText.ContextItems; public IReadOnlyList<string> DebugObjectItems => DebugText.ObjectItems;
    public IReadOnlyList<string> DebugToolItems => DebugText.ToolItems; public IReadOnlyList<string> DebugInputItems => DebugText.InputItems;
    public string ActiveTool => _activeTool;
    public bool IsSelectTool { get => _isSelectTool; private set => Set(ref _isSelectTool, value); }
    public bool IsMoveTool { get => _isMoveTool; private set => Set(ref _isMoveTool, value); }
    public bool IsRotateTool { get => _isRotateTool; private set => Set(ref _isRotateTool, value); }
    public bool IsScaleTool { get => _isScaleTool; private set => Set(ref _isScaleTool, value); }
    public bool IsBoxSelectTool { get => _isBoxSelectTool; private set => Set(ref _isBoxSelectTool, value); }
    public bool IsFocusTool { get => _isFocusTool; private set => Set(ref _isFocusTool, value); }
    public bool IsPanTool { get => _isPanTool; private set => Set(ref _isPanTool, value); }
    public bool IsOrbitTool { get => _isOrbitTool; private set => Set(ref _isOrbitTool, value); }
    public bool IsSnapTool { get => _isSnapTool; private set => Set(ref _isSnapTool, value); }
    public string SelectionTitle { get => _selectionTitle; private set => Set(ref _selectionTitle, value); }
    public string SelectionSubtitle { get => _selectionSubtitle; private set => Set(ref _selectionSubtitle, value); }
    public string FooterMessage { get => _footerMessage; private set => Set(ref _footerMessage, value); }
    public string FooterMode { get => _footerMode; private set => Set(ref _footerMode, value); }
    public string FooterState { get => _footerState; private set => Set(ref _footerState, value); }
    public bool HasSelection { get => _hasSelection; private set => Set(ref _hasSelection, value); }
    public bool IsLogOpen { get => _isLogOpen; set => Set(ref _isLogOpen, value); }
    public bool IsEmptySelection => !HasSelection;

    public int LeftTabIndex { get => _leftTabIndex; set => Set(ref _leftTabIndex, value); }

    public string? SelectedProjectItem { get => _selectedProjectItem; set { if (Set(ref _selectedProjectItem, value) && value is not null) ApplySelection("项目", value); } }

    public string? SelectedHierarchyItem { get => _selectedHierarchyItem; set { if (Set(ref _selectedHierarchyItem, value) && value is not null) ApplySelection("世界层级", value); } }

    void Run(string name) { FooterMessage = UiText.CommandMessages.GetValueOrDefault(name, $"已执行：{name}"); FooterState = name is "运行" ? "状态：运行中" : "状态：就绪"; LogCommand(name); OnPropertyChanged(nameof(LogSummary)); }

    void SelectTool(string name) { _activeTool = string.IsNullOrWhiteSpace(name) ? "选择" : name; SetToolFlags(_activeTool); FooterMode = $"工具：{_activeTool}"; FooterMessage = $"当前工具：{_activeTool}。视口等待输入。"; LogTool(_activeTool); OnPropertyChanged(nameof(ActiveTool)); OnPropertyChanged(nameof(LogSummary)); }

    void ApplySelection(string source, string item) { HasSelection = true; SelectionTitle = item.TrimStart(' ', '├', '└', '─'); SelectionSubtitle = source; FooterMessage = $"{source}已选择：{SelectionTitle}"; FooterState = "状态：聚焦"; OnPropertyChanged(nameof(LogSummary)); }

    void SetToolFlags(string tool) { IsSelectTool = tool == "选择"; IsMoveTool = tool == "移动"; IsRotateTool = tool == "旋转"; IsScaleTool = tool == "缩放"; IsBoxSelectTool = tool == "框选"; IsFocusTool = tool == "聚焦"; IsPanTool = tool == "平移"; IsOrbitTool = tool == "环绕"; IsSnapTool = tool == "吸附"; }

    bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null) { if (EqualityComparer<T>.Default.Equals(field, value)) return false; field = value; OnPropertyChanged(name); if (name == nameof(HasSelection)) OnPropertyChanged(nameof(IsEmptySelection)); return true; }

    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

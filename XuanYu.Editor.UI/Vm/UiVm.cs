using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace XuanYu.Editor.UI;

public sealed class UiVm : INotifyPropertyChanged
{
    readonly Dictionary<string, string> _modeDescriptions =
        new(StringComparer.Ordinal)
        {
            ["Project"] = "项目视图：管理资源、入口和构建节点。",
            ["World"] = "世界视图：浏览场景树并选择实体。",
            ["Viewport"] = "视口视图：保持主编辑画布居中显示。",
            ["Inspector"] = "检查器视图：编辑当前对象的属性与变换。",
            ["Logs"] = "日志视图：查看编辑器状态和诊断输出。"
        };

    readonly Dictionary<string, string> _modeTitles =
        new(StringComparer.Ordinal)
        {
            ["Project"] = "项目",
            ["World"] = "世界",
            ["Viewport"] = "视口",
            ["Inspector"] = "检查器",
            ["Logs"] = "日志"
        };

    string _activeMode = "Viewport";
    int _leftTabIndex;
    string? _selectedProjectItem;
    string? _selectedHierarchyItem;
    string _selectionTitle = "未选择";
    string _selectionSubtitle = "选择项目节点或世界实体后，会在检查器中显示详情。";
    string _footerMessage = "布局迭代就绪";
    string _footerMode = "模式：视口";
    string _footerState = "状态：干净";

    public UiVm()
    {
        SelectModeCommand = new RelayCommand(mode => SelectMode(mode?.ToString() ?? string.Empty));
        ProjectItems =
        [
            "示例项目 / SampleProject",
            "内容 / 世界",
            "资源 / 图标",
            "运行 / 保存 / 构建",
        ];
        HierarchyItems =
        [
            "世界根节点",
            "主相机",
            "地面",
            "选中实体",
        ];
        InspectorFields =
        [
            "位置：0, 0, 0",
            "旋转：0°",
            "缩放：1, 1, 1",
            "脏标记：否",
        ];
        DebugItems =
        [
            "Shell 已挂载",
            "Avalonia UI 正在运行",
            "Vulkan 视口暂未接入",
            "下一步：接入编辑器状态和视口宿主",
        ];
        PropertyItems =
        [
            "布局：Grid + 面板",
            "视口：占位预览",
            "检查器：只读快照",
            "持久化：待接入",
        ];

        ApplyMode(_activeMode);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand SelectModeCommand { get; }

    public IReadOnlyList<string> ProjectItems { get; }

    public IReadOnlyList<string> HierarchyItems { get; }

    public IReadOnlyList<string> InspectorFields { get; }

    public IReadOnlyList<string> DebugItems { get; }

    public IReadOnlyList<string> PropertyItems { get; }

    public string? SelectedProjectItem
    {
        get => _selectedProjectItem;
        set
        {
            if (SetNullableField(ref _selectedProjectItem, value))
            {
                if (value is not null)
                {
                    ApplySelection("Project", value);
                }
            }
        }
    }

    public string? SelectedHierarchyItem
    {
        get => _selectedHierarchyItem;
        set
        {
            if (SetNullableField(ref _selectedHierarchyItem, value))
            {
                if (value is not null)
                {
                    ApplySelection("World", value);
                }
            }
        }
    }

    public int LeftTabIndex
    {
        get => _leftTabIndex;
        set
        {
            if (_leftTabIndex == value)
            {
                return;
            }

            _leftTabIndex = value;
            OnPropertyChanged();
        }
    }

    public string SelectionTitle
    {
        get => _selectionTitle;
        private set => SetField(ref _selectionTitle, value);
    }

    public string SelectionSubtitle
    {
        get => _selectionSubtitle;
        private set => SetField(ref _selectionSubtitle, value);
    }

    public string FooterMessage
    {
        get => _footerMessage;
        private set => SetField(ref _footerMessage, value);
    }

    public string FooterMode
    {
        get => _footerMode;
        private set => SetField(ref _footerMode, value);
    }

    public string FooterState
    {
        get => _footerState;
        private set => SetField(ref _footerState, value);
    }

    void SelectMode(string mode)
    {
        if (!_modeDescriptions.ContainsKey(mode))
        {
            return;
        }

        ApplyMode(mode);
    }

    void ApplyMode(string mode)
    {
        _activeMode = mode;
        LeftTabIndex = mode switch
        {
            "Project" => 0,
            "World" => 1,
            _ => LeftTabIndex
        };
        if (mode == "Project" && SelectedProjectItem is null)
        {
            SelectedProjectItem = ProjectItems.FirstOrDefault();
        }
        else if (mode == "World" && SelectedHierarchyItem is null)
        {
            SelectedHierarchyItem = HierarchyItems.FirstOrDefault();
        }

        FooterMode = $"模式：{_modeTitles[mode]}";
        FooterMessage = _modeDescriptions[mode];
        FooterState = mode == "Viewport" ? "状态：就绪" : "状态：干净";
        SelectionTitle = mode switch
        {
            "Project" => "项目根节点",
            "World" => "场景根节点",
            "Inspector" => "当前选择",
            "Logs" => "诊断输出",
            _ => "未选择"
        };
        SelectionSubtitle = mode switch
        {
            "Project" => "浏览资源并准备内容入口。",
            "World" => "选择实体后可在检查器中查看和编辑。",
            "Inspector" => "编辑当前对象的变换和基础属性。",
            "Logs" => "查看编辑器动作、诊断和运行状态。",
            _ => "选择项目节点或世界实体后，会在检查器中显示详情。"
        };
        OnPropertyChanged(nameof(SelectionTitle));
        OnPropertyChanged(nameof(SelectionSubtitle));
        OnPropertyChanged(nameof(FooterMessage));
        OnPropertyChanged(nameof(FooterMode));
        OnPropertyChanged(nameof(FooterState));
        OnPropertyChanged(nameof(ActiveModeTitle));
    }

    void ApplySelection(string source, string item)
    {
        ApplyMode(source);
        SelectionTitle = item;
        SelectionSubtitle = source switch
        {
            "Project" => $"项目项：{item}。可作为入口或内容节点使用。",
            "World" => $"世界项：{item}。已作为当前编辑目标。",
            _ => $"已选择：{item}。"
        };
        FooterMessage = $"{_modeTitles[source]}已选择：{item}";
        FooterState = "状态：聚焦";
        OnPropertyChanged(nameof(SelectionTitle));
        OnPropertyChanged(nameof(SelectionSubtitle));
        OnPropertyChanged(nameof(FooterMessage));
        OnPropertyChanged(nameof(FooterState));
        OnPropertyChanged(nameof(ActiveModeTitle));
    }

    public string ActiveMode => _activeMode;

    public string ActiveModeTitle => _modeTitles[_activeMode];

    bool SetField(ref string field, string value, [CallerMemberName] string? name = null)
    {
        if (field == value)
        {
            return false;
        }

        field = value;
        OnPropertyChanged(name);
        return true;
    }

    bool SetNullableField(ref string? field, string? value, [CallerMemberName] string? name = null)
    {
        if (field == value)
        {
            return false;
        }

        field = value;
        OnPropertyChanged(name);
        return true;
    }

    void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

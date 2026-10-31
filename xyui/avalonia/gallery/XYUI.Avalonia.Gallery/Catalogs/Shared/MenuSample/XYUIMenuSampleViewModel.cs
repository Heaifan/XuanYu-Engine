using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace XYUI.Avalonia.Gallery;

public sealed class XYUIMenuSampleViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    string _lastAction = "就绪 · 点击菜单项体验 ICommand 与状态绑定";
    string _selectedWorkspace = "地图编辑";
    bool _isGrid = true, _isOrigin, _isAxes = true;

    public ICommand RunCommand { get; }
    public ICommand DisabledCommand { get; }

    public XYUIMenuSampleViewModel()
    {
        RunCommand = new SampleCmd(p => LastAction = $"已执行命令 · {p}");
        DisabledCommand = new SampleCmd(_ => { }, _ => false);
    }

    public string LastAction { get => _lastAction; set { _lastAction = value; Notify(); } }
    public bool IsGridVisible { get => _isGrid; set { _isGrid = value; Notify(); Notify(nameof(StatusSummary)); } }
    public bool IsOriginVisible { get => _isOrigin; set { _isOrigin = value; Notify(); Notify(nameof(StatusSummary)); } }
    public bool IsAxesVisible { get => _isAxes; set { _isAxes = value; Notify(); Notify(nameof(StatusSummary)); } }

    public bool IsMapEditor
    {
        get => _selectedWorkspace == "地图编辑";
        set { if (value && _selectedWorkspace != "地图编辑") { _selectedWorkspace = "地图编辑"; NotifyWorkspace(); } }
    }

    public bool IsRegionEditor
    {
        get => _selectedWorkspace == "区域编辑";
        set { if (value && _selectedWorkspace != "区域编辑") { _selectedWorkspace = "区域编辑"; NotifyWorkspace(); } }
    }

    public string StatusSummary =>
        $"工作区: {_selectedWorkspace} | 网格: {(_isGrid ? "开" : "关")} | 原点: {(_isOrigin ? "开" : "关")} | 坐标轴: {(_isAxes ? "开" : "关")}";

    void NotifyWorkspace()
    {
        Notify(nameof(IsMapEditor));
        Notify(nameof(IsRegionEditor));
        Notify(nameof(StatusSummary));
        LastAction = $"工作区切换 · {_selectedWorkspace}";
    }

    void Notify([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new(name));

    sealed class SampleCmd(Action<object?> exec, Func<object?, bool>? canExec = null) : ICommand
    {
        public bool CanExecute(object? parameter) => canExec?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => exec(parameter);
        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}

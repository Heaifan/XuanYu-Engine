using System.ComponentModel;
using System.Runtime.CompilerServices;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D4：图层行显示模型（面板行绑定；写操作转发会话命令，不直接持有领域状态）。
// 名称/类型/系统标识为行创建时快照；显隐/锁定写回会话（成功由 ContentChanged 重建列表）。
public sealed class MapLayerRowViewModel : INotifyPropertyChanged
{
    readonly UiVm _owner;
    bool _isVisible;
    bool _isLocked;
    bool _isActive;

    public MapLayerRowViewModel(UiVm owner, MapLayer layer)
    {
        _owner = owner;
        LayerId = layer.LayerId;
        Name = layer.DisplayName;
        KindTagText = layer.Kind == MapLayerKind.Region ? "区域" : "系统";
        IsSystem = MapLayerRules.IsSystemLayer(layer.Kind);
        _isVisible = layer.IsVisible;
        _isLocked = layer.IsLocked;
    }

    public MapLayerId LayerId { get; }

    public string Name { get; }

    public string KindTagText { get; }

    public bool IsSystem { get; }

    // F3：区域图层（可拖动、蓝青标签）；系统图层不显示拖动手柄。
    public bool IsRegion => !IsSystem;

    // F3：拖动插入线（2 DIP 低饱和蓝，显示在本行上方）。
    public bool IsDropBefore { get; internal set; }

    public bool IsActive { get => _isActive; set => Set(ref _isActive, value); }

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (value == _isVisible) return;
            if (!_owner.SetLayerVisibility(LayerId, value)) return;
            _isVisible = value;
            OnPropertyChanged();
        }
    }

    public bool IsLocked
    {
        get => _isLocked;
        set
        {
            if (value == _isLocked) return;
            if (!_owner.SetLayerLock(LayerId, value)) return;
            _isLocked = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(name);
    }

    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

using System.Threading;
using FluidWarfare.Editor.Input.Actions;
using FluidWarfare.Editor.Input.Bindings;
using FluidWarfare.Editor.Input.Settings;

namespace FluidWarfare.Editor.Input.Runtime;

/// <summary>
/// 单例服务，管理运行时绑定快照的生命周期和热更新。
/// 设置变更后在此重建快照并原子替换。
/// </summary>
public sealed class EditorInputService
{
    /// <summary>全局唯一实例。</summary>
    public static EditorInputService Instance { get; } = new();

    private EditorInputService() { }

    private EditorInputBindingSnapshot _currentSnapshot = null!;
    private int _revision;
    private bool _initialized;

    /// <summary>当前运行时快照（O(1) 查找入口）。</summary>
    public EditorInputBindingSnapshot CurrentSnapshot
    {
        get
        {
            if (!_initialized) Initialize();
            return _currentSnapshot;
        }
        private set => _currentSnapshot = value;
    }

    /// <summary>快照被替换时触发（translator 订阅以更新其引用）。</summary>
    public event Action<EditorInputBindingSnapshot>? SnapshotReplaced;

    /// <summary>
    /// 从设置文件加载并构建初始快照。
    /// 可在 EditorShell 初始化时调用。重复调用安全。
    /// </summary>
    public void Initialize()
    {
        var doc = EditorSettingsReader.Read(out _);
        var snapshot = EditorInputBindingSnapshot.Build(
            doc.Input, EditorInputActionCatalog.All, Interlocked.Increment(ref _revision));
        lock (this)
        {
            _currentSnapshot = snapshot;
            _initialized = true;
        }
    }

    /// <summary>
    /// 获取当前设置绑定的只读快照（用于 Preferences 窗口读取）。
    /// </summary>
    public EditorInputBindingSet GetCurrentBindingSet()
    {
        var doc = EditorSettingsReader.Read(out _);
        return doc.Input;
    }

    /// <summary>
    /// 原子替换运行时快照。取消任何活动拖动后替换。
    /// 调用方应先在设置写入成功后再调用此方法。
    /// </summary>
    public bool TryApplyNewBindingSet(EditorInputBindingSet newSet, out string? error)
    {
        // 1. 构建新快照
        EditorInputBindingSnapshot newSnapshot;
        try
        {
            newSnapshot = EditorInputBindingSnapshot.Build(
                newSet, EditorInputActionCatalog.All, Interlocked.Increment(ref _revision));
        }
        catch (Exception ex)
        {
            error = $"构建绑定快照失败：{ex.Message}";
            return false;
        }

        // 2. 取消旧快照中的活动拖动
        _currentSnapshot.EndDrag();

        // 3. 原子替换
        EditorInputBindingSnapshot replaced;
        lock (this)
        {
            replaced = _currentSnapshot;
            _currentSnapshot = newSnapshot;
        }

        // 4. 通知所有消费者
        SnapshotReplaced?.Invoke(newSnapshot);

        error = null;
        return true;
    }
}

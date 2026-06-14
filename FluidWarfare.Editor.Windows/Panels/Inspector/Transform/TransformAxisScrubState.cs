using Avalonia.Input;

namespace FluidWarfare.Editor.Windows.Panels.Inspector.Transform;

/// <summary>
/// Transform 数值拖拽状态机。
/// 灵敏度：普通 0.02/px, Shift 0.002/px, Ctrl 0.20/px。
/// </summary>
public sealed class TransformAxisScrubState
{
    public const double NormalSensitivity = 0.02;
    public const double FineSensitivity = 0.002;
    public const double CoarseSensitivity = 0.20;

    private TransformPositionAxis _axis;
    private double _initialValue;
    private double _currentValue;
    private double _lastPixelX;
    private double _lastCommittedValue;
    private double _sensitivity;

    /// <summary>是否正在拖拽中。</summary>
    public bool IsScrubbing { get; private set; }

    /// <summary>当前轴向。</summary>
    public TransformPositionAxis Axis => _axis;

    /// <summary>初始值（用于取消恢复）。</summary>
    public double InitialValue => _initialValue;

    /// <summary>当前草稿值。</summary>
    public double CurrentValue => _currentValue;

    /// <summary>值是否发生了变化。</summary>
    public bool HasChanged => _currentValue != _initialValue;

    /// <summary>值变化时触发。</summary>
    public event Action<double>? ValueChanged;

    /// <summary>拖拽完成时触发。</summary>
    public event Action? ScrubCompleted;

    /// <summary>拖拽取消时触发。</summary>
    public event Action? ScrubCancelled;

    /// <summary>
    /// 开始拖拽。
    /// </summary>
    public void Begin(TransformPositionAxis axis, double initialValue, double pointerX, KeyModifiers modifiers)
    {
        _axis = axis;
        _initialValue = initialValue;
        _currentValue = initialValue;
        _lastPixelX = pointerX;
        _lastCommittedValue = initialValue;
        _sensitivity = CalcSensitivity(modifiers);
        IsScrubbing = true;
    }

    /// <summary>
    /// 更新拖拽值（增量算法，修饰键变化时重新锚定，避免跳值）。
    /// </summary>
    public void Update(double pointerX, KeyModifiers modifiers)
    {
        if (!IsScrubbing) return;

        var newSensitivity = CalcSensitivity(modifiers);
        var deltaPx = pointerX - _lastPixelX;
        _lastPixelX = pointerX;

        // 修饰键变化：重新锚定
        if (newSensitivity != _sensitivity)
        {
            _lastCommittedValue = _currentValue;
            _sensitivity = newSensitivity;
        }

        _currentValue = _lastCommittedValue + deltaPx * _sensitivity;
        ValueChanged?.Invoke(_currentValue);
    }

    /// <summary>
    /// 完成拖拽（确认值）。
    /// </summary>
    public void Complete()
    {
        if (!IsScrubbing) return;
        IsScrubbing = false;
        ScrubCompleted?.Invoke();
    }

    /// <summary>
    /// 取消拖拽（恢复初始值）。
    /// </summary>
    public void Cancel()
    {
        if (!IsScrubbing) return;
        IsScrubbing = false;
        _currentValue = _initialValue;
        ScrubCancelled?.Invoke();
    }

    private static double CalcSensitivity(KeyModifiers modifiers)
    {
        if ((modifiers & KeyModifiers.Shift) != 0) return FineSensitivity;
        if ((modifiers & KeyModifiers.Control) != 0) return CoarseSensitivity;
        return NormalSensitivity;
    }
}

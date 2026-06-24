using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Editor.Transform.Scrub;

/// <summary>
/// Transform 数值拖拽状态机（平台无关）。
/// 灵敏度：普通 0.02/px, Shift 0.002/px, Ctrl 0.20/px。
/// </summary>
public sealed class TransformAxisScrubState
{
    public const double NormalSensitivity = 0.02;
    public const double FineSensitivity = 0.002;
    public const double CoarseSensitivity = 0.20;

    private double _initialValue;
    private int _axis; // 0=X, 1=Y, 2=Z — generic, caller assigns meaning
    private double _currentValue;
    private double _lastPixelX;
    private double _sensitivity;

    /// <summary>是否正在拖拽中。</summary>
    public bool IsScrubbing { get; private set; }

    /// <summary>初始值（用于取消恢复）。</summary>
    public double InitialValue => _initialValue;

    /// <summary>当前草稿值。</summary>
    public double CurrentValue => _currentValue;

    /// <summary>当前轴向（调用方赋予的含义：0=X, 1=Y, 2=Z 等）。</summary>
    public int Axis => _axis;

    /// <summary>值变化时触发。</summary>
    public event Action<double>? ValueChanged;

    /// <summary>拖拽完成时触发。</summary>
    public event Action? ScrubCompleted;

    /// <summary>拖拽取消时触发。</summary>
    public event Action? ScrubCancelled;

    /// <summary>
    /// 开始拖拽。
    /// </summary>
    public void Begin(int axis, double initialValue, double pointerX, double sensitivity)
    {
        _axis = axis;
        _initialValue = initialValue;
        _currentValue = initialValue;
        _lastPixelX = pointerX;
        _sensitivity = sensitivity;
        IsScrubbing = true;
    }

    /// <summary>
    /// 更新拖拽值（增量累计，修饰键变化会切换灵敏度但不跳值）。
    /// </summary>
    public void Update(double pointerX, double sensitivity)
    {
        if (!IsScrubbing) return;

        var deltaPx = pointerX - _lastPixelX;
        _lastPixelX = pointerX;
        _sensitivity = sensitivity;

        _currentValue += deltaPx * _sensitivity;
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
}

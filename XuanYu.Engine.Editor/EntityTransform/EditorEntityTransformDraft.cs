using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Editor.EntityTransform;

/// <summary>
/// 检查器当前 Transform 输入草稿。
/// 只表示尚未提交的 UI 输入，不直接修改 World。
/// </summary>
public sealed class EditorEntityTransformDraft
{
    private string? _entityId;
    private string _xText = string.Empty;
    private string _yText = string.Empty;
    private string _zText = string.Empty;
    private bool _isDirty;

    /// <summary>当前编辑的实体 ID。</summary>
    public string? EntityId => _entityId;

    /// <summary>X 坐标输入文本。</summary>
    public string XText => _xText;

    /// <summary>Y 坐标输入文本。</summary>
    public string YText => _yText;

    /// <summary>Z 坐标输入文本。</summary>
    public string ZText => _zText;

    /// <summary>草稿是否已被修改（与加载的正式值不同）。</summary>
    public bool IsDirty => _isDirty;

    /// <summary>
    /// 加载实体的正式坐标。
    /// </summary>
    public void Load(string? entityId, Vector3d? position)
    {
        _entityId = entityId;
        _xText = position is not null ? Format(position.Value.X) : string.Empty;
        _yText = position is not null ? Format(position.Value.Y) : string.Empty;
        _zText = position is not null ? Format(position.Value.Z) : string.Empty;
        _isDirty = false;
    }

    /// <summary>
    /// 重置到给定坐标。
    /// </summary>
    public void Reset(Vector3d position)
    {
        _xText = Format(position.X);
        _yText = Format(position.Y);
        _zText = Format(position.Z);
        _isDirty = false;
    }

    /// <summary>
    /// 设置单个坐标文本。
    /// </summary>
    public void SetX(string text) { _xText = text; _isDirty = true; }
    public void SetY(string text) { _yText = text; _isDirty = true; }
    public void SetZ(string text) { _zText = text; _isDirty = true; }

    /// <summary>
    /// 清除草稿。
    /// </summary>
    public void Clear()
    {
        _entityId = null;
        _xText = string.Empty;
        _yText = string.Empty;
        _zText = string.Empty;
        _isDirty = false;
    }

    private static string Format(double value) =>
        value.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
}

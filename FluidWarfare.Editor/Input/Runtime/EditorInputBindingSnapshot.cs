using FluidWarfare.Editor.Input.Actions;
using FluidWarfare.Editor.Input.Bindings;

namespace FluidWarfare.Editor.Input.Runtime;

/// <summary>
/// 不可变的运行时绑定快照，提供 O(1) 手势→动作查找。
/// 设置变更后重建此快照并原子替换。
/// </summary>
public sealed class EditorInputBindingSnapshot
{
    private readonly Dictionary<string, EditorInputBinding> _primaryBySignature;
    private readonly Dictionary<string, EditorInputBinding> _secondaryBySignature;
    private readonly Dictionary<string, EditorInputActionDefinition> _definitions;

    /// <summary>修订号，每次重建递增。</summary>
    public int Revision { get; }

    /// <summary>当前用于拖动的动作（按下时解析一次）。</summary>
    public EditorInputBinding? ActiveDragBinding { get; private set; }

    /// <summary>活动拖动手势签名（用于持续匹配）。</summary>
    public string? ActiveDragSignature { get; private set; }

    private EditorInputBindingSnapshot(
        Dictionary<string, EditorInputBinding> primary,
        Dictionary<string, EditorInputBinding> secondary,
        Dictionary<string, EditorInputActionDefinition> defs,
        int revision)
    {
        _primaryBySignature = primary;
        _secondaryBySignature = secondary;
        _definitions = defs;
        Revision = revision;
    }

    /// <summary>
    /// 从绑定集和动作目录构建快照。
    /// </summary>
    public static EditorInputBindingSnapshot Build(
        EditorInputBindingSet set,
        IReadOnlyList<EditorInputActionDefinition> allActions,
        int revision)
    {
        var primary = new Dictionary<string, EditorInputBinding>();
        var secondary = new Dictionary<string, EditorInputBinding>();
        var defs = allActions.ToDictionary(a => a.Id, a => a);

        // 从预设获取默认绑定
        var presetBindings = GetPresetBindings(set.Preset, allActions);

        // 应用覆盖
        foreach (var binding in presetBindings)
        {
            if (binding.PrimaryGesture is not null && !string.IsNullOrEmpty(binding.PrimaryGesture.Signature))
                primary[binding.PrimaryGesture.Signature] = binding;
            if (binding.SecondaryGesture is not null && !string.IsNullOrEmpty(binding.SecondaryGesture.Signature))
                secondary[binding.SecondaryGesture.Signature] = binding;
        }

        foreach (var ov in set.Overrides)
        {
            var targetBinding = presetBindings.FirstOrDefault(b => b.ActionId == ov.ActionId);
            if (targetBinding is null) continue;

            if (ov.Slot == "primary")
            {
                if (ov.Gesture is null)
                {
                    // 清除：移除旧签名
                    if (targetBinding.PrimaryGesture is not null)
                        primary.Remove(targetBinding.PrimaryGesture.Signature);
                }
                else
                {
                    // 替换：移除旧签名，添加新签名
                    if (targetBinding.PrimaryGesture is not null)
                        primary.Remove(targetBinding.PrimaryGesture.Signature);
                    primary[ov.Gesture.Signature] = targetBinding with { PrimaryGesture = ov.Gesture };
                }
            }
            else if (ov.Slot == "secondary")
            {
                if (ov.Gesture is null)
                {
                    if (targetBinding.SecondaryGesture is not null)
                        secondary.Remove(targetBinding.SecondaryGesture.Signature);
                }
                else
                {
                    if (targetBinding.SecondaryGesture is not null)
                        secondary.Remove(targetBinding.SecondaryGesture.Signature);
                    secondary[ov.Gesture.Signature] = targetBinding with { SecondaryGesture = ov.Gesture };
                }
            }
        }

        return new EditorInputBindingSnapshot(primary, secondary, defs, revision);
    }

    /// <summary>
    /// O(1) 查找手势对应的动作。
    /// </summary>
    public EditorInputActionDefinition? Resolve(string gestureSignature)
    {
        if (_primaryBySignature.TryGetValue(gestureSignature, out var binding))
            return binding.Definition ?? _definitions.GetValueOrDefault(binding.ActionId);
        if (_secondaryBySignature.TryGetValue(gestureSignature, out binding))
            return binding.Definition ?? _definitions.GetValueOrDefault(binding.ActionId);
        return null;
    }

    /// <summary>
    /// 开始拖动：锁定动作，移动时不再重新查询。
    /// </summary>
    public void BeginDrag(string gestureSignature, int pixelX, int pixelY)
    {
        if (_primaryBySignature.TryGetValue(gestureSignature, out var binding) ||
            _secondaryBySignature.TryGetValue(gestureSignature, out binding))
        {
            ActiveDragBinding = binding;
            ActiveDragSignature = gestureSignature;
        }
    }

    /// <summary>
    /// 获取当前拖动的动作 ID（移动时调用，不重新查表）。
    /// </summary>
    public string? GetActiveDragActionId() => ActiveDragBinding?.ActionId;

    /// <summary>
    /// 获取当前拖动的完整动作定义（移动时调用，不重新查表）。
    /// 用于 OnRawPointerMoved 构造完整 EditorInputMatch。
    /// </summary>
    public EditorInputActionDefinition? GetActiveDragDefinition()
    {
        var binding = ActiveDragBinding;
        if (binding is null) return null;
        return binding.Definition ?? _definitions.GetValueOrDefault(binding.ActionId);
    }

    /// <summary>
    /// 结束拖动。
    /// </summary>
    public void EndDrag()
    {
        ActiveDragBinding = null;
        ActiveDragSignature = null;
    }

    private static List<EditorInputBinding> GetPresetBindings(
        string presetName,
        IReadOnlyList<EditorInputActionDefinition> allActions)
    {
        if (presetName == "blender")
        {
            // 使用实际的 Blender 默认绑定数据
            var defaultBindings = EditorInputActionCatalog.BlenderDefaultBindings;
            var defs = allActions.ToDictionary(a => a.Id, a => a);

            // 为每个默认绑定注入 Definition
            var result = new List<EditorInputBinding>(defaultBindings.Count);
            foreach (var b in defaultBindings)
            {
                if (defs.TryGetValue(b.ActionId, out var def))
                    result.Add(b with { Definition = def });
                else
                    result.Add(b);
            }
            return result;
        }

        return new List<EditorInputBinding>();
    }
}

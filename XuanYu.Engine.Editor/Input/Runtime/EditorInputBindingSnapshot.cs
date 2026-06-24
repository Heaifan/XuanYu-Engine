using FluidWarfare.Editor.Input.Actions;
using FluidWarfare.Editor.Input.Bindings;

namespace FluidWarfare.Editor.Input.Runtime;

/// <summary>运行时绑定快照，O(1) 手势→动作查找。设置变更后重建并原子替换。</summary>
public sealed partial class EditorInputBindingSnapshot
{
    readonly Dictionary<string, EditorInputBinding> _primaryBySignature;
    readonly Dictionary<string, EditorInputBinding> _secondaryBySignature;
    readonly Dictionary<string, EditorInputActionDefinition> _definitions;

    public int Revision { get; }
    public EditorInputBinding? ActiveDragBinding { get; private set; }
    public string? ActiveDragSignature { get; private set; }

    EditorInputBindingSnapshot(Dictionary<string, EditorInputBinding> p, Dictionary<string, EditorInputBinding> s,
        Dictionary<string, EditorInputActionDefinition> d, int rev)
    { _primaryBySignature = p; _secondaryBySignature = s; _definitions = d; Revision = rev; }

    public EditorInputActionDefinition? Resolve(string sig)
    {
        if (_primaryBySignature.TryGetValue(sig, out var b) || _secondaryBySignature.TryGetValue(sig, out b))
            return b.Definition ?? _definitions.GetValueOrDefault(b.ActionId);
        return null;
    }

    public void BeginDrag(string sig, int px, int py)
    {
        if (_primaryBySignature.TryGetValue(sig, out var b) || _secondaryBySignature.TryGetValue(sig, out b))
        { ActiveDragBinding = b; ActiveDragSignature = sig; }
    }

    public string? GetActiveDragActionId() => ActiveDragBinding?.ActionId;
    public EditorInputActionDefinition? GetActiveDragDefinition()
    { var b = ActiveDragBinding; return b?.Definition ?? (b is not null ? _definitions.GetValueOrDefault(b.ActionId) : null); }
    public void EndDrag() { ActiveDragBinding = null; ActiveDragSignature = null; }
}

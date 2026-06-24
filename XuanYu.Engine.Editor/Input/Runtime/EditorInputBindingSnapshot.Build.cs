using XuanYu.Engine.Editor.Input.Actions;
using XuanYu.Engine.Editor.Input.Bindings;

namespace XuanYu.Engine.Editor.Input.Runtime;

/// <summary>Partial：快照构建逻辑。</summary>
sealed partial class EditorInputBindingSnapshot
{
    public static EditorInputBindingSnapshot Build(EditorInputBindingSet set,
        IReadOnlyList<EditorInputActionDefinition> allActions, int revision)
    {
        var primary = new Dictionary<string, EditorInputBinding>();
        var secondary = new Dictionary<string, EditorInputBinding>();
        var defs = allActions.ToDictionary(a => a.Id, a => a);
        var presetBindings = GetPresetBindings(set.Preset, allActions);
        foreach (var b in presetBindings)
        {
            if (b.PrimaryGesture is not null && !string.IsNullOrEmpty(b.PrimaryGesture.Signature))
                primary[b.PrimaryGesture.Signature] = b;
            if (b.SecondaryGesture is not null && !string.IsNullOrEmpty(b.SecondaryGesture.Signature))
                secondary[b.SecondaryGesture.Signature] = b;
        }
        foreach (var ov in set.Overrides)
        {
            var tb = presetBindings.FirstOrDefault(b => b.ActionId == ov.ActionId);
            if (tb is null) continue;
            if (ov.Slot == "primary")
            {
                if (ov.Gesture is null) { if (tb.PrimaryGesture is not null) primary.Remove(tb.PrimaryGesture.Signature); }
                else { if (tb.PrimaryGesture is not null) primary.Remove(tb.PrimaryGesture.Signature); primary[ov.Gesture.Signature] = tb with { PrimaryGesture = ov.Gesture }; }
            }
            else if (ov.Slot == "secondary")
            {
                if (ov.Gesture is null) { if (tb.SecondaryGesture is not null) secondary.Remove(tb.SecondaryGesture.Signature); }
                else { if (tb.SecondaryGesture is not null) secondary.Remove(tb.SecondaryGesture.Signature); secondary[ov.Gesture.Signature] = tb with { SecondaryGesture = ov.Gesture }; }
            }
        }
        return new EditorInputBindingSnapshot(primary, secondary, defs, revision);
    }

    static List<EditorInputBinding> GetPresetBindings(string preset, IReadOnlyList<EditorInputActionDefinition> allActions)
    {
        if (preset != "blender") return [];
        var defaults = EditorInputActionCatalog.BlenderDefaultBindings;
        var defs = allActions.ToDictionary(a => a.Id, a => a);
        var result = new List<EditorInputBinding>(defaults.Count);
        foreach (var b in defaults)
            result.Add(defs.TryGetValue(b.ActionId, out var d) ? b with { Definition = d } : b);
        return result;
    }
}

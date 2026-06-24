using XuanYu.Engine.Editor.Input.Actions;
using XuanYu.Engine.Editor.Input.Bindings;
using XuanYu.Engine.Editor.Input.Runtime;
using XuanYu.Engine.Editor.Input.Settings;

namespace FluidWarfare.Editor.Windows.Preferences;

sealed class EditorPreferencesDraftHandler
{
    EditorInputBindingSet _original = null!;
    EditorInputBindingSet _draft = null!;

    public void LoadFromService()
    {
        var s = EditorInputService.Instance;
        _original = s.GetCurrentBindingSet();
        _draft = _original with { Overrides = _original.Overrides.ToArray() };
    }

    public EditorInputGesture? GetEffective(string actionId, string slot)
    {
        var ov = _draft.Overrides.FirstOrDefault(o => o.ActionId == actionId && o.Slot == slot);
        if (ov is not null) return ov.Gesture;
        var bb = EditorInputActionCatalog.BlenderDefaultBindings.FirstOrDefault(b => b.ActionId == actionId);
        if (bb is null) return null;
        return slot == "primary" ? bb.PrimaryGesture : bb.SecondaryGesture;
    }

    public bool HasAnyChanges() => !_draft.HasSameEffectiveBindingsAs(_original);
    public bool HasOverride(string actionId) => _draft.Overrides.Any(o => o.ActionId == actionId);

    public void SetOverride(string actionId, string slot, EditorInputGesture? gesture)
    {
        var list = _draft.Overrides.ToList();
        list.RemoveAll(o => o.ActionId == actionId && o.Slot == slot);
        list.Add(new EditorInputBindingOverride { ActionId = actionId, Slot = slot, Gesture = gesture });
        _draft = _draft with { Overrides = list.ToArray() };
    }

    public void RemoveOverrides(string actionId)
    {
        _draft = _draft with { Overrides = _draft.Overrides.Where(o => o.ActionId != actionId).ToArray() };
    }

    public void ClearAll() { _draft = _draft with { Overrides = [] }; }

    public List<EditorInputBinding> BuildEffectiveBindingList()
    {
        var defs = EditorInputActionCatalog.BlenderDefaultBindings;
        var result = new List<EditorInputBinding>();
        foreach (var bb in defs)
        {
            var aId = bb.ActionId;
            var pOv = _draft.Overrides.FirstOrDefault(o => o.ActionId == aId && o.Slot == "primary");
            var sOv = _draft.Overrides.FirstOrDefault(o => o.ActionId == aId && o.Slot == "secondary");
            result.Add(bb with
            {
                PrimaryGesture = pOv is not null ? pOv.Gesture : bb.PrimaryGesture,
                SecondaryGesture = sOv is not null ? sOv.Gesture : bb.SecondaryGesture
            });
        }
        return result;
    }

    public bool DetectConflict(string actionId, EditorInputGesture gesture, out string? cid, out string? cs) =>
        EditorInputConflictDetector.DetectConflict(BuildEffectiveBindingList(), actionId, gesture, out cid, out cs);

    public bool Flush(bool closeAfterSave, Action<string> onError, Action onUpdate, Action onClose)
    {
        var doc = new EditorSettingsDocument { Input = _draft };
        if (!EditorSettingsWriter.TrySave(doc, out var e)) { onError($"保存设置失败：{e}"); return false; }
        if (!EditorInputService.Instance.TryApplyNewBindingSet(_draft, out var ae)) { onError($"应用绑定失败：{ae}"); return false; }
        _original = _draft;
        _draft = _draft with { Overrides = _draft.Overrides.ToArray() };
        onUpdate();
        if (closeAfterSave) onClose();
        return true;
    }
}

using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public static class DiagnosticRegistry
{
    static readonly Dictionary<string, Control> TargetsById = new(StringComparer.Ordinal);
    static readonly ReadOnlyDictionary<string, Control> ReadOnlyTargets = new(TargetsById);

    public static IReadOnlyDictionary<string, Control> Targets => ReadOnlyTargets;
    public static event EventHandler? Changed;

    public static void Register(Control target)
    {
        var id = XYDiagnostic.GetDebugId(target);
        if (id is null) return;
        DiagnosticId.Validate(id);
        if (!TargetsById.TryAdd(id, target))
            throw new InvalidOperationException($"诊断 ID 重复：{id}");
        Changed?.Invoke(null, EventArgs.Empty);
    }

    public static void Unregister(Control target)
    {
        var id = XYDiagnostic.GetDebugId(target);
        if (id is null || !TargetsById.TryGetValue(id, out var current) || !ReferenceEquals(current, target)) return;
        TargetsById.Remove(id); Changed?.Invoke(null, EventArgs.Empty);
    }

    public static void Clear()
    {
        if (TargetsById.Count == 0) return;
        TargetsById.Clear(); Changed?.Invoke(null, EventArgs.Empty);
    }
}

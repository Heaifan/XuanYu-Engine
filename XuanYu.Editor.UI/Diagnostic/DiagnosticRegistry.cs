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
        if (TargetsById.TryGetValue(id, out var current))
        {
            if (!IsAuto(id) || ReferenceEquals(current, target))
                throw new InvalidOperationException($"诊断 ID 重复：{id}");
            id = AllocateAutoId(id, TargetsById.Keys);
            XYDiagnostic.SetDebugId(target, id);
        }
        TargetsById.Add(id, target);
        Changed?.Invoke(null, EventArgs.Empty);
    }

    public static void Rebuild(IEnumerable<Control> targets)
    {
        var planned = new List<(Control Target, string Id)>();
        var used = new HashSet<string>(StringComparer.Ordinal);
        foreach (var target in targets)
        {
            var id = XYDiagnostic.GetDebugId(target);
            if (id is null) continue;
            DiagnosticId.Validate(id);
            if (!used.Add(id))
            {
                if (!IsAuto(id)) throw new InvalidOperationException($"诊断 ID 重复：{id}");
                id = AllocateAutoId(id, used); used.Add(id);
            }
            planned.Add((target, id));
        }
        foreach (var item in planned)
            if (XYDiagnostic.GetDebugId(item.Target) != item.Id) XYDiagnostic.SetDebugId(item.Target, item.Id);
        TargetsById.Clear();
        foreach (var item in planned) TargetsById.Add(item.Id, item.Target);
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

    static bool IsAuto(string id) => id.StartsWith("XYE.AUTO.", StringComparison.Ordinal);

    static string AllocateAutoId(string baseId, IEnumerable<string> used)
    {
        var occupied = used.ToHashSet(StringComparer.Ordinal);
        for (var suffix = 2; ; suffix++)
        {
            var id = $"{baseId}_{(char)('A' + suffix - 1)}";
            if (!occupied.Contains(id)) { DiagnosticId.Validate(id); return id; }
        }
    }
}

using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public static class DiagnosticRegistry
{
    static readonly Dictionary<string, Control> TargetsById = new(StringComparer.Ordinal);
    static readonly ReadOnlyDictionary<string, Control> ReadOnlyTargets = new(TargetsById);

    public static IReadOnlyDictionary<string, Control> Targets => ReadOnlyTargets;

    public static void Register(Control target)
    {
        var id = XYDiagnostic.GetDebugId(target);
        if (id is null) return;
        DiagnosticId.Validate(id);
        if (!TargetsById.TryAdd(id, target))
            throw new InvalidOperationException($"诊断 ID 重复：{id}");
    }

    public static void Clear() => TargetsById.Clear();
}

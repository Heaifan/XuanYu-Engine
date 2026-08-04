namespace XuanYu.Editor.Assets;

public sealed record StaticModelImportWarning(
    StaticModelImportWarningCode Code,
    string Message);

sealed class StaticModelWarningSet
{
    readonly Dictionary<StaticModelImportWarningCode, StaticModelImportWarning> _warnings = [];

    public void Add(StaticModelImportWarningCode code, string message) =>
        _warnings.TryAdd(code, new StaticModelImportWarning(code, message));

    public IReadOnlyList<StaticModelImportWarning> ToList() =>
        _warnings.Values.OrderBy(x => x.Code).ToArray();
}

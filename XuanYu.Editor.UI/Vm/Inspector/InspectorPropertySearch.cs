namespace XuanYu.Editor.UI;

public static class InspectorPropertySearch
{
    public static IReadOnlyList<InspectorPropertyDescriptor> Find(
        IEnumerable<InspectorPropertyDescriptor> properties, string? query)
    {
        var text = query?.Trim() ?? "";
        return properties.Where(item => text.Length == 0 || Matches(item, text)).ToArray();
    }

    static bool Matches(InspectorPropertyDescriptor item, string query) =>
        item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
        item.Alias.Contains(query, StringComparison.OrdinalIgnoreCase);
}

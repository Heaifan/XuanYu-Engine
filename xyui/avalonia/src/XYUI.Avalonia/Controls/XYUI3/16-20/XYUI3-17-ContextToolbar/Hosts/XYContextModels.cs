namespace XYUI.Avalonia.Controls;

public sealed record XYContextCategory(string Id, string Label);
public sealed record XYContextAction(string Id, string Label, Action? Execute = null, bool IsEnabled = true);

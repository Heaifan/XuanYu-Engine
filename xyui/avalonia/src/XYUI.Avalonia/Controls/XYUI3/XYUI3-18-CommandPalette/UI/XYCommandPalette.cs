using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public enum XYPaletteCommandType { Command, Object, Navigation, Setting }

public sealed record XYPaletteCommand
{
    public string Id { get; init; }
    public string Label { get; init; }
    public XYPaletteCommandType Type { get; init; }
    public string Category { get; init; }
    public string Description { get; init; }
    public string Shortcut { get; init; }
    public IReadOnlyList<string> Keywords { get; init; }
    public bool IsEnabled { get; init; }
    public string Prefix => Type switch { XYPaletteCommandType.Command => ">", XYPaletteCommandType.Object => "@", XYPaletteCommandType.Navigation => "#", XYPaletteCommandType.Setting => ":", _ => "" };

    public XYPaletteCommand(string label, string? category = null) : this(Slug(label), label, XYPaletteCommandType.Command, category ?? "命令", $"执行“{label}”命令。", "", [label], true) { }
    public XYPaletteCommand(string id, string label, XYPaletteCommandType type, string category, string description, string shortcut = "", IEnumerable<string>? keywords = null, bool isEnabled = true)
    { Id = id; Label = label; Type = type; Category = category; Description = description; Shortcut = shortcut; Keywords = keywords?.ToArray() ?? [label]; IsEnabled = isEnabled; }
    static string Slug(string value) => new(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
}

public sealed partial class XYCommandPalette : Border
{
    readonly Popup _popup = new() { Placement = PlacementMode.Center, IsLightDismissEnabled = true };
    readonly StackPanel _results = new(); readonly ScrollViewer _resultsViewport = new();
    readonly TextBlock _detailTitle = new(), _detailDescription = new(), _detailCategory = new(), _detailShortcut = new();
    Panel? _restoreParent; ContentControl? _restoreContentHost; int _restoreIndex; bool _reparenting; bool _closing; int _selected = -1;
    IActivatableLifetime? _applicationLifetime; WindowBase? _hostWindow;
    public XYSearchField SearchBox { get; } = new() { Placeholder = "输入命令或搜索...", Height = 34 };
    public XYMenu ScopeMenu { get; }
    public IReadOnlyList<XYPaletteCommand> Commands { get; }
    public IReadOnlyList<XYPaletteCommand> RecentItems { get; }
    public IReadOnlyList<XYPaletteCommand> FilteredCommands { get; private set; } = [];
    public XYPaletteCommand? SelectedCommand => _selected < 0 || _selected >= FilteredCommands.Count ? null : FilteredCommands[_selected];
    public XYPaletteCommandType? ScopeFilter { get; private set; }
    public bool IsOpen { get; private set; }
    public Popup PalettePopup => _popup;
    public event EventHandler<XYPaletteCommand>? ExecuteRequested;
    public XYCommandPalette(params XYPaletteCommand[] commands) : this(commands, null) { }
    public XYCommandPalette(IEnumerable<XYPaletteCommand> commands, IEnumerable<XYPaletteCommand>? recentItems = null)
    {
        Commands = commands.ToArray(); RecentItems = (recentItems ?? Commands).ToArray(); Classes.Add("xyui-command-palette");
        ScopeMenu = CreateScopeMenu(); SearchBox.FilterContent = ScopeMenu; SearchBox.TextChanged += (_, _) => Refresh(); SearchBox.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Bubble, true); SearchBox.FilterRequested += OnFilterRequested;
        _popup.Closed += (_, _) => Close(); Child = BuildSurface(); Refresh();
    }
}

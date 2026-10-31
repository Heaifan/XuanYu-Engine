using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Metadata;
using System.Collections.ObjectModel;

namespace XYUI.Avalonia.Controls;

public enum XYPaletteCommandType { Command, Object, Navigation, Setting }

public sealed record XYPaletteCommand
{
    public string Id { get; set; }
    public string Label { get; set; }
    public XYPaletteCommandType Type { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public string Shortcut { get; set; }
    public IReadOnlyList<string> Keywords { get; set; }
    public bool IsEnabled { get; set; }
    public string Prefix => Type switch { XYPaletteCommandType.Command => ">", XYPaletteCommandType.Object => "@", XYPaletteCommandType.Navigation => "#", XYPaletteCommandType.Setting => ":", _ => "" };

    public XYPaletteCommand() : this("", "", XYPaletteCommandType.Command, "命令", "", "", []) { }
    public XYPaletteCommand(string label, string? category = null) : this(Slug(label), label, XYPaletteCommandType.Command, category ?? "命令", $"执行“{label}”命令。", "", [label], true) { }
    public XYPaletteCommand(string id, string label, XYPaletteCommandType type, string category, string description, string shortcut = "", IEnumerable<string>? keywords = null, bool isEnabled = true)
    { Id = id; Label = label; Type = type; Category = category; Description = description; Shortcut = shortcut; Keywords = keywords?.ToArray() ?? [label]; IsEnabled = isEnabled; }
    static string Slug(string value) => new(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
}

public sealed partial class XYCommandPalette : Border
{
    readonly Popup _popup = new() { Placement = PlacementMode.Center, IsLightDismissEnabled = true };
    readonly StackPanel _results = new(); readonly ScrollViewer _resultsViewport = new();
    readonly TextBlock _detailTitle = new(), _detailDescription = new(), _detailCategory = new(), _detailShortcut = new(), _emptyState = new();
    Panel? _restoreParent; ContentControl? _restoreContentHost; int _restoreIndex; bool _reparenting; bool _closing; int _selected = -1;
    IActivatableLifetime? _applicationLifetime; WindowBase? _hostWindow;
    public XYSearchField SearchBox { get; } = new() { Placeholder = "输入命令或搜索...", Height = 34 };
    public XYMenu ScopeMenu { get; }
    [Content] public ObservableCollection<XYPaletteCommand> Items { get; } = [];
    public IReadOnlyList<XYPaletteCommand> Commands => Items;
    public ObservableCollection<XYPaletteCommand> RecentItems { get; } = [];
    public IReadOnlyList<XYPaletteCommand> FilteredCommands { get; private set; } = [];
    public XYPaletteCommand? SelectedCommand => _selected < 0 || _selected >= FilteredCommands.Count ? null : FilteredCommands[_selected];
    public XYPaletteCommandType? ScopeFilter { get; private set; }
    public bool IsOpen { get; private set; }
    public string OpenShortcut { get; set; } = "Ctrl+K";
    public Popup PalettePopup => _popup;
    public event EventHandler<XYPaletteCommand>? ExecuteRequested;
    public XYCommandPalette(params XYPaletteCommand[] commands) : this(commands, null) { }
    public XYCommandPalette(IEnumerable<XYPaletteCommand> commands, IEnumerable<XYPaletteCommand>? recentItems = null)
    {
        foreach (var command in commands) Items.Add(command); foreach (var command in recentItems ?? Items) RecentItems.Add(command); Items.CollectionChanged += (_, _) => Refresh(); Classes.Add("xyui-command-palette"); KeyDown += OnPaletteKeyDown;
        ScopeMenu = CreateScopeMenu(); SearchBox.FilterContent = ScopeMenu; SearchBox.TextChanged += (_, _) => Refresh(); SearchBox.PropertyChanged += (_, e) => { if (e.Property == TextBox.TextProperty) Refresh(); }; SearchBox.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Bubble, true); SearchBox.FilterRequested += OnFilterRequested;
        _popup.Closed += (_, _) => Close(); Child = BuildSurface(); Refresh();
    }
}

using System.ComponentModel;
using Avalonia.Controls;
using XYUI.Avalonia.Gallery.Views;

namespace XYUI.Avalonia.Gallery;

public sealed partial class XYUI1DocumentationViewModel : INotifyPropertyChanged
{
    public IReadOnlyList<XYUI1ComponentDocument> Documents { get; } = XYUI1DocumentationCatalog.Build();
    public int ImplementedCount => Documents.Count;
    public int CanonicalAlignedCount => Documents.Count;
    public int ReadyCount => Documents.Count(x => string.IsNullOrEmpty(x.KnownGap));
    public int ReadyWithGapCount => Documents.Count(x => !string.IsNullOrEmpty(x.KnownGap));
    public int GapCount => ReadyWithGapCount;
    public int VisualAcceptedCount => Documents.Count;
    public IReadOnlyList<XYUI1NavigationItem> Items { get; }
    public IReadOnlyList<XYUI1NavigationItem> ComponentItems => Items.Skip(1).ToArray();

    bool _isX1;
    public bool IsXYUI1Expanded { get => _isX1; set { if (_isX1 == value) return; _isX1 = value; PropertyChanged?.Invoke(this, new(nameof(IsXYUI1Expanded))); } }
    bool _isX2 = true;
    public bool IsXYUI2Expanded { get => _isX2; set { if (_isX2 == value) return; _isX2 = value; PropertyChanged?.Invoke(this, new(nameof(IsXYUI2Expanded))); } }
    public string XYUI1CountText => "24/24";
    public string XYUI2CountText => "24/24";
    XYUI1NavigationItem _selectedItem;
    FoundationNavigationItem? _selectedFoundation;

    public XYUI1NavigationItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (value == _selectedItem) return;
            _selectedItem = value;
            _selectedFoundation = null;
            SelectedDocument = value.Document is null
                ? new XYUI1ModuleOverviewView { DataContext = this }
                : new XYUI1ComponentDocumentView { DataContext = value.Document };
            PropertyChanged?.Invoke(this, new(nameof(SelectedItem)));
            PropertyChanged?.Invoke(this, new(nameof(SelectedFoundation)));
            PropertyChanged?.Invoke(this, new(nameof(SelectedDocument)));
        }
    }

    public FoundationNavigationItem? SelectedFoundation
    {
        get => _selectedFoundation;
        set
        {
            if (value == _selectedFoundation) return;
            _selectedFoundation = value;
            if (value is not null) SelectedDocument = CreateFoundationView(value.Id);
            PropertyChanged?.Invoke(this, new(nameof(SelectedFoundation)));
            PropertyChanged?.Invoke(this, new(nameof(SelectedDocument)));
        }
    }

    public Control SelectedDocument { get; private set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public XYUI1DocumentationViewModel()
    {
        var documents = Documents.Select(x => new XYUI1NavigationItem(x.Id, x.ChineseName, x.EnglishName, x)).ToArray();
        Items = new[] { new XYUI1NavigationItem("XYUI-1", "模块概览", "Text & Information", null) }.Concat(documents).ToArray();
        _selectedItem = Items[0];
        SelectedDocument = new XYUI1ModuleOverviewView { DataContext = this };
        BootstrapXYUI2(); BootstrapXYUI3();
    }

    public void Select(string id)
    {
        var item = Items.FirstOrDefault(x => x.Id == id);
        if (item is not null) SelectedItem = item;
        else if (XYUI2Items.Any(x => x.Id == id)) SelectXYUI2(id);
        else if (XYUI3Items.Any(x => x.Id == id)) SelectXYUI3(id);
    }
}

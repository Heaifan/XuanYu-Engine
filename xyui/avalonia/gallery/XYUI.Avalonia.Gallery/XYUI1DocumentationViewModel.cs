using System.ComponentModel;
using Avalonia.Controls;
using XYUI.Avalonia.Gallery.Views;

namespace XYUI.Avalonia.Gallery;

public sealed class XYUI1DocumentationViewModel : INotifyPropertyChanged
{
    public IReadOnlyList<XYUI1ComponentDocument> Documents { get; } = XYUI1DocumentationCatalog.Build();
    public IReadOnlyList<XYUI1NavigationItem> Items { get; }
    public IReadOnlyList<XYUI1NavigationItem> ComponentItems => Items.Skip(1).ToArray();
    public IReadOnlyList<FoundationNavigationItem> FoundationItems { get; } =
    [new("palette", "色彩", "Palette"), new("typography", "字体与排版", "Typography"), new("shape", "形状", "Shape")];
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
    }

    public void Select(string id)
    {
        var item = Items.FirstOrDefault(x => x.Id == id);
        if (item is not null) SelectedItem = item;
    }

    public void SelectFoundation(string id)
    {
        var item = FoundationItems.FirstOrDefault(x => x.Id == id);
        if (item is not null) SelectedFoundation = item;
    }

    static Control CreateFoundationView(string id) => id switch
    {
        "palette" => new PaletteView(),
        "typography" => new TypographyView(),
        "shape" => new ShapeView(),
        _ => new XYUI1ModuleOverviewView()
    };
}

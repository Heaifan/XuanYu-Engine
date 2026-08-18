using System.ComponentModel;
using Avalonia.Controls;
using XYUI.Avalonia.Gallery.Views;

namespace XYUI.Avalonia.Gallery;

public sealed class XYUI1DocumentationViewModel : INotifyPropertyChanged
{
    public IReadOnlyList<XYUI1ComponentDocument> Documents { get; } = XYUI1DocumentationCatalog.Build();
    public IReadOnlyList<XYUI1NavigationItem> Items { get; }
    public IReadOnlyList<XYUI1NavigationItem> ComponentItems => Items.Skip(1).ToArray();
    XYUI1NavigationItem _selectedItem;

    public XYUI1NavigationItem SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (value == _selectedItem) return;
            _selectedItem = value;
            SelectedDocument = value.Document is null
                ? new XYUI1ModuleOverviewView { DataContext = this }
                : new XYUI1ComponentDocumentView { DataContext = value.Document };
            PropertyChanged?.Invoke(this, new(nameof(SelectedItem)));
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
}

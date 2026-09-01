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
    public IReadOnlyList<FoundationNavigationItem> FoundationItems { get; } =
    [
        new("palette", "色彩", "Palette"),
        new("typography", "字体与排版", "Typography"),
        new("spacing_layout", "间距与排版", "Spacing & Layout"),
        new("sizing", "尺寸控制", "Sizing"),
        new("density", "信息密度", "Density"),
        new("iconography", "图标体系", "Iconography"),
        new("radius_border_separator", "圆角/边框/分割线", "Radius / Border / Separator"),
        new("shape", "形状", "Shape"),
        new("surface", "表面层级", "Surface"),
        new("states", "交互状态", "States"),
        new("responsive", "响应式", "Responsive"),
        new("accessibility", "无障碍", "Accessibility"),
        new("layout_recipes", "组合模板", "Layout Recipes")
    ];

    // G0-R1 · 树形章节（非 Accordion 卡片；仅改导航呈现）
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

    public void SelectFoundation(string id)
    {
        var targetId = id switch
        {
            "color" => "palette",
            _ => id
        };
        var item = FoundationItems.FirstOrDefault(x => x.Id == targetId || x.Id == id);
        if (item is not null) SelectedFoundation = item;
    }

    static Control CreateFoundationView(string id) => id switch
    {
        "palette" or "color" => new PaletteView(),
        "typography" => new TypographyView(),
        "spacing_layout" => new SpacingLayoutView(),
        "sizing" => new SizingView(),
        "density" => new DensityView(),
        "iconography" => new IconographyView(),
        "radius_border_separator" or "shape" => new ShapeView(),
        "surface" => new SurfaceView(),
        "states" => new StatesView(),
        "responsive" => new ResponsiveView(),
        "accessibility" => new AccessibilityView(),
        "layout_recipes" => new LayoutRecipesView(),
        _ => new XYUI1ModuleOverviewView()
    };
}

using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Gallery.Views;

namespace XYUI.Avalonia.Gallery;

public sealed partial class XYUI1DocumentationViewModel
{
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

    public void SelectFoundation(string id)
    {
        var targetId = id switch { "color" => "palette", "spacing" => "spacing_layout", _ => id };
        var item = FoundationItems.FirstOrDefault(x => x.Id == targetId || x.Id == id);
        if (item is not null) SelectedFoundation = item;
    }

    static Control CreateFoundationView(string id)
    {
        if (Application.Current is null) return new Control();
        return id switch
        {
            "palette" or "color" => new PaletteView(),
            "typography" => new TypographyView(),
            "spacing_layout" or "spacing" => new SpacingLayoutView(),
            "sizing" => new SizingView(),
            "density" => new DensitySamplesView(),
            "iconography" => new IconographyView(),
            "radius_border_separator" => new RadiusBorderSeparatorView(),
            "shape" => new ShapeView(),
            "surface" => new SurfaceView(),
            "states" => new StatesView(),
            "responsive" => new ResponsiveView(),
            "accessibility" => new AccessibilityView(),
            "layout_recipes" => new LayoutRecipesView(),
            _ => new XYUI1ModuleOverviewView()
        };
    }
}
using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] SearchFields() =>
    [
        SearchSample("Default", new XYSearchField { Width = 300, Placeholder = "搜索资产..." }),
        SearchSample("With Content", new XYSearchField { Width = 300, Text = "terrain_texture" }),
    ];

    static Control[] PasswordFields() =>
    [
        PasswordSample("Masked", new XYPasswordField { Width = 300, Password = "XuanYu_Secure2026" }),
        PasswordSample("Reveal · 按住眼睛查看", new XYPasswordField { Width = 300, Password = "XuanYu_2026" }),
    ];

    static Control SearchSample(string caption, XYSearchField field) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, field } };
    static Control PasswordSample(string caption, XYPasswordField field) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = caption }, field } };
}

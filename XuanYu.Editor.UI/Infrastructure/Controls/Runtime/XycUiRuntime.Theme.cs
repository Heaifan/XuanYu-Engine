using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI.XycControl;

internal sealed partial class XycUiRuntime
{
    XycUiThemeState InspectThemeCore()
    {
        var scope = FindThemeScope();
        var app = Application.Current;
        if (scope is null && app is null)
            throw new InvalidOperationException("生产主题 Variant 尚未就绪。");
        return scope is not null
            ? new(Variant(scope.RequestedThemeVariant), Variant(scope.ActualThemeVariant))
            : new(Variant(app!.RequestedThemeVariant), Variant(app.ActualThemeVariant));
    }

    XycUiThemeState SetThemeCore(string theme)
    {
        if (theme is not ("Light" or "Dark"))
            throw new ArgumentException("主题只接受 Light 或 Dark。");
        var current = InspectThemeCore();
        if (current.Actual != theme)
        {
            var top = FindTop();
            var scope = FindThemeScope();
            if (scope is not null) SetRequestedTheme(scope, theme);
            else if (Application.Current is { } app) SetApplicationTheme(app, theme);
            else throw new InvalidOperationException("生产主题 Variant 尚未就绪。");
            top.InvalidateVisual();
            window.InvalidateVisual();
        }
        var actual = InspectThemeCore();
        if (actual.Actual != theme)
            throw new InvalidOperationException($"生产主题切换状态不一致：actual={actual.Actual};expected={theme}。");
        return actual;
    }

    internal static void SetRequestedTheme(ThemeVariantScope scope, string theme) =>
        scope.RequestedThemeVariant = TargetVariant(theme);

    internal static void SetApplicationTheme(Application app, string theme) =>
        app.RequestedThemeVariant = TargetVariant(theme);

    ThemeVariantScope? FindThemeScope() => FindTop().GetVisualDescendants()
        .OfType<ThemeVariantScope>().SingleOrDefault();

    static ThemeVariant TargetVariant(string theme) => theme switch
    {
        "Light" => ThemeVariant.Light,
        "Dark" => ThemeVariant.Dark,
        _ => throw new ArgumentException("主题只接受 Light 或 Dark。", nameof(theme))
    };

    static string Variant(ThemeVariant? value) => value?.ToString() ?? "Default";
}

using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Svg.Skia;

namespace FluidWarfare.Editor.Windows.Panels.HierarchyVisual;

/// <summary>
/// SVG 图标缓存与加载。URI → 已渲染 Bitmap，不重复解析。
/// 仅用于 Hierarchy 树节点图标。
/// </summary>
public static class HierarchySvgIcon
{
    /// <summary>SVG 文件所在的资源基路径。</summary>
    private const string AssetBase = "avares://FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/";

    /// <summary>图标尺寸（像素）。</summary>
    public const int IconSize = 15;

    /// <summary>展开箭头尺寸（像素）。</summary>
    public const int ArrowSize = 12;

    private static readonly Dictionary<string, Bitmap> _cache = new();

    /// <summary>
    /// 获取缓存中的 SVG 图标。第一次加载时解析并缓存。
    /// </summary>
    public static Bitmap? GetIcon(string iconName)
    {
        if (_cache.TryGetValue(iconName, out var cached))
            return cached;

        var uri = $"{AssetBase}{iconName}.svg";
        Bitmap? bitmap = null;

        try
        {
            var svg = new SKSvg();
            using var stream = AssetLoader.Open(new Uri(uri));
            if (svg.Load(stream) is { } picture && picture is not null)
            {
                using var surface = SkiaSharp.SKSurface.Create(
                    new SkiaSharp.SKImageInfo(IconSize, IconSize));
                var canvas = surface.Canvas;
                canvas.Clear(SkiaSharp.SKColors.Transparent);
                var scale = Math.Min(
                    IconSize / (float)picture.CullRect.Width,
                    IconSize / (float)picture.CullRect.Height);
                canvas.Scale(scale);
                canvas.DrawPicture(picture);
                using var image = surface.Snapshot();
                using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
                using var ms = new MemoryStream(data.ToArray());
                bitmap = new Bitmap(ms);
            }
        }
        catch
        {
            // SVG 加载失败返回 null，不崩溃
        }

        _cache[iconName] = bitmap!;
        return bitmap;
    }

    /// <summary>
    /// 清除所有缓存（项目重载时调用）。
    /// </summary>
    public static void ClearCache() => _cache.Clear();

    /// <summary>
    /// 根据节点信息解析图标名称。
    /// </summary>
    public static string ResolveIconName(string? iconKind, bool isExpanded)
    {
        return iconKind switch
        {
            "world" => "world",
            "project" => "project",
            "group" when isExpanded => "units",
            "group" => "units",
            "entity" => "unit-entity",
            "folder" when isExpanded => "folder-open",
            "folder" => "folder",
            "file" => "file-json",
            _ => "file-json"
        };
    }
}

namespace XuanYu.Render.Abstractions;

public enum RenderLabelPixelFormat
{
    Bgra8888Unpremultiplied
}

public sealed record RenderLabelBitmap(
    string CacheKey,
    int Width,
    int Height,
    int Stride,
    RenderLabelPixelFormat PixelFormat,
    byte[] Pixels);

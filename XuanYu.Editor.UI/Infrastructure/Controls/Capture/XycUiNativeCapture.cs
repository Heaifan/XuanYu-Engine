using System.Runtime.InteropServices;

namespace XuanYu.Editor.UI.XycControl;

internal static class XycUiNativeCapture
{
    [StructLayout(LayoutKind.Sequential)]
    struct BitmapHeader
    {
        public uint Size;
        public int Width, Height;
        public ushort Planes, BitsPerPixel;
        public uint Compression, ImageSize;
        public int XResolution, YResolution;
        public uint ColorsUsed, ImportantColors;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct BitmapInfo { public BitmapHeader Header; public uint Colors; }

    [DllImport("user32.dll", SetLastError = true)]
    static extern nint GetDC(nint hwnd);
    [DllImport("user32.dll", SetLastError = true)]
    static extern int ReleaseDC(nint hwnd, nint hdc);
    [DllImport("gdi32.dll", SetLastError = true)]
    static extern nint CreateCompatibleDC(nint hdc);
    [DllImport("gdi32.dll", SetLastError = true)]
    static extern bool DeleteDC(nint hdc);
    [DllImport("gdi32.dll", SetLastError = true)]
    static extern nint CreateDIBSection(nint hdc, ref BitmapInfo info, uint usage,
        out nint bits, nint section, uint offset);
    [DllImport("gdi32.dll", SetLastError = true)]
    static extern nint SelectObject(nint hdc, nint obj);
    [DllImport("gdi32.dll", SetLastError = true)]
    static extern bool DeleteObject(nint obj);
    [DllImport("gdi32.dll", SetLastError = true)]
    static extern bool BitBlt(nint destination, int x, int y, int width, int height,
        nint source, int sourceX, int sourceY, uint operation);

    public static byte[] ReadScreen(int x, int y, int width, int height)
    {
        var source = GetDC(0);
        if (source == 0) throw new InvalidOperationException("桌面 DC 获取失败。");
        var memory = CreateCompatibleDC(source);
        nint dib = 0, previous = 0;
        try
        {
            var info = Info(width, height);
            dib = CreateDIBSection(source, ref info, 0, out var bits, 0, 0);
            if (dib == 0 || bits == 0) throw new InvalidOperationException("PrintWindow DIB 创建失败。");
            previous = SelectObject(memory, dib);
            if (!BitBlt(memory, 0, 0, width, height, source, x, y, 0x00CC0020))
                throw new InvalidOperationException($"桌面像素复制失败：{Marshal.GetLastWin32Error()}。");
            var pixels = new byte[checked(width * height * 4)];
            Marshal.Copy(bits, pixels, 0, pixels.Length);
            return pixels;
        }
        finally
        {
            if (previous != 0) SelectObject(memory, previous);
            if (dib != 0) DeleteObject(dib);
            if (memory != 0) DeleteDC(memory);
            ReleaseDC(0, source);
        }
    }

    static BitmapInfo Info(int width, int height) => new()
    {
        Header = new BitmapHeader { Size = 40, Width = width, Height = -height,
            Planes = 1, BitsPerPixel = 32, Compression = 0,
            ImageSize = checked((uint)(width * height * 4)) }
    };
}

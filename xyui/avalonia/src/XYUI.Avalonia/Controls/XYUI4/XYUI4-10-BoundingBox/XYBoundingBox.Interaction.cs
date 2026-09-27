using Avalonia;
using Avalonia.Input;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYBoundingBox
{
    enum HandleMode { None, Resize, Rotate, Pivot }
    HandleMode _mode; int _handle = -1; Rect _startBounds; double _startAngle; double _startPointerAngle;

    void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _startBounds = BoundsRect; _startAngle = Angle;
        var point = e.GetPosition(this); var local = Rotate(point, BoundsRect.Center, -Angle);
        var rotation = new Point(BoundsRect.Center.X, BoundsRect.Top - 24);
        if (ShowRotationHandle && Distance(local, rotation) <= 14) _mode = HandleMode.Rotate;
        else if (ShowPivot && Distance(local, Pivot) <= 14) _mode = HandleMode.Pivot;
        else { _handle = NearestHandle(local); _mode = _handle < 0 ? HandleMode.None : HandleMode.Resize; }
        if (_mode == HandleMode.None) return;
        _startPointerAngle = Math.Atan2(point.Y - BoundsRect.Center.Y, point.X - BoundsRect.Center.X);
        e.Pointer.Capture(this); e.Handled = true;
    }

    void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_mode == HandleMode.None) return;
        var point = e.GetPosition(this); var local = Rotate(point, _startBounds.Center, -_startAngle);
        if (_mode == HandleMode.Resize) Resize(local);
        else if (_mode == HandleMode.Rotate) Angle = _startAngle + Degrees(Math.Atan2(point.Y - _startBounds.Center.Y, point.X - _startBounds.Center.X) - _startPointerAngle);
        else Pivot = Rotate(local, _startBounds.Center, _startAngle);
        e.Handled = true;
    }

    void OnPointerReleased(object? sender, PointerReleasedEventArgs e) { if (_mode == HandleMode.None) return; e.Pointer.Capture(null); _mode = HandleMode.None; _handle = -1; e.Handled = true; }
    void Resize(Point p)
    {
        var left = _startBounds.Left; var top = _startBounds.Top; var right = _startBounds.Right; var bottom = _startBounds.Bottom;
        if (_handle is 0 or 3 or 5) left = p.X; if (_handle is 2 or 4 or 7) right = p.X;
        if (_handle is 0 or 1 or 2) top = p.Y; if (_handle is 5 or 6 or 7) bottom = p.Y;
        if (right - left < 32) right = left + 32; if (bottom - top < 24) bottom = top + 24;
        BoundsRect = new Rect(left, top, right - left, bottom - top);
    }

    int NearestHandle(Point p) { var handles = Handles(_startBounds).ToArray(); var index = Array.FindIndex(handles, x => Distance(p, x) <= 14); return index; }
    static double Distance(Point a, Point b) => Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    static double Degrees(double radians) => radians * 180 / Math.PI;
    static Point Rotate(Point point, Point center, double radians) { var x = point.X - center.X; var y = point.Y - center.Y; var c = Math.Cos(radians); var s = Math.Sin(radians); return new Point(center.X + x * c - y * s, center.Y + x * s + y * c); }
}

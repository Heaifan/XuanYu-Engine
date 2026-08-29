using Avalonia.Media;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public partial class XYColorPicker
{
    internal bool TryParseHex(string? text, out Color color)
    {
        color = Color; var value = text?.Trim().TrimStart('#') ?? ""; if (value.Length is not (6 or 8)) return false;
        if (!uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var raw)) return false;
        var alpha = value.Length == 8 ? (byte)(raw & 255) : (byte)255; var rgb = value.Length == 8 ? raw >> 8 : raw;
        color = Color.FromArgb(alpha, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb); return true;
    }
    internal void OnHexCommitted() { if (TryParseHex(HexField?.Text, out var color)) { ErrorPart!.IsVisible = false; SetColor(color); } else ErrorPart!.IsVisible = true; }
    void CommitByte(TextBox? field, char channel)
    {
        if (!byte.TryParse(field?.Text, out var value)) return; var color = channel switch { 'R' => Color.FromArgb(Color.A, value, Color.G, Color.B), 'G' => Color.FromArgb(Color.A, Color.R, value, Color.B), 'B' => Color.FromArgb(Color.A, Color.R, Color.G, value), _ => Color.FromArgb(value, Color.R, Color.G, Color.B) }; SetColor(color);
    }
    void OnRedCommitted() => CommitByte(RedField, 'R');
    void OnGreenCommitted() => CommitByte(GreenField, 'G');
    void OnBlueCommitted() => CommitByte(BlueField, 'B');
    void OnAlphaCommitted() => CommitByte(AlphaField, 'A');
}

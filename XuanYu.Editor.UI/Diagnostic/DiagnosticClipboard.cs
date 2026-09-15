using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input.Platform;

namespace XuanYu.Editor.UI;

public interface IDiagnosticClipboard
{
    Task SetTextAsync(Control target, string text);
}

public sealed class DiagnosticClipboard : IDiagnosticClipboard
{
    public async Task SetTextAsync(Control target, string text)
    {
        try
        {
            var clipboard = TopLevel.GetTopLevel(target)?.Clipboard;
            if (clipboard is not null) await clipboard.SetTextAsync(text);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Diagnostic] clipboard copy failed: {ex}");
        }
    }
}

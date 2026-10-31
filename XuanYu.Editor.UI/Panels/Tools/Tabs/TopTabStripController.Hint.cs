using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D3：首次溢出一次性提示（合同 §10.1-7）。
// 只在当前用户环境首次触发：状态持久化到 %APPDATA%\XuanYuEngine\ui-once.json（本地用户状态，不进 git）。
public sealed partial class TopTabStripController
{
    const double HintDurationMs = 3000;
    bool _hintShownThisSession;

    static string HintStatePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "XuanYuEngine", "ui-once.json");

    void TryShowHintOnce()
    {
        if (_hintPopup is null || _disposed) return;
        if (!TopTabStripModel.ShouldShowHint(_model.Overflowing, _hintShownThisSession, IsHintPersisted()))
            return;
        _hintShownThisSession = true;
        PersistHint();
        _hintPopup.IsOpen = true;
        CloseHintLater();
    }

    async void CloseHintLater()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(HintDurationMs));
        if (!_disposed && _hintPopup is not null) _hintPopup.IsOpen = false;
    }

    static bool IsHintPersisted()
    {
        try
        {
            return File.Exists(HintStatePath)
                && JsonDocument.Parse(File.ReadAllText(HintStatePath)).RootElement
                    .TryGetProperty("TabStripOverflowHintShown", out var v) && v.GetBoolean();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[TopTabStrip] hint state read failed: {ex.Message}");
            return false;
        }
    }

    static void PersistHint()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(HintStatePath)!);
            File.WriteAllText(HintStatePath, "{\"TabStripOverflowHintShown\":true}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[TopTabStrip] hint state write failed: {ex.Message}");
        }
    }
}

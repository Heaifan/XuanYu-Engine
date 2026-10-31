using System;
using System.Threading.Tasks;

namespace XuanYu.Editor.UI;

public partial class UiWin
{
    async Task<T> RunNativePicker<T>(Func<Task<T>> picker)
    {
        DiagnosticOverlay.SuspendForNativeDialog();
        try { return await picker(); }
        finally { DiagnosticOverlay.RestoreAfterNativeDialog(); }
    }
}

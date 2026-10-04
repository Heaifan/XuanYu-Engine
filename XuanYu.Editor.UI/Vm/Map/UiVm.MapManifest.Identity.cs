using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool TryRequireCurrentMapManifestIdentity()
    {
        var result = MapManifestIdentityValidator.Validate(MapSession.CurrentMap.MapId, CurrentMapManifest);
        if (result.Succeeded) return true;
        FooterState = "状态：不可用";
        FooterMessage = $"区域绘制已阻止：{result.Message}";
        return false;
    }
}

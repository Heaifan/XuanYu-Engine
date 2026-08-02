namespace XuanYu.Editor.UI;

// D4：最小错误弹窗服务。只用于用户主动操作失败（导入 GLB / 打开场景 / 部分资源缺失）。
// 禁止 Core / World / Render.Vulkan 直接弹窗；UI 层用户命令才允许触发。
public interface IEditorDialogService
{
    Task ShowErrorAsync(string title, string message);

    Task ShowWarningAsync(string title, string message);
}

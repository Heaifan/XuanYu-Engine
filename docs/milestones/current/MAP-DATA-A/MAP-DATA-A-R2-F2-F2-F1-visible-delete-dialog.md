# MAP-DATA-A-R2-F2-F2-F1 · Visible Delete Dialog

状态：`CLOSED`；Dataset-backed 路由漏分支已修复，用户裁定 C01～C07 全 PASS；F3 Snap 已解除冻结。

## T1 根因结论

真机表现为删除后主 Avalonia UI 无输入、Vulkan 视口仍可操作、确认卡不可见，而 Esc 能取消并恢复 UI。确认流程和键盘路由已激活；`DialogCard` 是 Main Window 内的 Avalonia Visual，正覆盖 `VulkanNativeHost : NativeControlHost` 的 Native HWND。HWND airspace 将卡片压在下方，故这是 Native Host Airspace，不是 ZIndex、布局、状态不同步或 Visual Tree 挂载错误。

## T2 最小修复

普通“删除图层”和 Dataset-backed“解除注册数据集”都改为 Editor Owner 的独立 Avalonia Window，使用 `ShowDialog(owner)` 管理模态关系。两者保留不同标题、说明和确认按钮；解除注册明确保留磁盘 Dataset 文件。取消、Esc、Enter、关闭 X 统一返回 false；确认前捕获稳定 LayerId，确认后按 ID 重新验证并执行。

P1 Probe 证明 Dataset-backed 分支曾绕过新窗口并回落旧 `ShowDanger()`；Probe 已删除。用户已真机确认该分支现在可见，剩余 F1 项仍须按验收表逐项执行。

业务实现文件共 6 个，用户已批准这是独立 Window AXAML/code-behind 所必需的一次性例外。不得增加第 7 个业务文件，不得把其他 Dialog 迁移为 Window，不得回退为 Overlay + ZIndex 覆盖 NativeHost。

## 禁止项确认

- 未修改 Vulkan Renderer、Swapchain、Fence、Picking、Region/Road 输入、Schema、Manifest 或 Geometry。
- 未修改 Undo/Redo 总架构，未创建通用 Dialog 框架。
- `_tmp_blind_rows/` 未修改、未删除、未提交。

# REGION-INSPECTOR-COLOR-PICKER-R1

## Task State

Risk: MEDIUM
Goal: 用 XYColorPicker 替换 Region 填充色文本输入，同时保持单次事务提交。
Base: `e1964f1193d15850f35b25aa0e3c17289a2cc1c0`
Branch: `fix/region-inspector-color-picker-r1`

## Scope

1. Region Inspector 的 `Region.Style.FillColor` 使用 `XYColorPicker`。
2. Picker 固定 RGB 模式，不开放 Alpha。
3. 打开 Picker 时锁定 `InspectorEditTarget`。
4. 拖动/调色只更新 Editor.UI 临时预览，不写 `MapSession`。
5. 外部正常关闭或 Enter 只提交一次。
6. Esc、Window Deactivate、控件卸载、Selection 切换均取消预览。
7. 最终提交仍只调用既有 `MapEditSession.SetRegionFillColor`。
8. Selection 黄色边框/顶点保持现状。

## Prohibited

- 不修改 Vulkan Renderer。
- 不修改 Region Dataset / .xymap Schema。
- 不新增第二套 Undo。
- 不在 ColorChanged 中直接 Commit。
- 不开放边界颜色。
- 不修改 5+100 红线。
- 不扩大 Native HWND 特例。

## Transaction

```text
Open
→ Capture InspectorEditTarget + OriginalRgb
→ ColorChanged
→ Editor.UI Preview only
→ Commit close / Enter
→ SetRegionFillColor exactly once
```

取消：

```text
Esc / Selection change / Deactivate / Detach
→ discard preview
→ domain color unchanged
→ no Undo entry
```

## Acceptance

1. Region 基础页显示 XYColorPicker。
2. Picker 初值来自 `MapRegion.FillColorRgb`。
3. Picker 为 RGB 模式，无 Alpha 编辑器。
4. 连续拖动时 `MapSession.ChangeSequence` 不变化。
5. 连续拖动时 Region 画面可实时预览。
6. 确认后只产生一次内容提交。
7. Undo 一次恢复确认前颜色。
8. Esc 取消不修改 Region。
9. 切换到其他 Region 不误提交旧 Picker。
10. 非法 HEX 不产生领域提交。
11. 边界颜色和 Selection 黄色视觉不变。
12. Region Dataset Schema 不变化。

## Gate

- 受影响项目 Build。
- XYColorPicker 专项测试。
- Region Inspector 专项测试。
- 5+100。
- git diff --check。
- run.bat 人工：调色预览、确认、Esc、切 Region、Undo/Redo。

环境缺失 SDK 时，只允许声明静态审计完成，不得声明 Build/Test PASS。

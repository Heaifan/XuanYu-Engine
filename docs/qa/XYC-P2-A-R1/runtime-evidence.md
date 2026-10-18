# XYC-P2-A-R1 Candidate Runtime Evidence

## Candidate 与会话

- Candidate Tree Identity：`A66A2B0E55AD742B5203A971A3B0D4F9BAC327DBBC1EAD964725F317539C06D5`
- 基线 HEAD：`d0335c52b0c9938277f571cc58d11c4d038f3b43`
- Session：`5fa799ef0d894e42bb35115c28ed95ba`
- Editor PID：`15636`；HWND：`0x9A1104`
- 启动方式：纯 Candidate Release `XuanYu.Editor.Control.Cli.exe --xyc-ui --diagnostic-write` 启动真实生产 Editor。
- Editor 版本身份通过 Candidate Tree Identity、`Editor.App.exe`、`XuanYu.Editor.UI.dll`、CLI DLL 的 SHA-256 关联；TOP PNG sidecar 也记录 App 与 UI DLL 哈希。
- 测试场景：`75adc6cdc73e46a6b52c733940e26288`，启动时 `ScenePath=""`、实体/Hierarchy/RenderSnapshot 均空、History 为 `0/0`。

## 实体与事务序列

新建 Cube 的实际 ID 为 `EntityId(1)`。该 ID 同时出现在 World 实体清单、实际 Hierarchy 投影和 RenderSnapshot 投影。Inspector 的“实体编号”字段显示对应对象；T0 为位置 `(0,0,0)`、旋转 `(0,0,0)`、缩放 `(1,1,1)`。

| 步骤 | Revision | Entity | Position | Undo/Redo | 结果 |
|---|---:|---|---|---:|---|
| Baseline | 0 | 空 | — | 0/0 | 未命名空场景 |
| Create Cube | 1 | `EntityId(1)` | `(0,0,0)` | 1/0 | World、Hierarchy、RenderSnapshot、Inspector 对齐 |
| Set Position | 2 | 同一 ID | `(3.5,-2.25,7)` | 2/0 | Inspector 与 World 回读一致；旋转/缩放不变 |
| Undo Move | 3 | 存在 | `(0,0,0)` | 1/1 | T0 恢复 |
| Redo Move | 4 | 存在 | `(3.5,-2.25,7)` | 2/0 | T1 恢复 |
| Undo Move | 5 | 存在 | `(0,0,0)` | 1/1 | T0 再次恢复 |
| Undo Create | 6 | 不存在 | — | 0/2 | World、Hierarchy、RenderSnapshot 实体集合恢复为空 |

结束前 ScenePath 仍为空、无活动交互。没有执行 Save；随后 `session.close` 正常结束 CLI/Editor，child exit code `0`。操作期间 XYC 未发送鼠标/键盘输入。

## TOP 截图回归

Candidate Editor 设为 Light、1360×820 DIP、DPI 96。四张图均由 `Avalonia.RenderTargetBitmap` 从真实生产 Window 裁切 TOP 生成。窗口 `visible=true`、`foreground=false`，sidecar 记录 `foregroundUnchanged=true`、`zOrderUnchanged=true`、`takesForeground=false`、`changesZOrder=false`。实际客户区为 1360×820 px，TOP 截图为 1330×98 px。

| PNG | SHA-256 | Sidecar SHA-256 |
|---|---|---|
| `Light-1360-p2a-before.png` | `640E2401B7BFAD5AB69B1AF206AFFFC4AD33BCF1E3FFBCDAE4321DDBB5985314` | `FFE186875EDCB098AE434BE1A64D5BA458DA05DB349987D2F98D64D4B43D2B0D` |
| `Light-1360-p2a-created.png` | `B41AFF45447F510C2601D270F2C9A4C0D1B93F379805913B8123C9C2593AB593` | `2DE98C4CEE7B21A7910FBB71D3ED107ED0F867C4241E1A5CE63F726BF4508B5B` |
| `Light-1360-p2a-moved.png` | `B41AFF45447F510C2601D270F2C9A4C0D1B93F379805913B8123C9C2593AB593` | `67D6234493D170609148C77CC7580BDA28F39F04AE333ACEDBFC65349345BFAA` |
| `Light-1360-p2a-restored.png` | `640E2401B7BFAD5AB69B1AF206AFFFC4AD33BCF1E3FFBCDAE4321DDBB5985314` | `F348E2074A721F092EC63EF40F4D7A16ABBB1F4F8A214BD6CA6F14EC49BBB43A` |

P1-B 的 TOP 捕获回归在 Candidate Release Runtime 中通过；相同 TOP 像素符合实体不改变 TOP 布局的预期。本证据不代表 Cube 视口几何像素验证，也未对截图运行 P1-B 四主题/分辨率保真阈值套件。

## Silent Host 回归

Candidate Release `--xyc-silent` 会话运行 `runtime.info`、`scene.inspect`、`camera.inspect` 并关闭。记录 `HIDDEN-HWND-SWAPCHAIN-PRESENT`、HWND 存在但 `visible=false`、非前台、`presentSucceeded=true`、frame counter `1`；关闭日志确认 HWND 已销毁、Vulkan Surface/设备资源释放、child exit code `0`。本轮未请求 Readback，因此 `readbackCaptured=false`。

## 负向结果

真实 Runtime 结构化拒绝：错误 Session=`UNAUTHORIZED`；格式错误 ID=`INVALID_ENTITY_ID`；有效格式但不存在 ID=`ENTITY_NOT_FOUND`；过期 SceneRevision=`STALE_REVISION`；空 History Undo=`INVALID_STAGE`；重复创建=`UNSAFE_SCENE`；非本 Session 实体=`UNAUTHORIZED_ENTITY`；超界位置=`INVALID_INPUT`。Scoped 测试另验证无诊断写权限拒绝与重复 RequestId 拒绝。

详细逐请求/响应、状态对象、Sidecar 与构建哈希保存在 `evidence/candidate-runtime-final/`；Candidate Silent Host 响应在 `evidence/silent-smoke/silent-smoke.json`。


## R2 最终 Candidate Runtime

- Source base：`d0335c52b0c9938277f571cc58d11c4d038f3b43`；最终源文件树身份与 App/CLI/测试程序集 SHA-256 见 `candidate-identity-r2.json`。
- App：`Editor.App.exe=DE13C9CF9C40C876B9F62C49D864F0FD665E77FE73EA5CCC1EE8896E0538B74C`；UI：`XuanYu.Editor.UI.dll=CC291CF8A4CEB6670ADC406B5BDE0388C411A422078066172E5006619339AF1A`。
- 新真实生产 Editor 事务会话完成空场景检查、Create Cube、Transform、Undo/Redo、撤销变换与创建，World/Hierarchy/RenderSnapshot/Inspector/History 全链回读；session 正常关闭，exit code 0。TOP Light/1360 截图由生产 Window 的 Avalonia RenderTargetBitmap 裁切；新截图与之前像素哈希相同（布局未变化），sidecar 对应新 UI 二进制哈希。
- 新无写权限真实会话未传写授权；创建命令 `UNAUTHORIZED`，World/History/Stage 仍为 `0/0/0`，正常关闭。记录见 `evidence/candidate-runtime-r2/no-write-session.json`。
- 新 Silent Host Release Smoke：隐藏 HWND、无前台焦点、Vulkan Present 成功、session 正常关闭，exit code 0。记录见 `evidence/candidate-runtime-r2/silent-smoke.json`。
- 最终 `XuanYu.Editor.UI.dll` 哈希 `33C41F7F5A17C478F041D19C57A8A0DA54309CC9E2F6E7428584F8AB18348C29` 下，P1-B 四组合均用真实 Editor 新会话重采并按批准算法对固定 GDI 参考计算：Light-1360 exact 95.23%, MAE 0.104, PSNR 48.23 dB, >16 0.058%; Dark-1360 94.78%, 0.420, 35.34 dB, 0.817%; Light-1920 96.65%, 0.073, 49.76 dB, 0.041%; Dark-1920 96.33%, 0.295, 36.86 dB, 0.575%。四组分别满足阈值，见 `evidence/p1b-recheck-final/manifest.json`。

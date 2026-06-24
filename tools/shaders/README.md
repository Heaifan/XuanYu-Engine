# XuanYu Engine Shader 编译链

## 为什么禁止手写 SPIR-V

Milestone 8.1（c30de44）使用了 `tools/gen_spirv` 自编码 SPIR-V 生成器。
事后分析发现该生成器的所有结果型指令均存在 **Result Type 和 Result <id> 操作数顺序错误**，
导致生成的 `.spv` 文件语义非法。

事故链：

```text
手写 SPIR-V 编码错误
  → vkCreateShaderModule 表面通过
  → vkCreateGraphicsPipelines 触发 driver shader 编译
  → SPIR-V 语义错误 → 驱动级崩溃
  → Editor 进程闪退
```

结论：**禁止手写 SPIR-V。所有 .spv 必须由标准工具编译并验证。**

## 当前工具链

| 步骤 | 工具 | 脚本 |
|------|------|------|
| 编译 GLSL → SPIR-V | `glslangValidator` | `compile_basic_3d.ps1` |
| 验证 SPIR-V 合法性 | `spirv-val` | `validate_basic_3d.ps1` |

### 编译

```powershell
powershell -ExecutionPolicy Bypass -File tools/shaders/compile_basic_3d.ps1
```

要求：本机安装 `glslangValidator`（Vulkan SDK 或独立安装）。

找不到工具时脚本输出中文错误提示并返回非 0 退出码。
不会生成伪造 `.spv`。

### 验证

```powershell
powershell -ExecutionPolicy Bypass -File tools/shaders/validate_basic_3d.ps1
```

要求：本机安装 `spirv-val`（Vulkan SDK 或 SPIRV-Tools），
且 `.spv` 文件已存在（先执行编译脚本）。

找不到工具或 `.spv` 缺失时输出中文错误提示并返回非 0 退出码。
不会假装通过验证。

## 安装 Vulkan SDK

https://vulkan.lunarg.com/sdk/home

安装后，`glslangValidator` 和 `spirv-val` 会自动加入 PATH。

## 废弃工具

`tools/gen_spirv` 是 Milestone 8.1 的临时手写 SPIR-V 编码器，**已废弃**。
不再参与构建和运行。后续将在 8.R.3/8.R.4 中正式删除。

## Scene3D 状态

当前 Scene3D（3D 管线渲染）保持隔离状态。
即使 `.spv` 文件存在且验证通过，Scene3D 也不会自动随 Editor 启动。

重新启用的条件（预计 8.R.3）：

1. `spirv-val` 通过 `.spv` 合法性验证。
2. `VulkanScene3dRunGate.Evaluate()` 根据验证结果返回 Ready。
3. Editor 不自动运行 Scene3D，仅在用户明确操作后触发。

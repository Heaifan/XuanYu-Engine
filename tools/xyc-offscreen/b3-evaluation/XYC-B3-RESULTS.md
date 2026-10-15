# XYC-B3 · RTX 3060 地形 LOD 取证

基线 `9429be62288cd58c56ff7a9b6608857afb4c6175`，源数据 NASADEM `n24e121.hgt`（SHA-256 `BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1`）。测试条件为 2049×2049 render heightfield、240-cell chunks、1024×768 离屏 Vulkan、固定 near-oblique 世界相机。每配置 5 个独立进程、每进程 3 warm-up + 30 GPU timestamp 帧；6 档轮换顺序，逐进程保存 `nvidia-smi` 前后状态、完整 manifest、PNG 和探针日志。

## 近地斜视结果

表中 GPU P50/P95 是五个独立进程统计值的中位数；GPU 帧是进程内样本，不能算作独立重复。最终网格误差是在相同 NASADEM 样本上，按第一重复实际选中的可见 chunk 与 LOD 混合网格重新算得，覆盖视口内可见 chunk footprint，不代表整张地形。

| 配置 | GPU P50 / P95 (ms) | 三角形 | 相对自动 LOD 的 GPU P50 | 可见 footprint 高程误差 P95 / Max (m) | PNG RGB MAE / Mask XOR |
|---|---:|---:|---:|---:|---:|
| 2049 Auto LOD | 22.565 / 23.459 | 1,910,496 | 基线 | 8.08 / 134.80 | 0 / 0% |
| LOD +1 | 5.568 / 6.269 | 486,256 | −75.3% | 14.38 / 146.33 | 1.260 / 0.518% |
| SSE 0.5 px | 20.576 / 21.539 | 1,746,654 | −8.8% | 7.84 / 134.80 | 0 / 0% |
| SSE 1 px | 20.349 / 21.353 | 1,746,654 | −9.8% | 7.84 / 134.80 | 0 / 0% |
| SSE 2 px | 17.960 / 19.091 | 1,500,030 | −20.4% | 8.29 / 134.80 | 0 / 0% |
| SSE 4 px | 12.907 / 13.872 | 1,092,062 | −42.8% | 11.96 / 134.80 | 0 / 0% |

SSE 0.5/1/2/4 px 相对 Auto LOD 分别减少 8.6%、8.6%、21.5%、42.8% 三角形。SSE 4 px 在本近地姿态取得显著成本下降；它的 pooled 可见地形 footprint P95 上升 48.0%，但最大误差不变，且最终栅格 PNG 与 Auto LOD 完全一致。LOD +1 更快，然而本姿态的网格 P95/最大误差上升，图像差异为 RGB MAE 1.260、掩码异或 0.518%。

## 连续视角检查

Parent 运行了 baseline 与 SSE 4 px 的真实 Vulkan 固定轨迹，各 8 帧：俯视、near-oblique、高起伏敏感、chunk-entry、近裁剪面压力、A→B→A。所有相机字段、HGT/P​​NG hash、图像尺寸验证通过，A→B→A 返回相机相同。SSE 4 px 与 baseline 的八张配对 PNG 中七张完全一致；sequence-B 有 0.064% 像素的 RGB 通道最大差超过 8，平均 RGB MAE 为 0.019。GPU 时间和每帧 chunk/LOD/三角形计数见 raw `trajectory-sse-4/runtime.json`。

## 误差口径与结论

- 全地形 Source→Render 双线性高度场重建 P95 为 10.370 m；它与 LOD 网格误差分开统计。原始误差表含 81 chunks × 5 LOD = 405 项，每项保存有效样本数、P95、最大视觉高程误差，skirt 不计入表面。
- SSE 把每个 chunk 的离线 NASADEM 残差通过当前相机投影估算成像素，并以 0.5/1/2/4 px 作为选档预算。投影量是诊断估算，不是误差保证；相机位于 chunk 内或与 near plane 相交时会退回安全的细 LOD。
- 2049 网格自身仍有 10.370 m 的 Source→Render P95；表格中的 final mesh P95 是该固定姿态可见 chunk footprint 的真实源样本统计，不是全地形/所有相机的总体误差。GPU image equality 也不等于主观视觉验收。
- **决定：保留生产默认 Auto LOD。** SSE 4 px 是有潜力的诊断候选：在当前 near-oblique 视角以约 43% 更少三角形换取约 43% 更低 GPU P50，并通过固定轨迹像素对照。因为屏幕误差仍是估算、只测了一个地形/视口/RTX 3060，尚不足以作为默认算法；先扩大地形与连续视角验证。

## 可复现入口与结果

- 图表和静态结果见 [B3 结果图](results/20261015-202400/gpu-cost.svg)、[性能-网格误差 Pareto](results/20261015-202400/pareto-final-mesh.svg)、[可见 chunk 误差](results/20261015-202400/gpu-vs-visible-chunk-error.svg) 和 [误差层分解](results/20261015-202400/error-layers.svg)。
- 真实 Vulkan 样图：[SSE 4 px near-oblique](results/20261015-202400/sse4-near-oblique.png)、[LOD+1 图像差异](results/20261015-202400/lod-bias-1-near-oblique-diff.png)、[SSE 4 px sequence-B 差异](results/20261015-202400/sse4-sequence-B-diff.png)。
- 可机读证据：[进程统计 CSV](results/20261015-202400/processes.csv)、[配置汇总 CSV](results/20261015-202400/summaries.csv)、[成对波动比值](results/20261015-202400/paired-ratios.csv)、[405 项 chunk/LOD 误差表](results/20261015-202400/error-table.json)、[GPU 状态快照](results/20261015-202400/gpu-process-environment.jsonl)、[轨迹差异数据](results/20261015-202400/trajectory-differences.json)。
- GPU 轮换矩阵：`tools/xyc-offscreen/Invoke-XYC-B3.ps1`
- 误差表生成：`XuanYu.OffscreenTerrain.dll --b3-errors --source <hgt> --out <new.json> --samples 2049 --chunk 240`
- 真实 GPU 轨迹：`tools/xyc-offscreen/Invoke-XYC-B3-Visual.ps1 -RunDirectory <run> -Variant sse-4`
- 汇总 JSON/CSV/SVG 与 30 张像素差异图、16 张轨迹截图：本轮 `artifacts/diagnostics/xyc-b3/20261015-202400/`。
- 离线处理器：`tools/xyc-offscreen/b3-evaluation/import_b3_runtime.py`、`analyze_b3.py`；VIS 对照器：`tools/xyc-offscreen/b3-visual/compare_trajectories.py`。

此项仅增加诊断/验证能力，生产默认 LOD、版本号和产品行为未改变。真实交互窗口与主观产品画质仍待用户 P4 验收。

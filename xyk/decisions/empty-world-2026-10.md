# DEC-XYE-EMPTY-WORLD-20261003｜空世界与ReferencePlane

**ACTIVE（用户架构裁定）｜2026-10-03。**

Empty World包含合法World和Scene，即使Terrain、Region、Road、Marker、Unit、Imported Map全为零，也应显示Sky与Editor World Grid。ReferencePlane保留逻辑Picking/Ground Query含义，但不再提交可见Surface；Grid属于独立Editor World可视参考层。Map与Terrain不得决定ReferencePlane的生死；DEM导入/删除需保持空世界与其Camera/Grid权威一致。不得通过Horizon/Patch或ReferencePlane DrawKind继续堆渲染取巧来掩盖权威混乱。

必须另行验证Cold First Frame、1m～10km、DEM Import/Delete/Restore及用户P4；10/03当时P4-B仍需人工确认，不能记作最终CLOSED。

**冲突ID裁定**：10/03历史建议新建K-REN-006表达这条规则，但2026-10-31正式K-REN-006已用于多帧生命周期。本次把逻辑平面知识并入既有K-REN-004，不重用ID、不覆盖Renderer双帧知识。

# Controls directory layout

控件按组件族和 canonical 编号分目录，不允许将新增组件直接平铺在本目录。

```text
Controls/
├─ XYUI1/XYUI1-XX-ComponentName/
├─ XYUI2/XYUI2-XX-ComponentName/
├─ XYUI1/_Shared/
└─ XYUI2/_Shared/
```

组件自身的主文件、partial 文件和模板文件放入对应组件目录；XYUI-1 内部共用的基类、样式和几何辅助构件放入 `XYUI1/_Shared`；XYUI-2 内部共用的按钮族、样式和 Token 放入 `XYUI2/_Shared`。只有真正跨 XYUI 族共用的构件才允许新增根级 `Shared`。新增控件必须沿用 `XYUI<n>/XYUI<n>-XX-ComponentName` 命名规则。

# Controls directory layout

控件按组件族、编号区间和完整 canonical 编号分目录，不允许将新增组件直接平铺在本目录。

```text
Controls/
├─ XYUI1/01-05/XYUI1-01-ComponentName/
├─ XYUI1/06-10/XYUI1-06-ComponentName/
├─ XYUI1/11-15/XYUI1-11-ComponentName/
├─ XYUI1/16-20/XYUI1-16-ComponentName/
├─ XYUI1/21-24/XYUI1-21-ComponentName/
├─ XYUI2/XYUI2-XX-ComponentName/
└─ XYUI2/_Shared/
```

组件自身的主文件、partial 文件和模板文件放入对应组件目录；XYUI1 跨组件基础控件与几何构件位于 `Theme/Controls/XYUI1`，组件族样式实现位于 `Theme/Styles/{Foundation,XYUI1,XYUI3,XYUI4}`，XYUI3 样式再分为 `Core` 和 `Components`，并保留 `XyuiComponentStyles.Create()` 作为统一注册入口。XYUI2 内部共用的按钮族、样式和 Token 继续放入 `XYUI2/_Shared`。新增控件必须保留 `XYUI<n>-XX-ComponentName` 完整索引。

# XYUI-0.11 · Responsive Runtime Contract

## Definition

`AdaptiveLayout` is a container-first layout primitive. It observes its own arranged width and reflows its direct children into one to `MaxColumns` columns.

## Public API

```xml
<xy:AdaptiveLayout MinItemWidth="280" MaxColumns="3" xy:XY.Gap="XY.Space.2" />
```

- `MinItemWidth`: minimum usable width of one layout item; it is not a breakpoint.
- `MaxColumns`: maximum number of columns; values are clamped to at least one at runtime.
- `XY.Gap`: existing canonical spacing facade; it supplies the gap between columns and rows.

## Resolver and source of truth

The layout observes its own available container width. The column count is:

`Clamp(1..MaxColumns, floor((availableWidth + gap) / (MinItemWidth + gap)))`.

There is no window-width, screen-width, device-type, or global breakpoint branch.

## Reflow and stability

Children retain their order and remain present. The primitive changes only column arrangement. It does not hide primary content, change child size, change typography, or modify `XY.Size` / `XY.Density`. It performs no event subscription to `BoundsChanged` or `SizeChanged`; measurement and arrangement are the sole layout signals.

## Examples

Use `XY.Gap` for spacing and compose `XY.Size` or `XY.Density` independently. The Gallery may use the same `AdaptiveLayout` with `Grid`, `Border`, and XYUI controls as direct children.

## Tests and limitations

The 0-11 targeted tests cover container-width column selection, one/max-column clamping, gap consumption, child preservation, and Size/Density orthogonality. This MVP does not provide named variants, priorities, visibility collapse, device branches, public breakpoints, or public hysteresis controls.

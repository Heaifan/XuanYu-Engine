namespace XuanYu.Core.Gizmo;

// 单轴缩放手柄：修改实体自身 TRS 的局部 X / Y / Z 分量。
public enum ScaleGizmoAxis
{
    X,
    Y,
    Z
}

// Scale Gizmo 命中结果：三根轴向手柄之一，或中心等比手柄。
public enum ScaleGizmoHandle
{
    X,
    Y,
    Z,
    Uniform
}

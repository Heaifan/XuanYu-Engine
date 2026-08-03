namespace XuanYu.Editor.MapDocument;

// MAP-A-R2-D1：区域类型。R2 仅承载几何与基础元数据，不解释战斗含义。
// Generic=普通区域、Playable=可活动范围、Restricted=禁区、
// Deployment=部署区、Objective=目标区。未来业务可扩展。
public enum MapRegionKind
{
    Generic = 0,
    Playable = 1,
    Restricted = 2,
    Deployment = 3,
    Objective = 4
}

#version 450

// F3-F4：正交标准视图的视图平面网格（YZ/XZ 平面，以世界原点为基准）。
// 只在 ±X（YZ 平面）/±Y（XZ 平面）正交标准视图显示；±Z 复用现有地面网格（Z=0 即 XY 平面）。
// 顶点复用 editor_reference_grid.vert；本片元按 uniform 平面法线与视线求交，
// 正交投影下视线平行于平面法线（无掠射退化）；间距/LOD 由 CPU 每帧计算（同参考网格）。

layout(push_constant) uniform ViewPlaneGridPush {
    mat4 viewProjection;        // 0   世界→裁剪（深度投影用）
    mat4 inverseViewProjection; // 64  裁剪→世界（射线重建）
    vec4 cameraPosition;        // 128 相机世界位置
    vec4 viewportAndFar;        // 144 x,y=视口尺寸; z=Far; w=GridMaxDistance
    vec4 gridScale;             // 160 x=FineSpacing; y=CoarseSpacing; z=FineWeight; w=CoarseWeight
    vec4 planeNormal;           // 176 平面法线（轴向：YZ=±X / XZ=±Y）
} pc;

layout(location = 0) in vec4 vFarWorld;
layout(location = 1) in vec4 vNearWorld;
layout(location = 0) out vec4 outColor;

const float GRID_LINE_WIDTH_PX = 0.82;

const float DEPTH_BIAS_FACTOR = 0.5;
const float MIN_DEPTH_BIAS = 0.0000001;
const float MAX_DEPTH_BIAS = 0.00002;

float axisLineMask(float coordinate, float widthPixels) {
    float derivative = max(fwidth(coordinate), 0.000001);
    float distanceToLine = abs(fract(coordinate - 0.5) - 0.5) / derivative;
    return 1.0 - smoothstep(widthPixels - 0.5, widthPixels + 0.5, distanceToLine);
}

float densityFade(float coordinate, float spacing) {
    float cellPixels = 1.0 / max(fwidth(coordinate / spacing), 0.000001);
    return smoothstep(6.0, 12.0, cellPixels);
}

// 平面内两轴：YZ 平面（法线 ±X）→ (y, z)；XZ 平面（法线 ±Y）→ (x, z)；XY 平面 → (x, y)。
vec2 inPlane(vec3 p) {
    vec3 n = abs(pc.planeNormal.xyz);
    if (n.x > 0.5) return vec2(p.y, p.z);
    if (n.y > 0.5) return vec2(p.x, p.z);
    return vec2(p.x, p.y);
}

void main() {
    vec3 nearWorld = vNearWorld.xyz / vNearWorld.w;
    vec3 farWorld = vFarWorld.xyz / vFarWorld.w;
    vec3 rayDirection = farWorld - nearWorld;

    // 平面求交：n·(near + t·dir) = 0 → t = -(n·near)/(n·dir)。
    float denom = dot(pc.planeNormal.xyz, rayDirection);
    if (abs(denom) < 0.001) discard;
    float t = -dot(pc.planeNormal.xyz, nearWorld) / denom;
    if (t <= 0.0) discard;
    vec3 worldPosition = nearWorld + rayDirection * t;
    if (t > pc.viewportAndFar.w) discard;

    // 真实深度（Vulkan 0~1）+ 有界深度偏移（共面稳定）。
    vec4 clipPosition = pc.viewProjection * vec4(worldPosition, 1.0);
    float depth = clipPosition.z / clipPosition.w;
    if (!(depth >= 0.0 && depth <= 1.0)) discard;
    float bias = clamp(fwidth(depth) * DEPTH_BIAS_FACTOR, MIN_DEPTH_BIAS, MAX_DEPTH_BIAS);
    gl_FragDepth = depth - bias;

    // 距离淡出：0~45% far 完整，45~75% 平滑，>75% 隐藏。
    float distToCamera = length(worldPosition - pc.cameraPosition.xyz);
    float distanceFade = 1.0 - smoothstep(pc.viewportAndFar.z * 0.45,
        pc.viewportAndFar.z * 0.75, distToCamera);

    // 全局层级（CPU 每帧一次，禁止逐 Fragment LOD）。
    float fineSpacing = pc.gridScale.x;
    float coarseSpacing = pc.gridScale.y;
    float fineWeight = pc.gridScale.z;
    float coarseWeight = pc.gridScale.w;

    // 分方向抗摩尔纹：平面内两轴分别用单元密度。
    vec2 planeCoord = inPlane(worldPosition);
    float fadeA = densityFade(planeCoord.x, fineSpacing);
    float fadeB = densityFade(planeCoord.y, fineSpacing);

    float fineLine = axisLineMask(planeCoord.x / fineSpacing, GRID_LINE_WIDTH_PX) * fadeA
                   + axisLineMask(planeCoord.y / fineSpacing, GRID_LINE_WIDTH_PX) * fadeB;
    float coarseLine = axisLineMask(planeCoord.x / coarseSpacing, GRID_LINE_WIDTH_PX) * fadeA
                     + axisLineMask(planeCoord.y / coarseSpacing, GRID_LINE_WIDTH_PX) * fadeB;

    // 非累加合成（同参考网格）：重合处取 max，禁止相加。
    float fineContribution = fineLine * 0.16 * fineWeight;
    float coarseContribution = coarseLine * 0.24 * coarseWeight;
    float gridAlpha = max(fineContribution, coarseContribution);

    // 配色同参考网格（玄域浅色体系）：Fine #5D6670、Coarse #525C67。
    vec3 fineColor = vec3(0.365, 0.400, 0.439);
    vec3 coarseColor = vec3(0.322, 0.361, 0.404);
    float total = fineContribution + coarseContribution;
    vec3 gridColor = total > 0.000001
        ? (fineColor * fineContribution + coarseColor * coarseContribution) / total
        : fineColor;

    float alpha = gridAlpha * distanceFade;
    if (alpha < 0.005) discard;
    outColor = vec4(gridColor, alpha);
}

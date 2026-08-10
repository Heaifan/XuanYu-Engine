#version 450

// MAP-A-R1-D5-R1-F2-R3：Blender 式统一尺度参考网格 —— 片元着色器。
// 本 Pass 只画普通网格（不画 X/Y 轴、不画原点标记——它们由独立 WorldAxes/WorldOrigin Pass 负责）。
// 每个 Fragment 根据局部 world-per-pixel 选择十进制层级，CPU 不再决定整屏 Fine/Coarse；
// 唯一像素线宽（GridLineWidthPixels，Fine 与 Coarse 完全相同）；互补交叉淡化（FineWeight + CoarseWeight = 1）；
// 重合处使用非累加合成（max，禁止 fine+coarse 直接相加 → 无双重 Alpha、无粗黑线）；
// 分方向投影密度淡出抗摩尔纹；距离淡出 + 掠射角淡出；有界深度偏移。

layout(push_constant) uniform GridPush {
    mat4 viewProjection;        // 0   世界→裁剪（深度投影用）
    mat4 inverseViewProjection; // 64  裁剪→世界（射线重建）
    vec4 cameraPosition;        // 128 相机世界位置
    vec4 viewportAndFar;        // 144 x,y=视口尺寸; z=Far; w=GridMaxDistance
    vec4 gridScale;             // 160 x=FineSpacing; y=CoarseSpacing; z=FineWeight; w=CoarseWeight
    vec4 mapBounds;             // x/y/w 保留旧布局；z=BaseHeight
} pc;

layout(location = 0) in vec4 vFarWorld;
layout(location = 1) in vec4 vNearWorld;
layout(location = 0) out vec4 outColor;

// F2-R3：唯一像素线宽（Fine == Coarse，硬合同；范围 0.78~0.90，不得超过 1.0）。
const float GRID_LINE_WIDTH_PX = 0.82;

// 深度偏移（方案 12.2）：clamp(fwidth(depth)×0.5, 1e-7, 2e-5)，仅解决共面闪烁。
const float DEPTH_BIAS_FACTOR = 0.5;
const float MIN_DEPTH_BIAS = 0.0000001;
const float MAX_DEPTH_BIAS = 0.00002;

// 分方向线掩码：coordinate 为 worldXY/step 的对应轴分量。
// 标准像素距离模型：1 - smoothstep(w-0.5, w+0.5, d)；不得把 alpha 乘到宽度。
float axisLineMask(float coordinate, float widthPixels) {
    float derivative = max(fwidth(coordinate), 0.000001);
    float distanceToLine = abs(fract(coordinate - 0.5) - 0.5) / derivative;
    return 1.0 - smoothstep(widthPixels - 0.5, widthPixels + 0.5, distanceToLine);
}

// Anti-Moiré band-pass：过密和过疏的层级都退出，只保留可辨识窗口。
float projectedCellPixels(float coordinate, float spacing) {
    return spacing / max(fwidth(coordinate), 0.000001);
}

float bandPass(float cellPixels) {
    float fadeIn = smoothstep(10.0, 18.0, cellPixels);
    float fadeOut = 1.0 - smoothstep(80.0, 140.0, cellPixels);
    return fadeIn * fadeOut;
}

float levelLine(vec2 position, float spacing) {
    float xPixels = projectedCellPixels(position.x, spacing);
    float yPixels = projectedCellPixels(position.y, spacing);
    float xLine = axisLineMask(position.x / spacing, GRID_LINE_WIDTH_PX) * bandPass(xPixels);
    float yLine = axisLineMask(position.y / spacing, GRID_LINE_WIDTH_PX) * bandPass(yPixels);
    return max(xLine, yLine);
}

float log10Value(float value) {
    return log(value) * 0.434294482;
}

float levelAlpha(float spacing) {
    return 0.14 + 0.05 * clamp(log10Value(spacing) - 2.0, 0.0, 2.0);
}

vec3 levelColor(float spacing) {
    const vec3 fineColor = vec3(0.365, 0.400, 0.439);
    const vec3 coarseColor = vec3(0.322, 0.361, 0.404);
    float t = clamp((log10Value(spacing) - 2.0) * 0.5, 0.0, 1.0);
    return mix(fineColor, coarseColor, t);
}

void main() {
    vec3 nearWorld = vNearWorld.xyz / vNearWorld.w;
    vec3 farWorld = vFarWorld.xyz / vFarWorld.w;
    vec3 rayDirection = farWorld - nearWorld;

    // 保护 1：近似平行于地图平面（Z=BaseHeight；无地图时 mapBounds.z=0 = 原 Z=0 平面）。
    if (abs(rayDirection.z) < 0.001) discard;
    // 保护 2：交点在相机后方。
    float t = (pc.mapBounds.z - nearWorld.z) / rayDirection.z;
    if (t <= 0.0) discard;
    // 保护 3：超出最大距离。
    vec3 worldPosition = nearWorld + rayDirection * t;
    if (t > pc.viewportAndFar.w) discard;

    // D3：地图存在时按矩形边缘淡出（地图内完整、边缘外 w 宽度内衰减、更远隐藏）；
    // 无地图（w=0）时保持无限参考网格。
    // 真实深度（Vulkan 0~1）+ 有界深度偏移（往相机方向，共面稳定）。
    vec4 clipPosition = pc.viewProjection * vec4(worldPosition, 1.0);
    float depth = clipPosition.z / clipPosition.w;
    if (!(depth >= 0.0 && depth <= 1.0)) discard;
    float bias = clamp(fwidth(depth) * DEPTH_BIAS_FACTOR, MIN_DEPTH_BIAS, MAX_DEPTH_BIAS);
    gl_FragDepth = depth - bias;

    // 距离淡出：0~45% far 完整，45~75% 平滑，>75% 隐藏。
    float distToCamera = length(worldPosition - pc.cameraPosition.xyz);
    float distanceFade = 1.0 - smoothstep(pc.viewportAndFar.z * 0.45,
        pc.viewportAndFar.z * 0.75, distToCamera);

    // Anti-Moiré 保护区：|V·Z|<0.04 隐藏，0.04~0.12 淡入，>0.12 完整。
    vec3 viewDirection = normalize(pc.cameraPosition.xyz - worldPosition);
    float grazingFactor = abs(dot(vec3(0.0, 0.0, 1.0), viewDirection));
    float grazingFade = smoothstep(0.040, 0.120, grazingFactor);

    float worldPerPixelX = max(fwidth(worldPosition.x), 0.000001);
    float worldPerPixelY = max(fwidth(worldPosition.y), 0.000001);
    float worldPerPixel = sqrt(worldPerPixelX * worldPerPixelY);
    float idealSpacing = max(worldPerPixel * 48.0, 100.0);
    float centerSpacing = max(pow(10.0, floor(log10Value(idealSpacing))), 100.0);
    float lowerSpacing = max(centerSpacing * 0.1, 100.0);
    float upperSpacing = min(centerSpacing * 10.0, 10000000.0);
    float lowerContribution = levelLine(worldPosition.xy, lowerSpacing) * levelAlpha(lowerSpacing);
    float centerContribution = levelLine(worldPosition.xy, centerSpacing) * levelAlpha(centerSpacing);
    float upperContribution = levelLine(worldPosition.xy, upperSpacing) * levelAlpha(upperSpacing);
    float gridAlpha = max(max(lowerContribution, centerContribution), upperContribution);
    vec3 gridColor = levelColor(centerSpacing);
    if (lowerContribution >= centerContribution && lowerContribution >= upperContribution)
        gridColor = levelColor(lowerSpacing);
    else if (upperContribution >= centerContribution)
        gridColor = levelColor(upperSpacing);

    float alpha = gridAlpha * distanceFade * grazingFade;
    if (alpha < 0.005) discard;
    outColor = vec4(gridColor, alpha);
}

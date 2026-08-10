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

// 方向性密度淡出：某方向单元屏幕间距 <6px 隐藏、6~12px 渐入、>12px 正常。
// 只影响透明度，不影响线宽。
float densityFade(float coordinate, float spacing) {
    float cellPixels = 1.0 / max(fwidth(coordinate / spacing), 0.000001);
    return smoothstep(6.0, 12.0, cellPixels);
}

float levelLine(vec2 position, float spacing) {
    float xLine = axisLineMask(position.x / spacing, GRID_LINE_WIDTH_PX)
                * densityFade(position.x, spacing);
    float yLine = axisLineMask(position.y / spacing, GRID_LINE_WIDTH_PX)
                * densityFade(position.y, spacing);
    return max(xLine, yLine);
}

float log10Value(float value) {
    return log(value) * 0.434294482;
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

    // 掠射角淡出：V=相机方向；|V·Z|<0.015 隐藏，0.015~0.080 淡入，>0.080 完整。
    vec3 viewDirection = normalize(pc.cameraPosition.xyz - worldPosition);
    float grazingFactor = abs(dot(vec3(0.0, 0.0, 1.0), viewDirection));
    float grazingFade = smoothstep(0.015, 0.080, grazingFactor);

    float worldPerPixel = max(max(fwidth(worldPosition.x), fwidth(worldPosition.y)), 0.000001);
    float idealSpacing = max(worldPerPixel * 48.0, 100.0);
    float decade = pow(10.0, floor(log10Value(idealSpacing)));
    float lowerSpacing = max(decade, 100.0);
    float upperSpacing = min(lowerSpacing * 10.0, 10000000.0);
    float decadePhase = smoothstep(0.0, 1.0,
        (log10Value(idealSpacing) - log10Value(lowerSpacing))
        / max(log10Value(upperSpacing) - log10Value(lowerSpacing), 0.000001));
    float lowerContribution = levelLine(worldPosition.xy, lowerSpacing) * 0.16 * (1.0 - decadePhase);
    float upperContribution = levelLine(worldPosition.xy, upperSpacing) * 0.24 * decadePhase;
    float gridAlpha = max(lowerContribution, upperContribution);

    // F2-R3 配色（玄域浅色体系，克制灰蓝）：Fine #5D6670、Coarse #525C67。
    vec3 fineColor = vec3(0.365, 0.400, 0.439); // #5D6670
    vec3 coarseColor = vec3(0.322, 0.361, 0.404); // #525C67
    float total = lowerContribution + upperContribution;
    vec3 gridColor = total > 0.000001
        ? (fineColor * lowerContribution + coarseColor * upperContribution) / total
        : fineColor;

    float alpha = gridAlpha * distanceFade * grazingFade;
    if (alpha < 0.005) discard;
    outColor = vec4(gridColor, alpha);
}

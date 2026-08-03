#version 450

// MAP-A-R1-D5-R1-F2：Blender 风格无限自适应参考网格 —— 片元着色器（F2 修复版）。
// 世界射线与 Z=0 平面求交；worldMetersPerPixel×36 目标选相邻十进制层级；
// fwidth 像素恒定抗锯齿线宽（细 0.75px / 主 1.10px / 轴 1.35px）；
// 细格/主格加深叠加权重连续，跨级同组线不突变；距离淡出 + 掠射角淡出；
// 深度输出带像素级偏移，与平坦地形共面稳定；不按地图矩形裁剪（无限参考平面）。
//
// 配色（玄域浅色编辑器，克制蓝灰体系，禁高饱和/荧光/红绿工程轴）：
//   细网格 #566A82  α 0.20   主网格 #344A63  基础 0.20 + 加深 0.18
//   X 主轴 #AD8550  α 0.62   Y 主轴 #557C9E  α 0.62   原点 #D1AE69 α 0.70

layout(push_constant) uniform GridPush {
    mat4 viewProjection;        // 0   世界→裁剪（深度投影用）
    mat4 inverseViewProjection; // 64  裁剪→世界（射线重建）
    vec4 cameraPosition;        // 128 相机世界位置
    vec4 viewportAndFar;        // 144 x,y=视口尺寸; z=Far; w=GridMaxDistance
} pc;

layout(location = 0) in vec4 vFarWorld;
layout(location = 1) in vec4 vNearWorld;
layout(location = 0) out vec4 outColor;

// 合法层级（十进制）。
const float MIN_STEP = 0.1;
const float MAX_STEP = 10000.0;

// 像素恒定抗锯齿线（方案 4.6）：worldXY/step 的 fract+fwidth，返回线强度 0..1。
// 线宽以像素为单位（widthPixels），与相机距离、视角、分辨率无关。
float GridLineMask(vec2 position, float spacing, float widthPixels) {
    vec2 coordinate = position / spacing;
    vec2 derivative = max(fwidth(coordinate), vec2(0.000001));
    vec2 distanceToLine = abs(fract(coordinate - 0.5) - 0.5) / derivative;
    float nearestLine = min(distanceToLine.x, distanceToLine.y);
    return 1.0 - smoothstep(widthPixels - 0.5, widthPixels + 0.5, nearestLine);
}

void main() {
    vec3 nearWorld = vNearWorld.xyz / vNearWorld.w;
    vec3 farWorld = vFarWorld.xyz / vFarWorld.w;
    vec3 rayDirection = farWorld - nearWorld;

    // 保护 1：近似平行于 Z=0 平面。
    if (abs(rayDirection.z) < 0.001) discard;
    // 保护 2：交点在相机后方。
    float t = -nearWorld.z / rayDirection.z;
    if (t <= 0.0) discard;
    // 保护 3：超出最大距离。
    vec3 worldPosition = nearWorld + rayDirection * t;
    if (t > pc.viewportAndFar.w) discard;

    // 真实深度（Vulkan 0~1），带像素级偏移：与平坦地形（同为 Z=0）共面时
    // 网格略向相机偏移约 1.5 像素深度，稳定显示且不穿透实体/凸起地形。
    vec4 clipPosition = pc.viewProjection * vec4(worldPosition, 1.0);
    float depth = clipPosition.z / clipPosition.w;
    if (!(depth >= 0.0 && depth <= 1.0)) discard;
    gl_FragDepth = depth - max(fwidth(depth) * 1.5, 0.0000001);

    // 距离淡出：0~45% far 完整，45~75% 平滑，>75% 隐藏。
    float distToCamera = length(worldPosition - pc.cameraPosition.xyz);
    float distanceFade = 1.0 - smoothstep(pc.viewportAndFar.z * 0.45,
        pc.viewportAndFar.z * 0.75, distToCamera);

    // 掠射角淡出：V=相机方向；|V·Z|<0.015 隐藏，0.015~0.080 淡入，>0.080 完整。
    vec3 viewDirection = normalize(pc.cameraPosition.xyz - worldPosition);
    float grazingFactor = abs(dot(vec3(0.0, 0.0, 1.0), viewDirection));
    float grazingFade = smoothstep(0.015, 0.080, grazingFactor);

    // 自适应层级：worldMetersPerPixel×36（目标 36px/格，正常区间 24~48px）。
    float worldMetersPerPixel = max(length(dFdx(worldPosition.xy)), length(dFdy(worldPosition.xy)));
    float desiredStep = clamp(worldMetersPerPixel * 36.0, MIN_STEP, MAX_STEP);
    float logStep = log(desiredStep) / log(10.0);
    float lowerExp = floor(logStep);
    float lowerStep = clamp(pow(10.0, lowerExp), MIN_STEP, MAX_STEP);
    float upperStep = clamp(lowerStep * 10.0, MIN_STEP, MAX_STEP);
    float phase = fract(logStep);

    // 细格权重 1→0（phase 0.5~1.0 淡出），主格权重 0→1（phase 0.0~0.5 淡入）。
    // 主格线位置是细格线的子集：细格基础 α0.20 + 主格加深 α0.18；
    // 跨级时同组线从"主格（0.18）"平滑过渡为"细格（0.20）"，透明度连续。
    float fineWeight = 1.0 - smoothstep(0.5, 1.0, phase);
    float majorWeight = smoothstep(0.0, 0.5, phase);

    float minorLine = GridLineMask(worldPosition.xy, lowerStep, 0.75);
    float majorLine = GridLineMask(worldPosition.xy, upperStep, 1.10);
    float gridAlpha = minorLine * 0.20 * fineWeight + majorLine * 0.18 * majorWeight;

    // X/Y 主轴（屏幕恒定 1.35px），与层级无关，缩放时不会消失。
    // X 轴 = 世界 Y=0 线（金色 #AD8550）；Y 轴 = 世界 X=0 线（蓝色 #557C9E）。
    float xAxisPx = abs(worldPosition.y) / max(worldMetersPerPixel, 0.000001);
    float yAxisPx = abs(worldPosition.x) / max(worldMetersPerPixel, 0.000001);
    float xAxis = 1.0 - smoothstep(1.35 - 0.5, 1.35 + 0.5, xAxisPx);
    float yAxis = 1.0 - smoothstep(1.35 - 0.5, 1.35 + 0.5, yAxisPx);

    // 原点标记（屏幕恒定约 6px 范围）。
    float originRange = max(fwidth(worldPosition.x), fwidth(worldPosition.y)) * 3.0;
    float originMark = 1.0 - smoothstep(0.0, originRange,
        max(abs(worldPosition.x), abs(worldPosition.y)));

    vec3 minorColor = vec3(0.337, 0.408, 0.510); // #566A82
    vec3 majorColor = vec3(0.204, 0.290, 0.388); // #344A63
    vec3 axisXColor = vec3(0.678, 0.522, 0.314); // #AD8550
    vec3 axisYColor = vec3(0.333, 0.486, 0.620); // #557C9E
    vec3 originColor = vec3(0.820, 0.682, 0.412); // #D1AE69

    vec3 color = mix(minorColor, majorColor, majorWeight);
    float alpha = gridAlpha * distanceFade * grazingFade;

    // 主轴与原点优先于网格线（层级顺序：细格 → 主格 → 坐标轴）。
    if (xAxis > 0.5 || yAxis > 0.5 || originMark > 0.5) {
        float axisAlpha = max(max(xAxis, yAxis), originMark) * distanceFade * grazingFade
            * (originMark > 0.5 ? 0.70 : 0.62);
        color = originMark > 0.5 ? originColor : (xAxis > yAxis ? axisXColor : axisYColor);
        alpha = max(alpha, axisAlpha);
    }

    if (alpha < 0.005) discard;
    outColor = vec4(color, alpha);
}

#version 450

// MAP-A-R1-D5-R1-F2-R2：X/Y 世界轴独立全屏 Pass —— 片元着色器。
// 本 Pass 是轴线的唯一事实源（网格 Shader 不再画轴）：
// X 轴 = 世界 Y=0 线（金色 #AD8550）、Y 轴 = 世界 X=0 线（蓝色 #557C9E）；
// 屏幕恒定 1.25px 宽度，使用各自方向导数（不随网格 LOD 切换改变粗细、不形成楔形）；
// 开关独立于 ShowGrid（DrawPlan 按 ShowWorldAxes 单独发放）。

layout(push_constant) uniform GridPush {
    mat4 viewProjection;        // 0   世界→裁剪（深度投影用）
    mat4 inverseViewProjection; // 64  裁剪→世界（射线重建）
    vec4 cameraPosition;        // 128 相机世界位置
    vec4 viewportAndFar;        // 144 x,y=视口尺寸; z=Far; w=GridMaxDistance
    vec4 gridScale;             // 160 未使用（保持布局与网格 Pass 一致）
} pc;

layout(location = 0) in vec4 vFarWorld;
layout(location = 1) in vec4 vNearWorld;
layout(location = 0) out vec4 outColor;

const float AXIS_WIDTH_PX = 1.25;
const float DEPTH_BIAS_FACTOR = 0.5;
const float MIN_DEPTH_BIAS = 0.0000001;
const float MAX_DEPTH_BIAS = 0.00002;

void main() {
    vec3 nearWorld = vNearWorld.xyz / vNearWorld.w;
    vec3 farWorld = vFarWorld.xyz / vFarWorld.w;
    vec3 rayDirection = farWorld - nearWorld;

    if (abs(rayDirection.z) < 0.001) discard;
    float t = -nearWorld.z / rayDirection.z;
    if (t <= 0.0) discard;
    vec3 worldPosition = nearWorld + rayDirection * t;
    if (t > pc.viewportAndFar.w) discard;

    vec4 clipPosition = pc.viewProjection * vec4(worldPosition, 1.0);
    float depth = clipPosition.z / clipPosition.w;
    if (!(depth >= 0.0 && depth <= 1.0)) discard;
    float bias = clamp(fwidth(depth) * DEPTH_BIAS_FACTOR, MIN_DEPTH_BIAS, MAX_DEPTH_BIAS);
    gl_FragDepth = depth - bias;

    // 距离淡出 + 掠射角淡出（与网格 Pass 一致，地平线处平滑消失）。
    float distToCamera = length(worldPosition - pc.cameraPosition.xyz);
    float distanceFade = 1.0 - smoothstep(pc.viewportAndFar.z * 0.45,
        pc.viewportAndFar.z * 0.75, distToCamera);
    vec3 viewDirection = normalize(pc.cameraPosition.xyz - worldPosition);
    float grazingFactor = abs(dot(vec3(0.0, 0.0, 1.0), viewDirection));
    float grazingFade = smoothstep(0.015, 0.080, grazingFactor);

    // 各自方向导数：X 轴宽度只由 y 导数决定，Y 轴宽度只由 x 导数决定。
    float xAxisPx = abs(worldPosition.y) / max(fwidth(worldPosition.y), 0.000001);
    float yAxisPx = abs(worldPosition.x) / max(fwidth(worldPosition.x), 0.000001);
    float xAxis = 1.0 - smoothstep(AXIS_WIDTH_PX - 0.5, AXIS_WIDTH_PX + 0.5, xAxisPx);
    float yAxis = 1.0 - smoothstep(AXIS_WIDTH_PX - 0.5, AXIS_WIDTH_PX + 0.5, yAxisPx);

    vec3 axisXColor = vec3(0.678, 0.522, 0.314); // #AD8550 金色
    vec3 axisYColor = vec3(0.333, 0.486, 0.620); // #557C9E 蓝色

    float axisMask = max(xAxis, yAxis);
    if (axisMask < 0.005) discard;
    // 原点附近两轴重叠：按强度加权混合，避免浮点抖动闪烁。
    float weightX = xAxis / max(xAxis + yAxis, 0.000001);
    vec3 color = mix(axisYColor, axisXColor, weightX);
    float alpha = axisMask * 0.62 * distanceFade * grazingFade;
    outColor = vec4(color, alpha);
}

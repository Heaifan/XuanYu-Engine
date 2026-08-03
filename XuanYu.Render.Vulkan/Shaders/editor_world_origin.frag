#version 450

// MAP-A-R1-D5-R1-F2-R2：世界原点标记独立全屏 Pass —— 片元着色器。
// 只画世界原点标记（屏幕恒定 ~4px 半径，琥珀 #D1AE69 α0.70）；
// 开关独立于 ShowGrid/ShowWorldAxes（DrawPlan 按 ShowOrigin 单独发放）。

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

const float ORIGIN_RADIUS_PX = 4.0;
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

    float distToCamera = length(worldPosition - pc.cameraPosition.xyz);
    float distanceFade = 1.0 - smoothstep(pc.viewportAndFar.z * 0.45,
        pc.viewportAndFar.z * 0.75, distToCamera);
    vec3 viewDirection = normalize(pc.cameraPosition.xyz - worldPosition);
    float grazingFactor = abs(dot(vec3(0.0, 0.0, 1.0), viewDirection));
    float grazingFade = smoothstep(0.015, 0.080, grazingFactor);

    // 屏幕恒定半径（正方形标记，取两方向导数最大值）。
    float pixelRadius = max(abs(worldPosition.x), abs(worldPosition.y))
        / max(max(fwidth(worldPosition.x), fwidth(worldPosition.y)), 0.000001);
    float originMark = 1.0 - smoothstep(ORIGIN_RADIUS_PX - 0.5, ORIGIN_RADIUS_PX + 0.5, pixelRadius);
    if (originMark < 0.005) discard;

    vec3 originColor = vec3(0.820, 0.682, 0.412); // #D1AE69 琥珀
    float alpha = originMark * 0.70 * distanceFade * grazingFade;
    outColor = vec4(originColor, alpha);
}

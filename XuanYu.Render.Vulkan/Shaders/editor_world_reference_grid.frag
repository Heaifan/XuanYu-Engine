#version 450

// GRID-RW-2A：独立编辑器 World Reference Grid；只在 World XY（Z=0）平面绘制固定 100m 网格。
// Step/LOD 不由 Fragment 决策；fwidth 只参与线条抗锯齿。
layout(push_constant) uniform GridPush {
    mat4 viewProjection;
    mat4 inverseViewProjection;
    vec4 cameraPosition;
    vec4 viewportAndFar;
} pc;

layout(location = 0) in vec4 vFarWorld;
layout(location = 1) in vec4 vNearWorld;
layout(location = 0) out vec4 outColor;

const float STEP_METERS = 100.0;

float lineMask(float coordinate) {
    float derivative = max(fwidth(coordinate), 0.000001);
    float distanceToLine = abs(fract(coordinate - 0.5) - 0.5) / derivative;
    return 1.0 - smoothstep(0.65, 1.35, distanceToLine);
}

void main() {
    vec3 nearWorld = vNearWorld.xyz / vNearWorld.w;
    vec3 farWorld = vFarWorld.xyz / vFarWorld.w;
    vec3 rayDirection = farWorld - nearWorld;
    if (abs(rayDirection.z) < 0.000001) discard;
    float t = -nearWorld.z / rayDirection.z;
    if (t <= 0.0 || t > pc.viewportAndFar.z) discard;
    vec3 worldPosition = nearWorld + rayDirection * t;
    float grid = max(lineMask(worldPosition.x / STEP_METERS),
        lineMask(worldPosition.y / STEP_METERS));
    outColor = vec4(vec3(0.365, 0.400, 0.439), grid * 0.16);
}

#version 450

layout(push_constant) uniform ScenePush {
    mat4 viewProjection;
    vec4 worldPosition;
    float gizmoMode;
    float gizmoRingRadius;
    float selectionMode;
    float staticAlpha;
    vec4 entityRotation;
    vec4 entityScale;
} pc;

layout(location = 0) in vec3 inPosition;
layout(location = 1) in vec3 inNormal;
layout(location = 2) in vec2 inUv0;
layout(location = 0) out vec2 vLocal;
layout(location = 1) flat out float vSegmentLength;

void main() {
    vec4 a = pc.viewProjection * vec4(inPosition, 1.0);
    vec4 b = pc.viewProjection * vec4(inNormal, 1.0);
    vec2 pa = a.xy / a.w;
    vec2 pb = b.xy / b.w;
    vec2 viewport = max(vec2(pc.entityRotation.w, pc.entityScale.w), vec2(1.0));
    vec2 deltaPx = (pb - pa) * 0.5 * viewport;
    float lengthPx = length(deltaPx);
    vec2 directionPx = lengthPx > 0.000001 ? normalize(deltaPx) : vec2(1.0, 0.0);
    vec2 perpendicularPx = vec2(-directionPx.y, directionPx.x);
    float radiusPx = max(pc.gizmoRingRadius, 0.5);
    float t = inUv0.y;
    float segmentT = clamp(t, 0.0, 1.0);
    float localX = t < 0.0 ? t * radiusPx : (t > 1.0 ? lengthPx + (t - 1.0) * radiusPx : t * lengthPx);
    float localY = inUv0.x * radiusPx;
    vec2 localPx = directionPx * localX + perpendicularPx * localY;
    vec2 clip = pa + localPx * 2.0 / viewport;
    float w = mix(a.w, b.w, segmentT);
    gl_Position = vec4(clip * w, mix(a.z, b.z, segmentT), w);
    vLocal = vec2(localX, localY);
    vSegmentLength = lengthPx;
}

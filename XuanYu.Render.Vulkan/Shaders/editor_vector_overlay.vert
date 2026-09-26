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

void main() {
    vec4 a = pc.viewProjection * vec4(inPosition, 1.0);
    vec4 b = pc.viewProjection * vec4(inNormal, 1.0);
    vec2 pa = a.xy / a.w;
    vec2 pb = b.xy / b.w;
    vec2 delta = pb - pa;
    vec2 direction = length(delta) > 0.000001 ? normalize(delta) : vec2(1.0, 0.0);
    vec2 perpendicular = vec2(-direction.y, direction.x);
    vec2 viewport = max(vec2(pc.entityRotation.w, pc.entityScale.w), vec2(1.0));
    float t = inUv0.y;
    float segmentT = clamp(t, 0.0, 1.0);
    vec2 center = mix(pa, pb, segmentT);
    vec2 extension = direction * (t < 0.0 ? t : (t > 1.0 ? t - 1.0 : 0.0))
        * pc.gizmoRingRadius * 2.0 / viewport;
    vec2 offset = perpendicular * inUv0.x * pc.gizmoRingRadius * 2.0 / viewport;
    vec2 clip = center + extension + offset;
    float w = mix(a.w, b.w, segmentT);
    gl_Position = vec4(clip * w, mix(a.z, b.z, segmentT), w);
    vLocal = vec2(t, inUv0.x);
}

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

layout(location = 0) in vec2 vLocal;
layout(location = 0) out vec4 outColor;

float capsuleDistance(vec2 p) {
    float h = clamp(p.x, 0.0, 1.0);
    vec2 q = vec2(p.x - h, abs(p.y) - 1.0);
    return length(max(q, vec2(0.0))) + min(max(q.x, q.y), 0.0);
}

void main() {
    float distance = capsuleDistance(vLocal);
    float coverage = 1.0 - smoothstep(0.0, max(fwidth(distance), 0.0005), distance);
    outColor = vec4(pc.entityRotation.xyz, pc.staticAlpha * coverage);
}
